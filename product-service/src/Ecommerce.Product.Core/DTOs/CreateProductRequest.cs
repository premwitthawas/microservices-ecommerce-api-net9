namespace Ecommerce.Product.Core.DTOs;


public record CreateProductRequest(
    string ProductName,
    CategoryOptions Category,
    double UnitPrice,
    int QuantityInStock
)
{
    public CreateProductRequest() : this(default!, default, default, default)
    {

    }
};