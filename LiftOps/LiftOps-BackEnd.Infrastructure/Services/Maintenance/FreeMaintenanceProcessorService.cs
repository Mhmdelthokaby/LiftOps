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
            using var tenantBypass = context.UseSystemTenantBypass();

            try
            {
                // Get all active contracts with free months remaining
                var contractsWithFreeMonths = await context.MaintenanceContracts
                    .Where(c => c.Status == MaintenanceContractStatus.Active && c.FreeMonths > 0)
                    .ToListAsync();

                if (!contractsWithFreeMonths.Any())
                {
                    _logger.LogInformation("No contracts with free months found.");
                    return;
                }

                _logger.LogInformation("Found {Count} contracts with free months to process.", contractsWithFreeMonths.Count);

                var currentDate = DateTime.UtcNow;
                int processedCount = 0;
                int expiredCount = 0;

                foreach (var contract in contractsWithFreeMonths)
                {
                    try
                    {
                        // Calculate months elapsed since start date
                        var monthsElapsed = CalculateMonthsElapsed(contract.StartDate, currentDate);
                        var previousFreeMonths = contract.FreeMonths;
                        
                        // Check if free period has expired based on elapsed months
                        if (monthsElapsed >= contract.FreeMonths)
                        {
                            // Free period expired - set to paid with default price
                            contract.FreeMonths = 0;
                            
                            // Only set price if it's currently 0 (free)
                            if (contract.PricePerMonth == 0)
                            {
                                contract.PricePerMonth = DEFAULT_PRICE_PER_MONTH;
                                expiredCount++;
                                _logger.LogInformation(
                                    "Contract {ContractId} (Project: {ProjectNumber}) free period expired. Started: {StartDate} with {OriginalFreeMonths} free months, {MonthsElapsed} months elapsed. Set price to {Price}.",
                                    contract.Id, 
                                    contract.ProjectNumber, 
                                    contract.StartDate.ToString("yyyy-MM-dd"),
                                    previousFreeMonths,
                                    monthsElapsed, 
                                    DEFAULT_PRICE_PER_MONTH);
                            }
                        }
                        else
                        {
                            // Update free months to reflect elapsed time
                            // Calculate remaining: if monthsElapsed < FreeMonths, 
                            // remaining should be FreeMonths - monthsElapsed
                            // But to handle cases where service hasn't run for a while,
                            // we calculate: remaining = max(0, FreeMonths - monthsElapsed)
                            
                            // However, if this is the first run and monthsElapsed > 0,
                            // we need to catch up by setting FreeMonths to correct value
                            var expectedRemaining = Math.Max(0, contract.FreeMonths - monthsElapsed);
                            
                            // Only update if the calculated remaining is different from current
                            // This handles the case where service runs monthly and needs to catch up
                            if (expectedRemaining < contract.FreeMonths)
                            {
                                contract.FreeMonths = expectedRemaining;
                                
                                _logger.LogInformation(
                                    "Contract {ContractId} (Project: {ProjectNumber}) free months updated: {Previous} -> {Current} (Months elapsed: {Elapsed}).",
                                    contract.Id, contract.ProjectNumber, previousFreeMonths, contract.FreeMonths, monthsElapsed);
                            }
                        }

                        contract.LastModifiedAt = currentDate;
                        contract.LastModifiedBy = "System"; // Background service
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, 
                            "Error processing contract {ContractId} (Project: {ProjectNumber})",
                            contract.Id, contract.ProjectNumber);
                        // Continue with next contract
                    }
                }

                // Save all changes
                if (processedCount > 0)
                {
                    await context.SaveChangesAsync();
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

