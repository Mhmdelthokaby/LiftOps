using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class InstallationProjectService : IInstallationProjectService
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InstallationProjectService(IInstallationProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InstallationProject> CreateProjectAsync(InstallationProject project)
        {
            // Validate phone is provided (required)
            if (project.Customer != null && string.IsNullOrWhiteSpace(project.Customer.Phone))
            {
                throw new System.Exception("Phone number is required and must be unique.");
            }

            // Normalize phone number (trim and remove formatting) for comparison
            string normalizedPhone = string.Empty;
            string normalizedEmail = string.Empty;
            
            if (project.Customer != null)
            {
                normalizedPhone = NormalizePhone(project.Customer.Phone);
                normalizedEmail = project.Customer.Email?.Trim().ToLowerInvariant() ?? string.Empty;
                
                // Normalize the customer's phone and email
                project.Customer.Phone = normalizedPhone;
                if (!string.IsNullOrWhiteSpace(project.Customer.Email))
                {
                    project.Customer.Email = project.Customer.Email.Trim();
                }
                
                // All new projects require inspection/quotation approval
                // Set customer status to PendingInspectionQuotation until approved
                if (project.Customer.Status == default(CustomerStatus))
                {
                    project.Customer.Status = CustomerStatus.PendingInspectionQuotation;
                }
            }

            // Generate unique project number if not provided
            // GenerateProjectNumberAsync already ensures uniqueness
            if (string.IsNullOrWhiteSpace(project.ProjectNumber))
            {
                project.ProjectNumber = await GenerateProjectNumberAsync();
            }
            else
            {
                // Validate that the provided project number is unique
                if (await ProjectNumberExistsAsync(project.ProjectNumber))
                {
                    throw new System.Exception($"Project number '{project.ProjectNumber}' already exists. Please use a different project number.");
                }
            }

            // Customer Deduplication - Check for existing customer FIRST
            // This ensures we reuse existing customers instead of creating duplicates
            if (project.Customer != null)
            {
                // Get all customers once for efficiency
                var allCustomers = await _unitOfWork.Repository<Customer>().ListAllAsync();
                
                Customer? existingCustomer = null;
                
                // Priority 1: Check by phone (required, unique) - use normalized comparison
                if (!string.IsNullOrWhiteSpace(normalizedPhone))
                {
                    existingCustomer = allCustomers.FirstOrDefault(c => 
                        !string.IsNullOrWhiteSpace(c.Phone) && 
                        NormalizePhone(c.Phone) == normalizedPhone);
                }
                
                // Priority 2: If phone not found, check by email (if provided)
                if (existingCustomer == null && !string.IsNullOrWhiteSpace(normalizedEmail))
                {
                    existingCustomer = allCustomers.FirstOrDefault(c => 
                        !string.IsNullOrWhiteSpace(c.Email) && 
                        c.Email.Trim().ToLowerInvariant() == normalizedEmail);
                }
                
                if (existingCustomer != null)
                {
                    // Customer already exists - reuse it instead of creating new one
                    // This prevents duplicate customer creation when same phone/email is used
                    project.CustomerId = existingCustomer.Id;
                    // IMPORTANT: Set Customer to null to prevent EF from trying to insert a new customer
                    // When CustomerId is set and Customer is null, EF will use the existing customer
                    project.Customer = null;
                    
                    // Update existing customer status to PendingInspectionQuotation for new project
                    // All new projects require inspection approval, regardless of customer's previous status
                    if (existingCustomer.Status != CustomerStatus.PendingInspectionQuotation)
                    {
                        existingCustomer.Status = CustomerStatus.PendingInspectionQuotation;
                        _unitOfWork.Repository<Customer>().Update(existingCustomer);
                    }
                    
                    // Fallback logic: If project address/city is not provided, use customer's address/city
                    if (string.IsNullOrWhiteSpace(project.ProjectAddress))
                    {
                        project.ProjectAddress = existingCustomer.Address;
                    }
                    if (string.IsNullOrWhiteSpace(project.City) || project.City == "القاهرة الجديدة")
                    {
                        project.City = existingCustomer.City;
                    }
                }
                else
                {
                    // New customer - apply fallback logic using the new customer's address/city
                    if (string.IsNullOrWhiteSpace(project.ProjectAddress))
                    {
                        project.ProjectAddress = project.Customer?.Address ?? string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(project.City) || project.City == "القاهرة الجديدة")
                    {
                        project.City = project.Customer?.City ?? "القاهرة الجديدة";
                    }
                }
                // If no existing customer found, project.Customer remains set and will be created as new customer
            }

            _projectRepository.Add(project);
            await _unitOfWork.Complete();
            return project;
        }

        // Normalize phone number for comparison (remove spaces, dashes, parentheses)
        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
            return phone.Trim()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);
        }

        public async Task<Customer?> GetCustomerByContactAsync(string email, string phone)
        {
            // Phone is required and must be unique
            // Email is optional but must be unique if provided
            var customerRepository = _unitOfWork.Repository<Customer>();
            var allCustomers = await customerRepository.ListAllAsync();
            
            // Normalize inputs
            var normalizedPhone = NormalizePhone(phone ?? string.Empty);
            var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
            
            // First check by phone (required, unique) - use normalized comparison
            var customerByPhone = allCustomers.FirstOrDefault(c => 
                !string.IsNullOrWhiteSpace(c.Phone) && 
                NormalizePhone(c.Phone) == normalizedPhone);
            
            if (customerByPhone != null)
            {
                return customerByPhone;
            }
            
            // If email is provided, check by email (optional, but must be unique if provided)
            if (!string.IsNullOrWhiteSpace(normalizedEmail))
            {
                var customerByEmail = allCustomers.FirstOrDefault(c => 
                    !string.IsNullOrWhiteSpace(c.Email) && 
                    c.Email.Trim().ToLowerInvariant() == normalizedEmail);
                
                if (customerByEmail != null)
                {
                    return customerByEmail;
                }
            }
            
            return null;
        }

        public async Task UpdateProjectAsync(UpdateProjectDto dto)
        {
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
            if (project == null)
            {
                throw new System.Exception("Project not found");
            }

            // Update Customer
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(project.CustomerId);
            if (customer == null)
            {
                throw new System.Exception("Customer not found");
            }

            customer.Name = dto.Customer.Name;
            customer.Phone = NormalizePhone(dto.Customer.Phone);
            customer.Email = dto.Customer.Email?.Trim() ?? string.Empty;
            customer.Address = dto.Customer.Address;
            customer.City = dto.Customer.City;
            customer.ProjectNumber = dto.Customer.ProjectNumber;
            customer.GoogleMapsLink = dto.Customer.GoogleMapsLink;

            _unitOfWork.Repository<Customer>().Update(customer);

            // Update Project
            // Update ProjectNumber if provided and different
            if (!string.IsNullOrWhiteSpace(dto.Customer.ProjectNumber) && 
                !dto.Customer.ProjectNumber.Equals(project.ProjectNumber, StringComparison.OrdinalIgnoreCase))
            {
                // Check if new project number already exists
                if (await ProjectNumberExistsAsync(dto.Customer.ProjectNumber))
                {
                    throw new System.Exception($"Project with number {dto.Customer.ProjectNumber} already exists.");
                }
                project.ProjectNumber = dto.Customer.ProjectNumber;
            }
            
            // Update project address and city with fallback logic
            if (!string.IsNullOrWhiteSpace(dto.Contract.ProjectAddress))
            {
                project.ProjectAddress = dto.Contract.ProjectAddress;
            }
            else
            {
                // If project address is not provided, use customer address
                project.ProjectAddress = customer.Address;
            }
            
            if (!string.IsNullOrWhiteSpace(dto.Contract.City) && dto.Contract.City != "القاهرة الجديدة")
            {
                project.City = dto.Contract.City;
            }
            else
            {
                // If project city is not provided or is default, use customer city
                project.City = customer.City;
            }
            
            // Update GoogleMapsLink from contract if provided, otherwise use customer's GoogleMapsLink
            project.GoogleMapsLink = dto.Contract.GoogleMapsLink ?? customer.GoogleMapsLink;
            
            project.InstallationPricePerUnit = dto.Contract.InstallationPricePerUnit;
            project.TotalPrice = dto.Contract.TotalPrice;
            project.ContractDate = dto.Contract.ContractDate;
            project.InstallationStartDate = dto.Contract.InstallationStartDate;
            project.ExpectedFinishDate = dto.Contract.ExpectedFinishDate;
            project.Notes = dto.Contract.Notes;

            _projectRepository.Update(project);
            await _unitOfWork.Complete();
        }

        public async Task<bool> ProjectNumberExistsAsync(string projectNumber)
        {
            if (string.IsNullOrWhiteSpace(projectNumber)) return false;
            
            // Check in Installation Projects
            var allProjects = await _projectRepository.ListAllAsync();
            if (allProjects.Any(p => !string.IsNullOrWhiteSpace(p.ProjectNumber) && 
                                    p.ProjectNumber.Equals(projectNumber, StringComparison.OrdinalIgnoreCase)))
                return true;

            // Also check in Maintenance Contracts
            var maintenanceContractRepo = _unitOfWork.Repository<MaintenanceContract>();
            var maintenanceContracts = await maintenanceContractRepo.ListAllAsync();
            if (maintenanceContracts.Any(p => !string.IsNullOrWhiteSpace(p.ProjectNumber) && 
                                              p.ProjectNumber.Equals(projectNumber, StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        private async Task<string> GenerateProjectNumberAsync()
        {
            // Get all projects to find the highest number
            var allProjects = await _projectRepository.ListAllAsync();
            
            // Extract numbers from existing project numbers (format: PR-001, PRJ-123, etc.)
            int maxNumber = 0;
            foreach (var proj in allProjects)
            {
                if (string.IsNullOrWhiteSpace(proj.ProjectNumber)) continue;
                
                // Try to extract number from formats like "PR-10", "PRJ-001", "PR-123", etc.
                var parts = proj.ProjectNumber.Split('-');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[1], out int num))
                    {
                        if (num > maxNumber) maxNumber = num;
                    }
                }
            }
            
            // Generate next project number and ensure it's unique
            int nextNumber = maxNumber + 1;
            string newProjectNumber = $"PR-{nextNumber:D2}";
            
            // If the generated number already exists, keep incrementing until we find a unique one
            while (allProjects.Any(p => !string.IsNullOrWhiteSpace(p.ProjectNumber) && 
                                        p.ProjectNumber.Equals(newProjectNumber, StringComparison.OrdinalIgnoreCase)))
            {
                nextNumber++;
                newProjectNumber = $"PR-{nextNumber:D2}";
            }
            
            return newProjectNumber; // Format: PR-01, PR-02, etc.
        }

        public async Task<int> FixDuplicateProjectNumbersAsync()
        {
            var allProjects = await _projectRepository.ListAllAsync();
            var projectsByNumber = allProjects
                .Where(p => !string.IsNullOrWhiteSpace(p.ProjectNumber))
                .GroupBy(p => p.ProjectNumber, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            int fixedCount = 0;
            int nextNumber = 1;

            // Find the highest existing number
            foreach (var proj in allProjects)
            {
                if (string.IsNullOrWhiteSpace(proj.ProjectNumber)) continue;
                var parts = proj.ProjectNumber.Split('-');
                if (parts.Length >= 2 && int.TryParse(parts[1], out int num))
                {
                    if (num >= nextNumber) nextNumber = num + 1;
                }
            }

            // Fix duplicates
            foreach (var group in projectsByNumber)
            {
                var projects = group.ToList();
                // Keep the first one, reassign the rest
                for (int i = 1; i < projects.Count; i++)
                {
                    string newNumber;
                    do
                    {
                        newNumber = $"PR-{nextNumber:D2}";
                        nextNumber++;
                    } while (allProjects.Any(p => p.ProjectNumber.Equals(newNumber, StringComparison.OrdinalIgnoreCase)));

                    projects[i].ProjectNumber = newNumber;
                    _projectRepository.Update(projects[i]);
                    fixedCount++;
                }
            }

            if (fixedCount > 0)
            {
                await _unitOfWork.Complete();
            }

            return fixedCount;
        }
    }
}
