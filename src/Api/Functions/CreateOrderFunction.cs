using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderProcessingSystem.Services.Dtos;
using OrderProcessingSystem.Services.Interfaces;
using OrderProcessingSystem.Helpers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace OrderProcessingSystem.Api.Functions
{
    public class CreateOrderFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<CreateOrderFunction> _logger;
        private readonly IValidator<OrderCreateDto> _validator;

        public CreateOrderFunction(IOrderService orderService, ILogger<CreateOrderFunction> logger, IValidator<OrderCreateDto> validator)
        {
            _orderService = orderService;
            _logger = logger;
            _validator = validator;
        }

        [Function("CreateOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req)
        {
            LoggingHelper.LogInfo(_logger, "CreateOrder triggered.");

            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var dto = JsonConvert.DeserializeObject<OrderCreateDto>(body);

                // Validate DTO
                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    LoggingHelper.LogWarning(_logger, "Validation failed for CreateOrder.");
                    return new BadRequestObjectResult(validationResult.Errors);
                }

                var order = await _orderService.CreateOrderAsync(dto);

                LoggingHelper.LogInfo(_logger, $"Order created successfully: {order.Id}");
                return new OkObjectResult(new { message = "Order created successfully", data = order });
            }
            catch (System.Exception ex)
            {
                LoggingHelper.LogError(_logger, "Error in CreateOrder", ex);
                return new StatusCodeResult(500);
            }
        }
    }
}
