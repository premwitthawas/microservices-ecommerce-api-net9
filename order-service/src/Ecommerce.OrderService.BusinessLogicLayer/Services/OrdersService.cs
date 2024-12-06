using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
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

    public OrdersService(IMapper mapper, IOrdersRepository ordersRepository, IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
        _orderAddRequestValidator = orderAddRequestValidator;
        _orderItemAddRequestValidator = orderItemAddRequestValidator;
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
            //TODO: Check if user and product
            //Mapping Data
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

    public Task<bool> DeleteOrderAsync(Guid orderID)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse?> GetOrderByCondtitionAsync(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderResponse?>> GetOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderResponse?>> GetOrdersByCondtitionAsync(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse?> UpdateOrderAsync(OrderUpdateRequest orderUpdateRequest)
    {
        throw new NotImplementedException();
    }
};