using FluentValidation;
using Micro.Inventory.Contracts.Categories.CreateCategory;

namespace Micro.Inventory.Categories.CreateCategory;

internal class CreateCategoryValidator  : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Please specify a name for this product");
        
        RuleFor(x => x.Description).NotEmpty()
            .WithMessage("Please specify a description for this product");
    }
}