namespace Ecommerce.OrderService.BusinessLogicLayer.DTOs;

public record OrderResponse(
    Guid OrderID,
    Guid UserID,
    decimal TotalBill,
    DateTime OrderDate,
    List<OrderItemResponse> OrderItems)
{
    public OrderResponse(): this(default, default, default, default, default!)
    {

    }
};