using LiftOps_BackEnd.Application.Features.Installation.DTOs;
using MediatR;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Application.Features.Installation.Queries.GetAllCustomers;

public class GetAllCustomersQuery : IRequest<List<CustomerDto>>
{
}
