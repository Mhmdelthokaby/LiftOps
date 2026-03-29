using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands;

public class UpdateCustomerStatusCommand : IRequest<Result<Guid>>
{
    public Guid CustomerId { get; set; }
    public CustomerStatus Status { get; set; }

    public UpdateCustomerStatusCommand(Guid customerId, CustomerStatus status)
    {
        CustomerId = customerId;
        Status = status;
    }
}

public class UpdateCustomerStatusCommandHandler : IRequestHandler<UpdateCustomerStatusCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            return Result<Guid>.Failure("Customer not found");
        }

        customer.Status = request.Status;
        _unitOfWork.Repository<Customer>().Update(customer);
        
        var saveResult = await _unitOfWork.Complete();
        if (saveResult <= 0)
        {
            return Result<Guid>.Failure("Failed to update customer status");
        }

        return Result<Guid>.Success(customer.Id);
    }
}
