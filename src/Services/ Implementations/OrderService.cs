using OrderProcessingSystem.Models;
using OrderProcessingSystem.Services.Dtos;
using OrderProcessingSystem.Services.Interfaces;
using OrderProcessingSystem.Services.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            var order = new Order
            {
                ProductName = orderDto.ProductName,
                Quantity = orderDto.Quantity,
                Price = orderDto.Price
            };

            await _orderRepository.AddOrderAsync(order);

            return new OrderReadDto
            {
                Id = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                Price = order.Price
            };
        }

        public async Task<OrderReadDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order == null ? null : new OrderReadDto
            {
                Id = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                Price = order.Price
            };
        }

        public async Task<PagedResultDto<OrderReadDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                orders = orders.Where(o => o.ProductName.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase));
            }

            // Total count before pagination
            var totalCount = orders.Count();

            // Pagination
            var pagedOrders = orders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderReadDto
                {
                    Id = o.Id,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    Price = o.Price
                })
                .ToList();

            return new PagedResultDto<OrderReadDto>
            {
                Items = pagedOrders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OrderReadDto> UpdateOrderAsync(int id, OrderUpdateDto orderDto)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return null;

            order.ProductName = orderDto.ProductName;
            order.Quantity = orderDto.Quantity;
            order.Price = orderDto.Price;

            await _orderRepository.UpdateOrderAsync(order);

            return new OrderReadDto
            {
                Id = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                Price = order.Price
            };
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return false;

            await _orderRepository.DeleteOrderAsync(id);
            return true;
        }
    }
}
