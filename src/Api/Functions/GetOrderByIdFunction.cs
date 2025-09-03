using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Helpers;
using OrderProcessingSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Api.Functions
{
    public class GetOrderByIdFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetOrderByIdFunction> _logger;

        public GetOrderByIdFunction(IOrderService orderService, ILogger<GetOrderByIdFunction> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [Function("GetOrderById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id:int}")] HttpRequestData req, int id)
        {
            LoggingHelper.LogInfo(_logger, $"GetOrderById triggered for ID: {id}");

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    LoggingHelper.LogWarning(_logger, $"Order with ID {id} not found.");
                    return new NotFoundObjectResult(new { message = "Order not found" });
                }

                LoggingHelper.LogInfo(_logger, $"Order with ID {id} retrieved successfully.");
                return new OkObjectResult(order);
            }
            catch (System.Exception ex)
            {
                LoggingHelper.LogError(_logger, "Error in GetOrderById", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
