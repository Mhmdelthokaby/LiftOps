using LiftOps_BackEnd.Application.DTOs;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Categories.Commands.AddCategory;

public record AddCategoryCommand(CreateCategoryDto Dto) : IRequest<Guid?>;

public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, Guid?>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid?> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Dto.Name,
            Description = request.Dto.Description
        };

        _unitOfWork.Repository<Category>().Add(category);
        var result = await _unitOfWork.Complete();

        return result > 0 ? category.Id : null;
    }
}
