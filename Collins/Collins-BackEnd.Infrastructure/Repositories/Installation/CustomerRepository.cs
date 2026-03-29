using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Repositories.Installation
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }
    }
}
