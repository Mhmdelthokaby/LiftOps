using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Queries
{
    public class GetStageDetailsQuery : IRequest<Result<InstallationStageDto>>
    {
        public Guid StageId { get; set; }
    }

    public class GetStageDetailsQueryHandler : IRequestHandler<GetStageDetailsQuery, Result<InstallationStageDto>>
    {
        private readonly IInstallationStageRepository _stageRepository;
        private readonly IMapper _mapper;

        public GetStageDetailsQueryHandler(IInstallationStageRepository stageRepository, IMapper mapper)
        {
            _stageRepository = stageRepository;
            _mapper = mapper;
        }

        public async Task<Result<InstallationStageDto>> Handle(GetStageDetailsQuery request, CancellationToken cancellationToken)
        {
            var stage = await _stageRepository.GetStageWithPartsAsync(request.StageId);
            if (stage == null) return Result<InstallationStageDto>.Failure("Stage not found");

            var dto = _mapper.Map<InstallationStageDto>(stage);
            return Result<InstallationStageDto>.Success(dto);
        }
    }
}
