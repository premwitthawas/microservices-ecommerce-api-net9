using System.Linq.Expressions;
using Ecommerce.Product.Infrastructure.Entities;

namespace Ecommerce.Product.Infrastructure.RepositoryContacts
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Products>> GetProductsAsync();
        Task<IEnumerable<Products?>> GetProductsByConditionAsync(Expression<Func<Products, bool>> condition);
        Task<Products?> GetProductByConditionAsync(Expression<Func<Products, bool>> condition);
        Task<Products?> AddProductAsync(Products product);
        Task<Products?> UpdateProductAsync(Products product);
        Task<bool> DeleteProductAsync(Guid productID);
    }
}