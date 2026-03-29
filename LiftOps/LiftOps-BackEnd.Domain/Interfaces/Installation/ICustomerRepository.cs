using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
    }
}
