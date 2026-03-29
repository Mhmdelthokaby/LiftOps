using Collins_BackEnd.Application.DTOs.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface IPartSelectionService
    {
        Task AddPartsToStageAsync(Guid stageId, List<PartSelectionDto> parts);
    }
}
