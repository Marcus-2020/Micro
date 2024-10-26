using FluentValidation;
using Micro.Inventory.Contracts.Products.CreateProduct;

namespace Micro.Inventory.Products.CreateProduct.Validators;

internal class CreateProductValidator : AbstractValidator<CreateProductRequest>
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
        
        RuleFor(x => x.PriceInfo.CostPrice).GreaterThan(0)
            .WithMessage("The cost can't be less than zero");
        
        RuleFor(x => x.PriceInfo.ProfitMargin).GreaterThan(0)
            .WithMessage("The profit margin can't be less than zero");
        
        RuleFor(x => x.PriceInfo.SellingPrice).GreaterThan(0)
            .WithMessage("The sale price can't be less than zero");
    }
}