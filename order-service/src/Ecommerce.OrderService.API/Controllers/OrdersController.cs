using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.BusinessLogicLayer.ServiceContacts;
using Ecommerce.OrderService.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ecommerce.OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        [HttpPost]
        public async Task<ActionResult<OrderResponse?>> CreateOrder(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null)
            {
                return BadRequest("Invalid order data");
            }
            OrderResponse? order = await _ordersService.AddOrderAsync(orderAddRequest);
            if (order == null)
            {
                return Problem("Error in adding product");
            }
            return Created($"api/orders/get-by-order-id/{order.OrderID}", order);
        }
        [HttpPut("{orderId::guid}")]
        public async Task<ActionResult<OrderResponse?>> UpdateOrder(Guid orderId, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                return BadRequest("Invalid order data");
            }
            if (orderId != orderUpdateRequest.OrderID)
            {
                return BadRequest("Order ID mismatch");
            }
            OrderResponse? order = await _ordersService.UpdateOrderAsync(orderUpdateRequest);
            if (order == null)
            {
                return Problem("Error in updating product");
            }
            return Ok(order);
        }
        [HttpDelete("{orderId::guid}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest("Invalid order ID");
            }
            bool isDeleted = await _ordersService.DeleteOrderAsync(orderId);
            if (!isDeleted)
            {
                return Problem("Error in deleting product");
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse?>>> GetOrdersList()
        {
            List<OrderResponse?> orders = await _ordersService.GetOrdersAsync();
            return Ok(orders);
        }
        [HttpGet("search/get-by-order-id/{orderId::guid}")]
        public async Task<ActionResult<OrderResponse?>> GetOrderByOrderId(Guid orderId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderID, orderId);
            OrderResponse? order = await _ordersService.GetOrderByCondtitionAsync(filter);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet("search/get-by-product-id/{productId::guid}")]
        public async Task<ActionResult<OrderResponse?>> GetOrderByProductId(Guid productId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(x => x.OrderItems,
                Builders<OrderItem>.Filter.Eq(x => x.ProductID, productId));
            OrderResponse? order = await _ordersService.GetOrderByCondtitionAsync(filter);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet("search/get-by-products-id/{productId::guid}")]
        public async Task<ActionResult<IEnumerable<OrderResponse?>>> GetOrdersByProductId(Guid productId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(x => x.OrderItems,
                Builders<OrderItem>.Filter.Eq(x => x.ProductID, productId));
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondtitionAsync(filter);
            return Ok(orders);
        }
        [HttpGet("search/get-by-date/{date::datetime}")]
        public async Task<ActionResult<IEnumerable<OrderResponse?>>> GetOrdersByDate(DateTime date)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderDate.ToString("yyyy-MM-dd"),
                date.ToString("yyy-MM-dd"));
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondtitionAsync(filter);
            return Ok(orders);
        }
        [HttpGet("search/get-by-user-id/{userId::guid}")]
        public async Task<ActionResult<IEnumerable<OrderResponse?>>> GetOrdersByUserId(Guid userId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.UserID, userId);
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondtitionAsync(filter);
            return Ok(orders);
        }
    }
}