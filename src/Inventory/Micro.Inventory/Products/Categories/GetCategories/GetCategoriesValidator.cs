using FluentValidation;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;

namespace Micro.Inventory.Products.Categories.GetCategories;

public class GetCategoriesValidator : AbstractValidator<GetCategoriesRequest>
{
    public GetCategoriesValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0).WithMessage("Skip can't be less than zero");
        
        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(0).WithMessage("Take can't be less than zero");
        
        RuleFor(x => x.Take)
            .LessThan(100).WithMessage("Take can't be greater than 100");
    }
}