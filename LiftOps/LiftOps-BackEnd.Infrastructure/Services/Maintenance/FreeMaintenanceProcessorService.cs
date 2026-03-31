using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Maintenance
{
    public class FreeMaintenanceProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FreeMaintenanceProcessorService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(30); // Check every 30 days (approximately monthly)
        private const decimal DEFAULT_PRICE_PER_MONTH = 500m;

        public FreeMaintenanceProcessorService(
            IServiceProvider serviceProvider,
            ILogger<FreeMaintenanceProcessorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Free Maintenance Processor Service is starting.");

            // Run immediately on startup, then wait for the interval
            await ProcessFreeMaintenanceContractsAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                    await ProcessFreeMaintenanceContractsAsync();
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing free maintenance contracts.");
                    // Continue running even if there's an error
                }
            }

            _logger.LogInformation("Free Maintenance Processor Service is stopping.");
        }

        private async Task ProcessFreeMaintenanceContractsAsync()
        {
            _logger.LogInformation("Starting monthly free maintenance processing at {Time}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Build explicit tenant work items so background execution does not rely on ambient HttpContext.
                List<Guid> companyWorkItems;
                using (context.UseSystemTenantBypass())
                {
                    companyWorkItems = await context.MaintenanceContracts
                        .Where(c => c.Status == MaintenanceContractStatus.Active && c.FreeMonths > 0)
                        .Select(c => c.CompanyId)
                        .Distinct()
                        .ToListAsync();
                }

                if (!companyWorkItems.Any())
                {
                    _logger.LogInformation("No contracts with free months found.");
                    return;
                }

                int processedCount = 0;
                int expiredCount = 0;

                foreach (var companyId in companyWorkItems)
                {
                    var tenantResult = await ProcessCompanyContractsAsync(context, companyId);
                    processedCount += tenantResult.ProcessedCount;
                    expiredCount += tenantResult.ExpiredCount;
                }

                if (processedCount > 0)
                {
                    _logger.LogInformation(
                        "Free maintenance processing completed. Processed: {Processed}, Expired: {Expired}",
                        processedCount, expiredCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during free maintenance processing.");
                throw;
            }
        }

        private async Task<(int ProcessedCount, int ExpiredCount)> ProcessCompanyContractsAsync(ApplicationDbContext context, Guid companyId)
        {
            using var tenantScope = context.UseSystemTenantBypass(companyId);

            var contractsWithFreeMonths = await context.MaintenanceContracts
                .Where(c => c.CompanyId == companyId && c.Status == MaintenanceContractStatus.Active && c.FreeMonths > 0)
                .ToListAsync();

            if (!contractsWithFreeMonths.Any())
            {
                return (0, 0);
            }

            var currentDate = DateTime.UtcNow;
            var processedCount = 0;
            var expiredCount = 0;

            foreach (var contract in contractsWithFreeMonths)
            {
                try
                {
                    var monthsElapsed = CalculateMonthsElapsed(contract.StartDate, currentDate);
                    var previousFreeMonths = contract.FreeMonths;

                    if (monthsElapsed >= contract.FreeMonths)
                    {
                        contract.FreeMonths = 0;
                        if (contract.PricePerMonth == 0)
                        {
                            contract.PricePerMonth = DEFAULT_PRICE_PER_MONTH;
                            expiredCount++;
                        }
                    }
                    else
                    {
                        var expectedRemaining = Math.Max(0, contract.FreeMonths - monthsElapsed);
                        if (expectedRemaining < contract.FreeMonths)
                        {
                            contract.FreeMonths = expectedRemaining;
                        }
                    }

                    contract.LastModifiedAt = currentDate;
                    contract.LastModifiedBy = "System";
                    processedCount++;

                    _logger.LogInformation(
                        "Tenant {CompanyId} contract {ContractId} adjusted. FreeMonths {Previous}->{Current}.",
                        companyId, contract.Id, previousFreeMonths, contract.FreeMonths);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing tenant {CompanyId} contract {ContractId} (Project: {ProjectNumber})",
                        companyId,
                        contract.Id,
                        contract.ProjectNumber);
                }
            }

            await context.SaveChangesAsync();
            return (processedCount, expiredCount);
        }

        /// <summary>
        /// Calculates the number of complete months elapsed between start date and current date.
        /// </summary>
        private int CalculateMonthsElapsed(DateTime startDate, DateTime currentDate)
        {
            if (currentDate < startDate)
                return 0;

            // Calculate the difference in months
            var yearDiff = currentDate.Year - startDate.Year;
            var monthDiff = currentDate.Month - startDate.Month;
            var totalMonths = (yearDiff * 12) + monthDiff;

            // If the day of current date is before the day of start date, 
            // we haven't completed a full month yet
            if (currentDate.Day < startDate.Day)
            {
                totalMonths--;
            }

            return Math.Max(0, totalMonths);
        }
    }
}

