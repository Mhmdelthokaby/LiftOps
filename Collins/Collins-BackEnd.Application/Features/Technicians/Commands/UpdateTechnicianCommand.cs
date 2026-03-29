using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Technicians.Commands
{
    public class UpdateTechnicianCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public UpdateTechnicianDto Dto { get; set; } = null!;

        public UpdateTechnicianCommand(Guid id, UpdateTechnicianDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }

    public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, Result<Guid>>
    {
        private readonly ITechnicianRepository _repository;
        private readonly ITechnicianService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UpdateTechnicianCommandHandler(ITechnicianRepository repository, ITechnicianService service, IMapper mapper, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Result<Guid>> Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
        {
            var tech = await _repository.GetByIdAsync(request.Id);
            if (tech == null) return Result<Guid>.Failure("Technician not found");

            // Handle username/password update if provided
            if (!string.IsNullOrWhiteSpace(request.Dto.Username) && !string.IsNullOrWhiteSpace(request.Dto.Password))
            {
                AppUser? user = null;
                
                // If technician already has a linked user, update it
                if (tech.UserId.HasValue)
                {
                    user = await _userManager.FindByIdAsync(tech.UserId.Value.ToString());
                }

                if (user != null)
                {
                    // Update existing user
                    var existingUserWithEmail = await _userManager.FindByEmailAsync(request.Dto.Username);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                    {
                        return Result<Guid>.Failure("A user with this username/email already exists.");
                    }

                    user.Email = request.Dto.Username;
                    user.UserName = request.Dto.Username;
                    user.FullName = request.Dto.Name;
                    user.PhoneNumber = request.Dto.Phone;

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        return Result<Guid>.Failure($"Failed to update user account: {errors}");
                    }

                    // Update password
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Dto.Password);
                    if (!passwordResult.Succeeded)
                    {
                        var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                        return Result<Guid>.Failure($"Failed to update password: {errors}");
                    }
                }
                else
                {
                    // Create new user account
                    var existingUser = await _userManager.FindByEmailAsync(request.Dto.Username);
                    if (existingUser != null)
                    {
                        return Result<Guid>.Failure("A user with this username/email already exists.");
                    }

                    user = new AppUser
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
            }

            _mapper.Map(request.Dto, tech); 
            await _service.UpdateTechnicianAsync(tech);
            
            return Result<Guid>.Success(tech.Id);
        }
    }
}
