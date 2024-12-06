using System.Linq.Expressions;
using Ecommerce.Product.Core.DTOs;
using Ecommerce.Product.Infrastructure.Entities;
namespace Ecommerce.Product.Core.ServiceContacts;

public interface IProductsService
{
    Task<List<ProductsResponse?>> GetProductsAsync();
    Task<List<ProductsResponse?>> GetProductsByConditionAsync(Expression<Func<Products, bool>> conditonExpression);
    Task<ProductsResponse?> GetProductByConditionAsync(Expression<Func<Products, bool>> conditionExpression);
    Task<ProductsResponse?> AddProductAsync(CreateProductRequest createProductRequest);
    Task<ProductsResponse?> UpdateProductASync(UpdateProductRequest updateProductRequest);
    Task<bool> DeleteProductASync(Guid productId);
};