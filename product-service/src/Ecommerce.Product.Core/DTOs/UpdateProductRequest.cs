namespace Ecommerce.Product.Core.DTOs;


public record UpdateProductRequest(
    Guid ProductID,
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? Quantity
)
{
    public UpdateProductRequest() : this(default, default!, default, default, default)
    {

    }
};