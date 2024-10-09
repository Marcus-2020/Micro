using FluentValidation;

namespace Micro.Inventory.Products.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty()
            .WithMessage("Please specify a SKU for this product");
        
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Please specify a name for this product");
        
        RuleFor(x => x.Description).NotEmpty()
            .WithMessage("Please specify a description for this product");
        
        RuleFor(x => x.Type).NotEmpty()
            .WithMessage("Please specify a type for this product");
        
        RuleFor(x => x.UnitId).NotEmpty()
            .WithMessage("Please specify an unit for this product");
        
        RuleFor(x => x.PriceInfo.Cost).LessThan(0)
            .WithMessage("The cost can't be less than zero");
        
        RuleFor(x => x.PriceInfo.ProfitMargin).LessThan(0)
            .WithMessage("The profit margin can't be less than zero");
        
        RuleFor(x => x.PriceInfo.SalePrice).LessThan(0)
            .WithMessage("The sale price can't be less than zero");
    }
}