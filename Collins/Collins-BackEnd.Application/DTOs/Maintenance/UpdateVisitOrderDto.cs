using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Application.DTOs.Maintenance
{
    public class UpdateVisitOrderDto
    {
        public DateTime Date { get; set; }
        public List<Guid> VisitIds { get; set; } = new List<Guid>(); // Ordered list of visit IDs
    }
}
