using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Helpers;
using OrderProcessingSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Api.Functions
{
    public class DeleteOrderFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<DeleteOrderFunction> _logger;

        public DeleteOrderFunction(IOrderService orderService, ILogger<DeleteOrderFunction> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [Function("DeleteOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "orders/{id:int}")] HttpRequestData req, int id)
        {
            LoggingHelper.LogInfo(_logger, $"DeleteOrder triggered for ID: {id}");

            try
            {
                var deleted = await _orderService.DeleteOrderAsync(id);
                if (!deleted)
                {
                    LoggingHelper.LogWarning(_logger, $"Order with ID {id} not found.");
                    return new NotFoundObjectResult(new { message = "Order not found" });
                }

                LoggingHelper.LogInfo(_logger, $"Order with ID {id} deleted successfully.");
                return new OkObjectResult(new { message = "Order deleted successfully" });
            }
            catch (System.Exception ex)
            {
                LoggingHelper.LogError(_logger, "Error in DeleteOrder", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
