using LiftOps_BackEnd.Application.DTOs;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Categories.Queries.ListCategories;

public record ListCategoriesQuery() : IRequest<List<CategoryDto>>;

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, List<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<Category>().ListAllAsync();
        
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();
    }
}
