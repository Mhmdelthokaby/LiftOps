using AutoMapper;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Features.Installation.Commands;
using LiftOps_BackEnd.Application.Features.Technicians.Commands;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using Moq;

namespace LiftOps_BackEnd.Application.Tests;

public class InstallationIdentityHandlersTests
{
    [Fact]
    public async Task CreateInstallationProject_EmptyInstallationAdminId_ReturnsFailureWithoutCallingServices()
    {
        var projectService = new Mock<IInstallationProjectService>(MockBehavior.Strict);
        var elevatorService = new Mock<IElevatorService>(MockBehavior.Strict);
        var mapper = new Mock<IMapper>(MockBehavior.Strict);

        var handler = new CreateInstallationProjectCommandHandler(
            projectService.Object,
            elevatorService.Object,
            mapper.Object);

        var result = await handler.Handle(
            new CreateInstallationProjectCommand
            {
                InstallationAdminId = Guid.Empty,
                ProjectDto = new CreateProjectDto()
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(IdentityErrors.AuthenticatedUserIdentityRequired, result.Errors);
    }

    [Fact]
    public async Task CreateInspectionRequest_EmptyInstallationAdminId_ReturnsFailureWithoutPersistence()
    {
        var repository = new Mock<IInspectionRequestRepository>(MockBehavior.Strict);
        var customerRepository = new Mock<ICustomerRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var handler = new CreateInspectionRequestCommandHandler(
            repository.Object,
            customerRepository.Object,
            unitOfWork.Object);

        var result = await handler.Handle(
            new CreateInspectionRequestCommand
            {
                InstallationAdminId = Guid.Empty,
                Dto = new CreateInspectionRequestDto()
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(IdentityErrors.AuthenticatedUserIdentityRequired, result.Errors);
    }

    [Fact]
    public async Task CreateInspectionProject_EmptyInstallationAdminId_ReturnsFailureWithoutPersistence()
    {
        var projectRepository = new Mock<IInstallationProjectRepository>(MockBehavior.Strict);
        var customerRepository = new Mock<ICustomerRepository>(MockBehavior.Strict);
        var projectService = new Mock<IInstallationProjectService>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var handler = new CreateInspectionProjectCommandHandler(
            projectRepository.Object,
            customerRepository.Object,
            projectService.Object,
            unitOfWork.Object);

        var result = await handler.Handle(
            new CreateInspectionProjectCommand
            {
                InstallationAdminId = Guid.Empty,
                Dto = new CreateInspectionProjectDto()
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(IdentityErrors.AuthenticatedUserIdentityRequired, result.Errors);
    }

    [Fact]
    public async Task ConvertOfferToProject_EmptyInstallationAdminId_ReturnsFailureWithoutPersistence()
    {
        var offerRepository = new Mock<IOfferRepository>(MockBehavior.Strict);
        var inspectionRepository = new Mock<IInspectionRequestRepository>(MockBehavior.Strict);
        var customerRepository = new Mock<ICustomerRepository>(MockBehavior.Strict);
        var projectRepository = new Mock<IInstallationProjectRepository>(MockBehavior.Strict);
        var elevatorRepository = new Mock<IElevatorRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var handler = new ConvertOfferToProjectCommandHandler(
            offerRepository.Object,
            inspectionRepository.Object,
            customerRepository.Object,
            projectRepository.Object,
            elevatorRepository.Object,
            unitOfWork.Object);

        var result = await handler.Handle(
            new ConvertOfferToProjectCommand
            {
                OfferId = Guid.NewGuid(),
                InstallationAdminId = Guid.Empty
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(IdentityErrors.AuthenticatedUserIdentityRequired, result.Errors);
    }

    [Fact]
    public async Task AssignTechnician_EmptyAssignedByUserId_ReturnsFailureWithoutCallingService()
    {
        var technicianService = new Mock<ITechnicianService>(MockBehavior.Strict);

        var handler = new AssignTechnicianCommandHandler(technicianService.Object);

        var result = await handler.Handle(
            new AssignTechnicianCommand
            {
                AssignedByUserId = Guid.Empty,
                Dto = new AssignTechnicianDto { ElevatorId = Guid.NewGuid(), TechnicianIds = [] }
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(IdentityErrors.AuthenticatedUserIdentityRequired, result.Errors);
    }
}
