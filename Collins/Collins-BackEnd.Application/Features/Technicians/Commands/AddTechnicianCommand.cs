using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Collins_BackEnd.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Collins_BackEnd.Application.Features.Technicians.Commands
{
    public class AddTechnicianCommand : IRequest<Result<Guid>>
    {
        public CreateTechnicianDto Dto { get; set; } = null!;
    }

    public class AddTechnicianCommandHandler : IRequestHandler<AddTechnicianCommand, Result<Guid>>
    {
        private readonly ITechnicianService _technicianService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AddTechnicianCommandHandler(ITechnicianService technicianService, IMapper mapper, UserManager<AppUser> userManager)
        {
            _technicianService = technicianService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Result<Guid>> Handle(AddTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tech = _mapper.Map<Technician>(request.Dto);
                
                // Create AppUser account if username and password are provided
                if (!string.IsNullOrWhiteSpace(request.Dto.Username) && !string.IsNullOrWhiteSpace(request.Dto.Password))
                {
                    // Check if username/email already exists
                    var existingUser = await _userManager.FindByEmailAsync(request.Dto.Username);
                    if (existingUser != null)
                    {
                        return Result<Guid>.Failure("A user with this username/email already exists.");
                    }

                    var user = new AppUser
                    {
                        FullName = request.Dto.Name,
                        Email = request.Dto.Username,
                        UserName = request.Dto.Username,
                        PhoneNumber = request.Dto.Phone,
                        IsDisabled = false
                    };

                    var createResult = await _userManager.CreateAsync(user, request.Dto.Password);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        return Result<Guid>.Failure($"Failed to create user account: {errors}");
                    }

                    // Assign Technician role
                    var roleResult = await _userManager.AddToRoleAsync(user, Domain.Common.Roles.Technician);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        return Result<Guid>.Failure($"Failed to assign Technician role: {errors}");
                    }

                    // Link technician to user
                    tech.UserId = user.Id;
                }

                var createdTech = await _technicianService.AddTechnicianAsync(tech);
                return Result<Guid>.Success(createdTech.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
