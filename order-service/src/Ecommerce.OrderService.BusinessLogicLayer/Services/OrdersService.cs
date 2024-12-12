using AutoMapper;
using AutoMapper.Internal.Mappers;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.BusinessLogicLayer.HttpClients;
using Ecommerce.OrderService.BusinessLogicLayer.ServiceContacts;
using Ecommerce.OrderService.DataLayer.Models;
using Ecommerce.OrderService.DataLayer.RepositoryContacts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;

namespace Ecommerce.OrderService.BusinessLogicLayer.Services;

public class OrdersService : IOrdersService
{
    private IMapper _mapper;
    private IOrdersRepository _ordersRepository;
    private IValidator<OrderAddRequest> _orderAddRequestValidator;
    private IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
    private IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
    private IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
    private readonly UsersMicroserviceClient _usersMicroserviceClient;
    private readonly ProductMicroserviceClient _productMicroserviceClient;
    public OrdersService(IMapper mapper, IOrdersRepository ordersRepository, IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator, UsersMicroserviceClient usersMicroserviceClient, ProductMicroserviceClient productMicroserviceClient)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
        _orderAddRequestValidator = orderAddRequestValidator;
        _orderItemAddRequestValidator = orderItemAddRequestValidator;
        _orderUpdateRequestValidator = orderUpdateRequestValidator;
        _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
        _usersMicroserviceClient = usersMicroserviceClient;
        _productMicroserviceClient = productMicroserviceClient;
    }

    public async Task<OrderResponse?> AddOrderAsync(OrderAddRequest orderAddRequest)
    {
        if (orderAddRequest == null)
        {
            throw new ArgumentNullException(nameof(orderAddRequest));
        }
        ValidationResult orderAddRequestValidationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);
        if (!orderAddRequestValidationResult.IsValid)
        {
            string erros = string.Join(", ", orderAddRequestValidationResult.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(erros);
        }

        List<ProductDto?> productsDto = [];

        foreach (OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
        {
            ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);
            if (!orderItemAddRequestValidationResult.IsValid)
            {
                string erros = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(erros);
            }
            ProductDto? productDto = await _productMicroserviceClient.GetProductByIdAsync(orderItemAddRequest.ProductID);
            if (productDto == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }
            productsDto.Add(productDto);
        }
        //TODO: Check if user and product
        //Mapping Data
        UserDto? user = await _usersMicroserviceClient.GetUserByUserID(orderAddRequest.UserID);
        if (user == null)
        {
            throw new ArgumentException("Invalid User ID");
        }
        Order orderInput = _mapper.Map<Order>(orderAddRequest);

        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        orderInput.TotalBill = orderInput.OrderItems.Sum(x => x.TotalPrice);

        Order? addedOrder = await _ordersRepository.AddOrderAsync(orderInput);
        if (addedOrder == null)
        {
            return null;
        }
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(addedOrder);
        if (orderResponse != null)
        {
            foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
            {
                ProductDto? productDto = productsDto.Where(tmp => tmp?.ProductID == orderItemResponse.ProductID).FirstOrDefault();
                if (productDto == null) continue;
                _mapper.Map<ProductDto, OrderItemResponse>(productDto, orderItemResponse);
            }
        }
        return orderResponse;
    }

    public async Task<bool> DeleteOrderAsync(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderID, orderID);
        Order? orderExisting = await _ordersRepository.GetOrderByCondition(filter);
        if (orderExisting == null)
        {
            return false;
        }
        bool isDeleted = await _ordersRepository.DelelteOrderAsync(orderID);
        return isDeleted;
    }

    public async Task<OrderResponse?> GetOrderByCondtitionAsync(FilterDefinition<Order> filter)
    {
        Order? order = await _ordersRepository.GetOrderByCondition(filter);
        if (order == null)
        {
            return null;
        }
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);
        if(orderResponse != null){
            foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
            {
                ProductDto? productDto = await _productMicroserviceClient.GetProductByIdAsync(orderItemResponse.ProductID);
                if (productDto == null)
                {
                    continue;
                }
                _mapper.Map<ProductDto, OrderItemResponse>(productDto, orderItemResponse);
            }
            UserDto? user = await _usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }
            _mapper.Map<UserDto, OrderResponse>(user, orderResponse);
        }
        return orderResponse;
    }

    public async Task<List<OrderResponse?>> GetOrdersAsync()
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrdersAsync();
        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
        foreach (OrderResponse? orderResponse in orderResponses)
        {
            if (orderResponse == null)
            {
                continue;
            }
            foreach (OrderItemResponse? orderItemResponse in orderResponse.OrderItems)
            {
                if (orderItemResponse != null)
                {
                    ProductDto? productDto = await _productMicroserviceClient.GetProductByIdAsync(orderItemResponse.ProductID);
                    if (productDto == null)
                    {
                        continue;
                    }
                    _mapper.Map<ProductDto, OrderItemResponse>(productDto, orderItemResponse);
                }
            }
            UserDto? user = await _usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
            if (user != null)
            {
                _mapper.Map<UserDto, OrderResponse>(user, orderResponse);
            }
        }
        return orderResponses.ToList();
    }

    public async Task<List<OrderResponse?>> GetOrdersByCondtitionAsync(FilterDefinition<Order> filter)
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrdersByCondition(filter);
        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
        foreach (OrderResponse? orderResponse in orderResponses)
        {
            if (orderResponse == null)
            {
                continue;
            }
            foreach (OrderItemResponse? orderItemResponse in orderResponse.OrderItems)
            {
                if (orderItemResponse != null)
                {
                    ProductDto? productDto = await _productMicroserviceClient.GetProductByIdAsync(orderItemResponse.ProductID);
                    if (productDto == null)
                    {
                        continue;
                    }
                    _mapper.Map<ProductDto, OrderItemResponse>(productDto, orderItemResponse);
                }
            }
            UserDto? user = await _usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
            if (user != null)
            {
                _mapper.Map<UserDto, OrderResponse>(user, orderResponse);
            }
        }
        return orderResponses.ToList();
    }

    public async Task<OrderResponse?> UpdateOrderAsync(OrderUpdateRequest orderUpdateRequest)
    {
        if (orderUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(orderUpdateRequest));
        }
        ValidationResult validateUpdateOrderRequest = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
        if (!validateUpdateOrderRequest.IsValid)
        {
            string erros = String.Join(", ", validateUpdateOrderRequest.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(erros);
        }

        List<ProductDto?> productsDto = [];
        foreach (OrderItemUpdateRequest orderItem in orderUpdateRequest.OrderItems)
        {
            ValidationResult validateUpdateOrderItemRequest = await _orderItemUpdateRequestValidator.ValidateAsync(orderItem);
            if (!validateUpdateOrderItemRequest.IsValid)
            {
                string errors = String.Join(", ", validateUpdateOrderItemRequest.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(errors);
            }
            ProductDto? productDto = await _productMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
            if (productDto == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }
            productsDto.Add(productDto);
        }
        //TODO: Check if user and product
        UserDto? user = await _usersMicroserviceClient.GetUserByUserID(orderUpdateRequest.UserID);
        if (user == null)
        {
            throw new ArgumentException("Invalid User ID");
        }
        Order order = _mapper.Map<Order>(orderUpdateRequest);
        foreach (OrderItem orderItem in order.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(x => x.TotalPrice);
        Order? updatedOrder = await _ordersRepository.UpdateOrderAsync(order);
        if (updatedOrder == null)
        {
            return null;
        }
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(updatedOrder);
        if (orderResponse != null)
        {
            foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
            {
                ProductDto? productDto = productsDto.Where(tmp => tmp?.ProductID == orderItemResponse.ProductID).FirstOrDefault();
                if (productDto == null) continue;
                _mapper.Map<ProductDto, OrderItemResponse>(productDto, orderItemResponse);
            }
            UserDto? userDto = await _usersMicroserviceClient.GetUserByUserID(orderResponse.UserID);
            if (userDto != null)
            {
                _mapper.Map<UserDto, OrderResponse>(userDto, orderResponse);
            }
        }
        return orderResponse;
    }
};