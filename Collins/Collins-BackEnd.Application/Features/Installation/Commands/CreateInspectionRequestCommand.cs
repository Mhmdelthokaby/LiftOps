using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Collins_BackEnd.Domain.Interfaces;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class CreateInspectionRequestCommand : IRequest<Result<Guid>>
    {
        public CreateInspectionRequestDto Dto { get; set; } = null!;
        public Guid InstallationAdminId { get; set; }
    }

    public class CreateInspectionRequestCommandHandler : IRequestHandler<CreateInspectionRequestCommand, Result<Guid>>
    {
        private readonly IInspectionRequestRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInspectionRequestCommandHandler(
            IInspectionRequestRepository repository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateInspectionRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Guid? clientId = null;

                // If ClientId is provided, verify it exists
                if (request.Dto.ClientId.HasValue)
                {
                    var existingClient = await _customerRepository.GetByIdAsync(request.Dto.ClientId.Value);
                    if (existingClient == null)
                    {
                        return Result<Guid>.Failure("Specified client not found.");
                    }
                    clientId = request.Dto.ClientId.Value;
                }

                var inspection = new InspectionRequest
                {
                    ClientId = clientId, // Link to existing client if provided
                    ClientName = request.Dto.ClientName,
                    ClientPhone = request.Dto.ClientPhone,
                    ClientEmail = request.Dto.ClientEmail,
                    ProjectAddress = request.Dto.ProjectAddress,
                    GoogleMapsLink = request.Dto.GoogleMapsLink,
                    NumberOfElevatorsRequired = request.Dto.NumberOfElevatorsRequired,
                    ElevatorType = request.Dto.ElevatorType,
                    Notes = request.Dto.Notes,
                    Status = InspectionStatus.PendingInspection,
                    CreatedByAdminId = request.InstallationAdminId
                };

                _repository.Add(inspection);
                await _unitOfWork.Complete();

                return Result<Guid>.Success(inspection.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create inspection request: {ex.Message}");
            }
        }
    }
}

