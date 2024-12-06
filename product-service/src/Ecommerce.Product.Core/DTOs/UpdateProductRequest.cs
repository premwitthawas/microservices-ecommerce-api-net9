namespace Ecommerce.Product.Core.DTOs;


public record UpdateProductRequest(
    Guid ProductID,
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? QuantityInStock
)
{
    public UpdateProductRequest() : this(default, default!, default, default, default)
    {

    }
};