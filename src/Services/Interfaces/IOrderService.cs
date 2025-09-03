using OrderProcessingSystem.Services.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto);
        Task<OrderReadDto> GetOrderByIdAsync(int id);
        Task<PagedResultDto<OrderReadDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string searchTerm);
        Task<OrderReadDto> UpdateOrderAsync(int id, OrderUpdateDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
