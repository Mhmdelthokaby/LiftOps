using Collins_BackEnd.Application.Features.Installation.DTOs;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCustomersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.Repository<Customer>().ListAllAsync();

        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            ProjectNumber = c.ProjectNumber,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}
