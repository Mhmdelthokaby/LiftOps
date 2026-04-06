using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDetailDto>>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CompanyDetailDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await CompanyDetailAssembler.BuildAsync(_db, request.Id, cancellationToken);
        if (dto == null)
        {
            return Result<CompanyDetailDto>.Failure("Company not found.");
        }

        return Result<CompanyDetailDto>.Success(dto);
    }
}
