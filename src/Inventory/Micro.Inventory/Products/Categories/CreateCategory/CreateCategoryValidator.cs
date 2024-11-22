using FluentValidation;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;

namespace Micro.Inventory.Products.Categories.CreateCategory;

public class CreateCategoryValidator  : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Please specify a name for this product");
        
        RuleFor(x => x.Description).NotEmpty()
            .WithMessage("Please specify a description for this product");
    }
}