using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Helpers;
using OrderProcessingSystem.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OrderProcessingSystem.Api.Functions
{
    public class GetAllOrdersFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<GetAllOrdersFunction> _logger;

        public GetAllOrdersFunction(IOrderService orderService, ILogger<GetAllOrdersFunction> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [Function("GetAllOrders")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequestData req)
        {
            LoggingHelper.LogInfo(_logger, "GetAllOrders triggered.");

            try
            {
                var query = HttpUtility.ParseQueryString(req.Url.Query);
                int pageNumber = int.TryParse(query["pageNumber"], out var pn) ? pn : 1;
                int pageSize = int.TryParse(query["pageSize"], out var ps) ? ps : 10;
                string searchTerm = query["search"];

                var result = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, searchTerm);

                LoggingHelper.LogInfo(_logger, $"Retrieved {result.Items?.Count()} orders (Page {pageNumber}).");
                return new OkObjectResult(result);
            }
            catch (System.Exception ex)
            {
                LoggingHelper.LogError(_logger, "Error in GetAllOrders", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
