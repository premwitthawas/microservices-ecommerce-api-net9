namespace Ecommerce.OrderService.BusinessLogicLayer.DTOs;

public record UserDto(
    Guid UserID,
    string? Email,
    string? PersonName,
    string? Gender
)
{
    public UserDto() : this(default, default, default, default) { }
}