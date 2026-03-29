using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Queries
{
    public class GetNotificationsQuery : IRequest<Result<IReadOnlyList<NotificationDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public GetNotificationsQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(request.UserId);
            var dtos = _mapper.Map<IReadOnlyList<NotificationDto>>(notifications);
            return Result<IReadOnlyList<NotificationDto>>.Success(dtos);
        }
    }
}
