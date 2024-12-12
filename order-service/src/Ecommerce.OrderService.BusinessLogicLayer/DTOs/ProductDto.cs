namespace Ecommerce.OrderService.BusinessLogicLayer.DTOs;

public record ProductDto(Guid ProductID, string? ProductName,
string? Category, double UnitPrice, int QuantityInStock)
{
    public ProductDto() : this(default, default, default, default, default)
    {

    }
}