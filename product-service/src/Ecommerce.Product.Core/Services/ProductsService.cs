using System.Linq.Expressions;
using AutoMapper;
using Ecommerce.Product.Core.DTOs;
using Ecommerce.Product.Core.RabbitMQ;
using Ecommerce.Product.Core.ServiceContacts;
using Ecommerce.Product.Infrastructure.Entities;
using Ecommerce.Product.Infrastructure.RepositoryContacts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Ecommerce.Product.Core.Services;

public class ProductsService : IProductsService
{
    private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
    private readonly IValidator<UpdateProductRequest> _updateProductRequestValidator;
    private readonly IMapper _mapper;
    private readonly IProductsRepository _productsRepository;
    private readonly ILogger<ProductsService> _logger;
    private readonly IRabbitMQPublisher _rabbitMQPublisher;
    public ProductsService(IMapper mapper, IProductsRepository productsRepository, IValidator<CreateProductRequest> createProductRequestValidator, IValidator<UpdateProductRequest> updateProductRequestValidator, ILogger<ProductsService> logger, IRabbitMQPublisher rabbitMQPublisher)
    {
        _mapper = mapper;
        _productsRepository = productsRepository;
        _createProductRequestValidator = createProductRequestValidator;
        _updateProductRequestValidator = updateProductRequestValidator;
        _logger = logger;
        _rabbitMQPublisher = rabbitMQPublisher;
    }
    public async Task<ProductsResponse?> AddProductAsync(CreateProductRequest createProductRequest)
    {
        if (createProductRequest == null)
        {
            throw new ArgumentNullException(nameof(createProductRequest));
        }
        ValidationResult validationResult = await _createProductRequestValidator.ValidateAsync(createProductRequest);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }
        Products product = _mapper.Map<Products>(createProductRequest);
        Products? proudctNew = await _productsRepository.AddProductAsync(product);
        if (proudctNew == null)
        {
            return null;
        }
        ProductsResponse productsResponse = _mapper.Map<ProductsResponse>(proudctNew);
        return productsResponse;
    }

    public async Task<bool> DeleteProductASync(Guid productId)
    {
        Products? existingProduct = await _productsRepository.GetProductByConditionAsync(x => x.ProductID == productId);
        if (existingProduct == null)
        {
            return false;
        }
        ProductDeletionMessage msg = new(existingProduct.ProductID, existingProduct.ProductName);
        _logger.LogInformation($"Product found {existingProduct.ProductID}");
        // bool productIsDeleted = true;
        bool productIsDeleted = await _productsRepository.DeleteProductAsync(productId);
        if (productIsDeleted)
        {
            string routeKey = "product.delete";
            var header = new Dictionary<string, object>
            {
                 {
                    "x-match","all"
                },
                {
                    "event",routeKey
                },
                {
                    "rowCount",1
                }
            };
            _logger.LogInformation($"Msg {msg}");
            _rabbitMQPublisher.Publish(header, msg);
        }
        return productIsDeleted;
    }

    public async Task<ProductsResponse?> GetProductByConditionAsync(Expression<Func<Products, bool>> conditionExpression)
    {
        Products? existingProduct = await _productsRepository.GetProductByConditionAsync(conditionExpression);
        if (existingProduct == null)
        {
            return null;
        }
        ProductsResponse productsResponse = _mapper.Map<ProductsResponse>(existingProduct);
        return productsResponse;
    }

    public async Task<List<ProductsResponse?>> GetProductsAsync()
    {
        IEnumerable<Products> products = await _productsRepository.GetProductsAsync();
        IEnumerable<ProductsResponse?> responseProducts = _mapper.Map<IEnumerable<ProductsResponse>>(products);
        return responseProducts.ToList();
    }

    public async Task<List<ProductsResponse?>> GetProductsByConditionAsync(Expression<Func<Products, bool>> conditonExpression)
    {
        IEnumerable<Products?> products = await _productsRepository.GetProductsByConditionAsync(conditonExpression);
        IEnumerable<ProductsResponse?> responseProducts = _mapper.Map<IEnumerable<ProductsResponse>>(products);
        return responseProducts.ToList();
    }

    public async Task<ProductsResponse?> UpdateProductASync(UpdateProductRequest updateProductRequest)
    {
        if (updateProductRequest == null)
        {
            throw new ArgumentNullException(nameof(updateProductRequest));
        }
        ValidationResult validationResult = await _updateProductRequestValidator.ValidateAsync(updateProductRequest);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }
        Products? existingProduct = await _productsRepository.GetProductByConditionAsync(x => x.ProductID == updateProductRequest.ProductID);

        if (existingProduct == null)
        {
            throw new ArgumentException("Product not found");
        }

        Products products = _mapper.Map<Products>(updateProductRequest);
        bool isProductNameChanged = existingProduct.ProductName != products.ProductName;

        // Publish message to RabbitMQ if product name is changed
        if (isProductNameChanged)
        {
            // _logger.LogInformation(products.ProductID.ToString());
            string routeKey = "product.update.name";
            // var message = new ProductNameUpdateMessage(products.ProductID, products.ProductName);
            var header = new Dictionary<string, object>
            {
                {
                    "x-match","any"
                },
                {
                    "event",routeKey
                },
                {
                    "field","name"
                },
                {
                    "rowCount",1
                }
            };
            _rabbitMQPublisher.Publish(header, products);
        }
        _logger.LogInformation($"Product found {products.ProductName}");
        Products? updatedProduct = await _productsRepository.UpdateProductAsync(products);
        if (updatedProduct == null)
        {
            return null;
        }
        ProductsResponse productsResponse = _mapper.Map<ProductsResponse>(updatedProduct);
        return productsResponse;
    }
};