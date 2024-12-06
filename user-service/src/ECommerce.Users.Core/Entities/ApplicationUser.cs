namespace ECommerce.Users.Core.Entities;
/// <summary>
/// ApplicationUser entity class
/// </summary>
public class AppplicaitonUser {
    public Guid UserID { get; set; }
    public string? Email  {get; set;}
    public string? Password { get; set; }
    public string? PersonName { get; set; }
    public string? Gender { get; set; }
};