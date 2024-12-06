using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Product.Infrastructure.Entities;

public class Products
{
    [Key]
    public Guid ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double? UnitPrice { get; set; }
    public  int? QuantityInStock { get; set; }
}
