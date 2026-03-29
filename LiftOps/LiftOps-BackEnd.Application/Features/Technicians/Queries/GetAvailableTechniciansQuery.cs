using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Technicians.Queries
{
    public class GetAvailableTechniciansQuery : IRequest<Result<IReadOnlyList<TechnicianDto>>>
    {
    }

    public class GetAvailableTechniciansQueryHandler : IRequestHandler<GetAvailableTechniciansQuery, Result<IReadOnlyList<TechnicianDto>>>
    {
        private readonly ITechnicianService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public GetAvailableTechniciansQueryHandler(ITechnicianService service, IMapper mapper, UserManager<AppUser> userManager)
        {
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Result<IReadOnlyList<TechnicianDto>>> Handle(GetAvailableTechniciansQuery request, CancellationToken cancellationToken)
        {
            var techs = await _service.GetAvailableTechniciansAsync();
            var dtos = _mapper.Map<IReadOnlyList<TechnicianDto>>(techs);
            
            // Populate username from linked AppUser
            foreach (var dto in dtos)
            {
                var tech = techs.FirstOrDefault(t => t.Id == dto.Id);
                if (tech?.UserId.HasValue == true)
                {
                    var user = await _userManager.FindByIdAsync(tech.UserId.Value.ToString());
                    if (user != null)
                    {
                        dto.Username = user.Email ?? user.UserName;
                    }
                }
            }
            
            return Result<IReadOnlyList<TechnicianDto>>.Success(dtos);
        }
    }
}
