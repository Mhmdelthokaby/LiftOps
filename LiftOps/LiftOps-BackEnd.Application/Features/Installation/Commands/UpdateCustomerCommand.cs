using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.Installation.DTOs;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands;

public class UpdateCustomerCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public UpdateCustomerDto Dto { get; set; } = null!;

    public UpdateCustomerCommand(Guid id, UpdateCustomerDto dto)
    {
        Id = id;
        Dto = dto;
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.Id);
        if (customer == null)
        {
            return Result<Guid>.Failure("Customer not found");
        }

        // Update customer properties
        customer.Name = request.Dto.Name;
        customer.Email = request.Dto.Email;
        customer.Phone = request.Dto.Phone;
        customer.Address = request.Dto.Address ?? string.Empty;
        customer.ProjectNumber = request.Dto.ProjectNumber ?? string.Empty;
        customer.GoogleMapsLink = request.Dto.GoogleMapsLink;

        _unitOfWork.Repository<Customer>().Update(customer);
        
        // Save changes
        var saveResult = await _unitOfWork.Complete();
        if (saveResult <= 0)
        {
            return Result<Guid>.Failure("Failed to update customer");
        }

        return Result<Guid>.Success(customer.Id);
    }
}

