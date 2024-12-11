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
    public OrdersService(IMapper mapper, IOrdersRepository ordersRepository, IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator, UsersMicroserviceClient usersMicroserviceClient)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
        _orderAddRequestValidator = orderAddRequestValidator;
        _orderItemAddRequestValidator = orderItemAddRequestValidator;
        _orderUpdateRequestValidator = orderUpdateRequestValidator;
        _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
        _usersMicroserviceClient = usersMicroserviceClient;
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
        foreach (OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
        {
            ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);
            if (!orderItemAddRequestValidationResult.IsValid)
            {
                string erros = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(erros);
            }
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
        return orderResponse;
    }

    public async Task<List<OrderResponse?>> GetOrdersAsync()
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrdersAsync();
        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
        return orderResponses.ToList();
    }

    public async Task<List<OrderResponse?>> GetOrdersByCondtitionAsync(FilterDefinition<Order> filter)
    {
        IEnumerable<Order?> orders = await _ordersRepository.GetOrdersByCondition(filter);
        IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
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
        foreach (OrderItemUpdateRequest orderItem in orderUpdateRequest.OrderItems)
        {
            ValidationResult validateUpdateOrderItemRequest = await _orderItemUpdateRequestValidator.ValidateAsync(orderItem);
            if (!validateUpdateOrderItemRequest.IsValid)
            {
                string errors = String.Join(", ", validateUpdateOrderItemRequest.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(errors);
            }
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
        return orderResponse;
    }
};