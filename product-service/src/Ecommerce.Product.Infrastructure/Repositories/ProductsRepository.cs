using System.Linq.Expressions;
using Ecommerce.Product.Infrastructure.Context;
using Ecommerce.Product.Infrastructure.Entities;
using Ecommerce.Product.Infrastructure.RepositoryContacts;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Product.Infrastructure.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ProductsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Products?> AddProductAsync(Products product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(Guid productID)
    {
        Products? productExsiting = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == productID);
        if (productExsiting == null)
        {
            return false;
        }
        _dbContext.Products.Remove(productExsiting);
        int rowAffectedCount = await _dbContext.SaveChangesAsync();
        return rowAffectedCount > 0;
    }

    public async Task<Products?> GetProductByConditionAsync(Expression<Func<Products, bool>> condition)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(condition);
    }

    public async Task<IEnumerable<Products>> GetProductsAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Products?>> GetProductsByConditionAsync(Expression<Func<Products, bool>> condition)
    {
        return await _dbContext.Products.Where(condition).ToListAsync();
    }

    public async Task<Products?> UpdateProductAsync(Products product)
    {
        Products? productsExsiting = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == product.ProductID);
        if (productsExsiting == null)
        {
            return null;
        }
        productsExsiting.ProductName = product.ProductName ?? productsExsiting.ProductName;
        productsExsiting.Category = product.Category ?? productsExsiting.Category;
        productsExsiting.UnitPrice = product.UnitPrice ?? productsExsiting.UnitPrice;
        productsExsiting.QuantityInStock = product.QuantityInStock ?? productsExsiting.QuantityInStock;
        await _dbContext.SaveChangesAsync();
        return productsExsiting;
    }
}