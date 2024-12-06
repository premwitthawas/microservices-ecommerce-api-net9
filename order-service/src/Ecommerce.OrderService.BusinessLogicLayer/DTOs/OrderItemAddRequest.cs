namespace Ecommerce.OrderService.BusinessLogicLayer.DTOs;

public record OrderItemAddRequest(Guid ProductID, decimal UnitPrice, int Quantity)
{
    public OrderItemAddRequest() : this(default, default, default)
    {

    }
}