using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Helpers;
using OrderProcessingSystem.Services.Dtos;
using OrderProcessingSystem.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;
using FluentValidation;

namespace OrderProcessingSystem.Api.Functions
{
    public class UpdateOrderFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<UpdateOrderFunction> _logger;
        private readonly IValidator<OrderUpdateDto> _validator;

        public UpdateOrderFunction(IOrderService orderService, ILogger<UpdateOrderFunction> logger, IValidator<OrderUpdateDto> validator)
        {
            _orderService = orderService;
            _logger = logger;
            _validator = validator;
        }

        [Function("UpdateOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "orders/{id:int}")] HttpRequestData req, int id)
        {
            LoggingHelper.LogInfo(_logger, $"UpdateOrder triggered for ID: {id}");

            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var dto = JsonConvert.DeserializeObject<OrderUpdateDto>(body);

                // Validate DTO
                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    LoggingHelper.LogWarning(_logger, $"Validation failed for UpdateOrder ID: {id}");
                    return new BadRequestObjectResult(validationResult.Errors);
                }

                var updatedOrder = await _orderService.UpdateOrderAsync(id, dto);

                if (updatedOrder == null)
                {
                    LoggingHelper.LogWarning(_logger, $"Order with ID {id} not found.");
                    return new NotFoundObjectResult(new { message = "Order not found" });
                }

                LoggingHelper.LogInfo(_logger, $"Order with ID {id} updated successfully.");
                return new OkObjectResult(new { message = "Order updated successfully", data = updatedOrder });
            }
            catch (System.Exception ex)
            {
                LoggingHelper.LogError(_logger, "Error in UpdateOrder", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
