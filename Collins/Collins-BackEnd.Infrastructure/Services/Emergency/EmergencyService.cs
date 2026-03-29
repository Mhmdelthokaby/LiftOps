using Collins_BackEnd.Application.Interfaces.Emergency;
using Collins_BackEnd.Domain.Entities.Emergency;
using Collins_BackEnd.Domain.Interfaces;
using Collins_BackEnd.Domain.Interfaces.Emergency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Services.Emergency
{
    public class EmergencyService : IEmergencyService
    {
        private readonly IEmergencyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EmergencyService(
            IEmergencyRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmergencyTicket> CreateTicketAsync(EmergencyTicket ticket)
        {
            ticket.TicketNumber = await _repository.GetNextTicketNumberAsync();
            ticket.Status = EmergencyStatus.Open;
            ticket.ReportedAt = DateTime.UtcNow;

            _repository.Add(ticket);
            await _unitOfWork.Complete();
            return ticket;
        }

        public async Task<EmergencyTicket> UpdateTicketAsync(Guid ticketId, EmergencyTicket ticket)
        {
            var existingTicket = await _repository.GetByIdAsync(ticketId);
            if (existingTicket == null)
                throw new Exception("Emergency ticket not found");

            existingTicket.Project = ticket.Project;
            existingTicket.Location = ticket.Location;
            existingTicket.UnitId = ticket.UnitId;
            existingTicket.GoogleMapsLink = ticket.GoogleMapsLink;
            existingTicket.Priority = ticket.Priority;
            existingTicket.Status = ticket.Status;
            existingTicket.Description = ticket.Description;
            existingTicket.ReportedBy = ticket.ReportedBy;
            existingTicket.Contact = ticket.Contact;
            existingTicket.AssignedTechnicianId = ticket.AssignedTechnicianId;
            existingTicket.Notes = ticket.Notes;

            if (ticket.Status == EmergencyStatus.Resolved && existingTicket.Status != EmergencyStatus.Resolved)
            {
                existingTicket.ResolvedDate = DateTime.UtcNow;
            }

            _repository.Update(existingTicket);
            await _unitOfWork.Complete();
            return existingTicket;
        }

        public async Task DeleteTicketAsync(Guid ticketId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new Exception("Emergency ticket not found");

            _repository.Delete(ticket);
            await _unitOfWork.Complete();
        }

        public async Task<EmergencyTicket?> GetTicketByIdAsync(Guid ticketId)
        {
            return await _repository.GetByIdAsync(ticketId);
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetAllTicketsAsync()
        {
            return await _repository.ListAllAsync();
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetOpenTicketsAsync()
        {
            return await _repository.GetOpenTicketsAsync();
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetTicketsByTechnicianAsync(Guid technicianId)
        {
            return await _repository.GetTicketsByTechnicianAsync(technicianId);
        }

        public async Task AssignTechnicianAsync(Guid ticketId, Guid technicianId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new Exception("Emergency ticket not found");

            ticket.AssignedTechnicianId = technicianId;
            if (ticket.Status == EmergencyStatus.Open)
            {
                ticket.Status = EmergencyStatus.EnRoute;
            }

            _repository.Update(ticket);
            await _unitOfWork.Complete();
        }

        public async Task ResolveTicketAsync(Guid ticketId, string? notes = null)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new Exception("Emergency ticket not found");

            ticket.Status = EmergencyStatus.Resolved;
            ticket.ResolvedDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(notes))
            {
                ticket.Notes = notes;
            }

            _repository.Update(ticket);
            await _unitOfWork.Complete();
        }
    }
}

