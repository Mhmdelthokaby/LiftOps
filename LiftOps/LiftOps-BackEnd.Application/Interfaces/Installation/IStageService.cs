using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Installation
{
    public interface IStageService
    {
        Task StartStageAsync(Guid stageId, DateTime startDate);
        Task UpdateStageAsync(Guid stageId, List<PartSelectionDto>? parts, string? notes, decimal? supplyCost, decimal? stagePrice, DateTime? endDate, List<Guid>? technicianIds);
        Task CompleteStageAsync(Guid stageId, decimal? supplyCost, string? notes, DateTime? endDate, decimal? price, bool collectPrice, int? freeMonths, List<TechnicianRatingDto>? technicianRatings);
    }
}
