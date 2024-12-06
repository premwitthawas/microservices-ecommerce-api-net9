namespace ECommerce.Users.Core.DTOs;

public record RegisterRequest(string? Email, string? Password, string? PersonName, GenderOptions Gender);