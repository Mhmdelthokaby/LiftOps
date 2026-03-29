using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Installation
{
    /// <summary>
    /// Service interface for calculating and updating customer status based on projects
    /// </summary>
    public interface ICustomerStatusService
    {
        /// <summary>
        /// Gets the calculated customer status for a customer based on their projects
        /// This is used to check status without updating the database
        /// </summary>
        Task<CustomerStatus> GetCalculatedCustomerStatusAsync(Guid customerId);

        /// <summary>
        /// Recalculates and updates customer status based on all their projects
        /// </summary>
        Task UpdateCustomerStatusAsync(Guid customerId);
    }
}

