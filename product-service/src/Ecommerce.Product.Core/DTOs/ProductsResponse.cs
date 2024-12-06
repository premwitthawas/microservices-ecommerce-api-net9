namespace Ecommerce.Product.Core.DTOs;


public record ProductsResponse(
    Guid ProductID,
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? QuantityInStock
)
{
    public ProductsResponse() : this(default, default!, default, default, default)
    {

    }
};