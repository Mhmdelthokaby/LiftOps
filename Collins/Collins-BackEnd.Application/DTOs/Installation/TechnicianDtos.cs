using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Application.DTOs.Installation
{
    public class CreateTechnicianDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public Guid? LeaderId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class UpdateTechnicianDto : CreateTechnicianDto
    {
    }

    public class AssignTechnicianDto
    {
        public Guid ElevatorId { get; set; }
        public List<Guid> TechnicianIds { get; set; } = new List<Guid>();
    }

    public class TechnicianDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public int TotalElevatorsInstalled { get; set; }
        public int CurrentActiveElevatorsCount { get; set; }
        public double? OverallRating { get; set; }
        public bool IsDisabled { get; set; }
        public Guid? LeaderId { get; set; }
        public string? LeaderName { get; set; }
        public bool IsLeader { get; set; }
        public string? Username { get; set; }
    }
}
