using AutoMapper;
using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using System.Linq;

namespace Collins_BackEnd.Application.Mappings
{
    public class InstallationMappingProfile : Profile
    {
        public InstallationMappingProfile()
        {
            // Create Project Mappings
            CreateMap<CustomerDto, Customer>();
            CreateMap<CreateElevatorDto, Elevator>()
                .ForMember(dest => dest.ElevatorType, opt => opt.MapFrom(src => ParseElevatorType(src.ElevatorType)))
                .ForMember(dest => dest.PitType, opt => opt.MapFrom(src => ParsePitType(src.PitType)))
                .ForMember(dest => dest.FloorsCount, opt => opt.MapFrom(src => src.FloorsCount > 0 ? src.FloorsCount : src.NumberOfFloors))
                .ForMember(dest => dest.StopsCount, opt => opt.MapFrom(src => src.StopsCount > 0 ? src.StopsCount : src.NumberOfStops))
                .ForMember(dest => dest.NumberOfFloors, opt => opt.MapFrom(src => src.FloorsCount > 0 ? src.FloorsCount : src.NumberOfFloors))
                .ForMember(dest => dest.NumberOfStops, opt => opt.MapFrom(src => src.StopsCount > 0 ? src.StopsCount : src.NumberOfStops));
            CreateMap<ContractDto, InstallationProject>()
                .ForMember(dest => dest.InstallationPricePerUnit, opt => opt.MapFrom(src => src.InstallationPricePerUnit))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.ProjectAddress, opt => opt.MapFrom(src => src.ProjectAddress))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.GoogleMapsLink, opt => opt.MapFrom(src => src.GoogleMapsLink)); // Map strict contract fields
            
            // Query Mappings
            CreateMap<InstallationProject, InstallationProjectDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.Phone))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.Customer.Address))
                .ForMember(dest => dest.CustomerCity, opt => opt.MapFrom(src => src.Customer.City))
                .ForMember(dest => dest.ProjectNumber, opt => opt.MapFrom(src => src.ProjectNumber)) // Use project's own ProjectNumber
                .ForMember(dest => dest.ProjectAddress, opt => opt.MapFrom(src => GetEffectiveAddress(src))) // Use project address or fallback to customer address
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => GetEffectiveCity(src))) // Use project city or fallback to customer city
                .ForMember(dest => dest.GoogleMapsLink, opt => opt.MapFrom(src => GetEffectiveGoogleMapsLink(src))) // Use project GoogleMapsLink or fallback to customer
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CalculateProjectStatus(src)));
            
            CreateMap<Elevator, ElevatorDto>()
                .ForMember(dest => dest.ElevatorType, opt => opt.MapFrom(src => src.ElevatorType.ToString()))
                .ForMember(dest => dest.PitType, opt => opt.MapFrom(src => src.PitType.ToString()))
                .ForMember(dest => dest.FloorsCount, opt => opt.MapFrom(src => src.FloorsCount > 0 ? src.FloorsCount : src.NumberOfFloors))
                .ForMember(dest => dest.StopsCount, opt => opt.MapFrom(src => src.StopsCount > 0 ? src.StopsCount : src.NumberOfStops))
                .ForMember(dest => dest.NumberOfFloors, opt => opt.MapFrom(src => src.NumberOfFloors))
                .ForMember(dest => dest.NumberOfStops, opt => opt.MapFrom(src => src.NumberOfStops))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => CalculatePaidAmount(src)))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => CalculateRemainingAmount(src)));
            CreateMap<StageRequiredPart, StageRequiredPartDto>()
                .ForMember(dest => dest.InventoryItemName, opt => opt.MapFrom(src => src.InventoryItem != null ? src.InventoryItem.Name : string.Empty))
                .ForMember(dest => dest.InventoryItemNumber, opt => opt.MapFrom(src => src.InventoryItem != null ? src.InventoryItem.ItemNumber : string.Empty));
            
            CreateMap<StageTechnician, StageTechnicianDto>()
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? src.Technician.Name : string.Empty));
            
            CreateMap<InstallationStage, InstallationStageDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            
            CreateMap<Notification, NotificationDto>();
            
            // Technician Mappings
            CreateMap<CreateTechnicianDto, Technician>();
            CreateMap<UpdateTechnicianDto, Technician>();
            CreateMap<Technician, TechnicianDto>()
                .ForMember(dest => dest.LeaderName, opt => opt.MapFrom(src => src.Leader != null ? src.Leader.Name : null))
                .ForMember(dest => dest.IsLeader, opt => opt.MapFrom(src => src.LeaderId == null))
                .ForMember(dest => dest.OverallRating, opt => opt.MapFrom(src => src.OverallRating));
        }

        private string CalculateProjectStatus(InstallationProject project)
        {
            // First, check ProjectStatus enum - this takes priority
            // If project is UnderInspectionAndQuotation, it's pending regardless of QuotationId
            if (project.ProjectStatus == ProjectStatus.UnderInspectionAndQuotation)
            {
                return "Pending";
            }
            
            if (project.ProjectStatus == ProjectStatus.Rejected)
            {
                return "Rejected";
            }
            
            if (project.ProjectStatus == ProjectStatus.Approved)
            {
                // Approved projects are ready to start
                if (project.Elevators == null || project.Elevators.Count == 0)
                {
                    return "Active";
                }
                // If approved and has elevators, calculate based on stages
                return CalculateStatusFromStages(project);
            }
            
            if (project.ProjectStatus == ProjectStatus.Active)
            {
                // Active projects are in progress
                if (project.Elevators == null || project.Elevators.Count == 0)
                {
                    return "Active";
                }
                return CalculateStatusFromStages(project);
            }

            // For existing projects (created before the new flow - no QuotationId and default ProjectStatus)
            // Only treat as existing if ProjectStatus is default (0) which is UnderInspectionAndQuotation
            // But we already handled that above, so this is for legacy projects
            // If QuotationId is null and ProjectStatus is not explicitly set, calculate from stages
            if (project.QuotationId == null && project.ProjectStatus == default(ProjectStatus))
            {
                if (project.Elevators == null || project.Elevators.Count == 0)
                {
                    return "Active";
                }
                return CalculateStatusFromStages(project, requireFourStages: true);
            }
            else if (project.ProjectStatus == ProjectStatus.Rejected)
            {
                return "Rejected";
            }
            else if (project.ProjectStatus == ProjectStatus.Approved)
            {
                // Approved projects are ready to start
                if (project.Elevators == null || project.Elevators.Count == 0)
                {
                    return "Active";
                }
                // If approved and has elevators, calculate based on stages
                return CalculateStatusFromStages(project);
            }
            else if (project.ProjectStatus == ProjectStatus.Active)
            {
                // Active projects are in progress
                if (project.Elevators == null || project.Elevators.Count == 0)
                {
                    return "Active";
                }
                return CalculateStatusFromStages(project);
            }

            // Fallback: calculate based on stages
            if (project.Elevators == null || project.Elevators.Count == 0)
            {
                return "Active";
            }

            return CalculateStatusFromStages(project, requireFourStages: true);
        }

        private string CalculateStatusFromStages(InstallationProject project, bool requireFourStages = false)
        {
            if (project.Elevators == null || project.Elevators.Count == 0)
            {
                return "Active";
            }

            bool allComplete = true;
            bool anyInProgress = false;

            foreach (var elevator in project.Elevators)
            {
                if (elevator.Stages == null || elevator.Stages.Count == 0)
                {
                    allComplete = false;
                    continue;
                }

                // For existing projects, must have exactly 4 stages
                if (requireFourStages && elevator.Stages.Count != 4)
                {
                    allComplete = false;
                }

                foreach (var stage in elevator.Stages)
                {
                    if (stage.Status == StageStatus.InProgress)
                    {
                        anyInProgress = true;
                        allComplete = false;
                    }
                    else if (stage.Status != StageStatus.Success)
                    {
                        allComplete = false;
                    }
                }
            }

            if (allComplete)
            {
                return "Completed";
            }
            else if (anyInProgress)
            {
                return "InProgress";
            }
            else
            {
                return "Active";
            }
        }

        private static ElevatorType ParseElevatorType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return ElevatorType.WithMachineRoom;
            
            if (Enum.TryParse<ElevatorType>(value, true, out var result))
                return result;
            
            return ElevatorType.WithMachineRoom;
        }

        private static PitType ParsePitType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return PitType.Concrete;
            
            if (Enum.TryParse<PitType>(value, true, out var result))
                return result;
            
            return PitType.Concrete;
        }

        /// <summary>
        /// Calculates the total amount paid from completed stages.
        /// Only includes stages that are Success and have IsPriceCollected = true.
        /// </summary>
        private static decimal CalculatePaidAmount(Elevator elevator)
        {
            if (elevator.Stages == null || elevator.Stages.Count == 0)
                return 0;

            return elevator.Stages
                .Where(s => s.Status == StageStatus.Success && s.IsPriceCollected && s.StagePrice.HasValue)
                .Sum(s => s.StagePrice!.Value);
        }

        /// <summary>
        /// Calculates the remaining amount: Total Price - Paid Amount.
        /// The elevator price remains constant, and this shows how much is still owed.
        /// </summary>
        private static decimal CalculateRemainingAmount(Elevator elevator)
        {
            var paidAmount = CalculatePaidAmount(elevator);
            return elevator.Price - paidAmount;
        }

        /// <summary>
        /// Gets the effective address for a project. If project address is not set, returns customer address.
        /// </summary>
        private static string GetEffectiveAddress(InstallationProject project)
        {
            if (!string.IsNullOrWhiteSpace(project.ProjectAddress))
            {
                return project.ProjectAddress;
            }
            return project.Customer?.Address ?? string.Empty;
        }

        /// <summary>
        /// Gets the effective city for a project. If project city is not set or is default, returns customer city.
        /// </summary>
        private static string GetEffectiveCity(InstallationProject project)
        {
            if (!string.IsNullOrWhiteSpace(project.City) && project.City != "القاهرة الجديدة")
            {
                return project.City;
            }
            return project.Customer?.City ?? "القاهرة الجديدة";
        }

        /// <summary>
        /// Gets the effective Google Maps link for a project. If project GoogleMapsLink is not set, returns customer GoogleMapsLink.
        /// </summary>
        private static string? GetEffectiveGoogleMapsLink(InstallationProject project)
        {
            if (!string.IsNullOrWhiteSpace(project.GoogleMapsLink))
            {
                return project.GoogleMapsLink;
            }
            return project.Customer?.GoogleMapsLink;
        }
    }
}
