using Collins_BackEnd.Application.Interfaces.Faults;
using Collins_BackEnd.Application.Interfaces.Installation; // Corrected
using Collins_BackEnd.Domain.Entities.Faults;
using Collins_BackEnd.Domain.Interfaces.Faults;
using Collins_BackEnd.Domain.Interfaces; // IUnitOfWork
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Services.Faults
{
    public class FaultService : IFaultService
    {
        private readonly IFaultRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IGenericRepository<Collins_BackEnd.Domain.Entities.InventoryItem> _inventoryRepo;

        public FaultService(
            IFaultRepository repository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _inventoryRepo = unitOfWork.Repository<Collins_BackEnd.Domain.Entities.InventoryItem>();
        }

        public async Task<FaultTicket> CreateTicketAsync(FaultTicket ticket)
        {
            ticket.TicketNumber = $"FLT-{DateTime.UtcNow.Ticks}"; // Simple ID gen
            ticket.Status = TicketStatus.Pending;
            ticket.FaultDate = DateTime.UtcNow;

            _repository.Add(ticket);
            await _unitOfWork.Complete();
            return ticket;
        }

        public async Task AssignTechnicianAsync(Guid ticketId, Guid technicianId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found");

            ticket.AssignedTechnicianId = technicianId;
            ticket.Status = TicketStatus.InProgress;
            
            _repository.Update(ticket);
            await _unitOfWork.Complete();
        }

        public async Task AddSparePartsAsync(Guid ticketId, List<(Guid ItemId, int Qty)> parts)
        {
             var ticket = await _repository.GetByIdAsync(ticketId);
             if (ticket == null) throw new Exception("Ticket not found");
             
             foreach(var part in parts)
             {
                 var item = await _inventoryRepo.GetByIdAsync(part.ItemId);
                 if(item == null) continue;

                 var usage = new FaultSparePartUsage
                 {
                     FaultTicketId = ticketId,
                     InventoryItemId = part.ItemId,
                     Quantity = part.Qty,
                     PriceAtTimeOfUsage = item.UnitPrice,
                     IsPaid = false
                 };
                 
                 // How to add to collection? 
                 // If not eagerly loaded, we should add via context or load it.
                 // Assuming I can add via dbset or I reload ticket with parts.
                 // Or better: _unitOfWork.Repository<FaultSparePartUsage>().Add(usage);
                 
                 // I'll assume we can use the generic repo for the child entity
                 var usageRepo = _unitOfWork.Repository<FaultSparePartUsage>();
                 usageRepo.Add(usage);
                 
                 // Stock Logic
                 if (item.StockQuantity < part.Qty)
                 {
                      await _notificationService.CreateNotificationAsync(new Domain.Entities.Installation.Notification
                      {
                            Title = "Low/No Stock Used in Fault",
                            Message = $"Item {item.Name} used in Fault {ticket.TicketNumber} but stock was insufficient.",
                            Type = Domain.Entities.Installation.NotificationType.OutOfStock,
                            CreatedAt = DateTime.UtcNow
                      });
                 }
                 item.StockQuantity -= part.Qty;
                 _inventoryRepo.Update(item);
             }
             await _unitOfWork.Complete();
        }

        public async Task ResolveTicketAsync(Guid ticketId, string notes)
        {
             var ticket = await _repository.GetByIdAsync(ticketId);
             if (ticket == null) throw new Exception("Ticket not found");

             ticket.Status = TicketStatus.Done;
             ticket.ResolvedDate = DateTime.UtcNow;
             ticket.Notes = notes;
             
             _repository.Update(ticket);
             await _unitOfWork.Complete();
        }

        public async Task<IReadOnlyList<FaultTicket>> GetOpenTicketsAsync()
        {
            return await _repository.GetOpenTicketsAsync();
        }
        
        public async Task<IReadOnlyList<FaultTicket>> GetAllTicketsAsync()
        {
            return await _repository.ListAllAsync();
        }
    }
}
