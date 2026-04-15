using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuantityMeasurementApp.Models.DTO;
using QuantityMeasurementApp.Business.Interfaces;

namespace QuantityMeasurementApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementService _service;
        private readonly ILogger<QuantityMeasurementController> _logger;

        public QuantityMeasurementController(
            IQuantityMeasurementService service,
            ILogger<QuantityMeasurementController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // Compare
        [HttpPost("compare")]
        public IActionResult Compare([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("Compare API called with input: {@input}", input);

            if (input?.First == null || input?.Second == null)
            {
                _logger.LogWarning("Invalid input for Compare: {@input}", input);
                return BadRequest("Input quantities cannot be null.");
            }

            var result = _service.Compare(input.First, input.Second, GetUserId());

            return Ok(result);
        }

        // Convert
        [HttpPost("convert")]
        public IActionResult Convert([FromBody] ConvertRequestDTO request)
        {
            _logger.LogInformation("Convert API called with request: {@request}", request);

            if (request == null || request.Input == null)
            {
                _logger.LogWarning("Invalid convert request: {@request}", request);
                return BadRequest("Invalid request.");
            }

            var result = _service.Convert(request.Input, request.TargetUnit, GetUserId());

            return Ok(result);
        }

        // Add
        [HttpPost("add")]
        public IActionResult Add([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("Add API called with input: {@input}", input);

            if (input?.First == null || input?.Second == null)
            {
                _logger.LogWarning("Invalid input for Add: {@input}", input);
                return BadRequest("Invalid input.");
            }

            var result = _service.Add(input.First, input.Second, GetUserId());

            return Ok(result);
        }

        // Subtract
        [HttpPost("subtract")]
        public IActionResult Subtract([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("Subtract API called with input: {@input}", input);

            if (input?.First == null || input?.Second == null)
            {
                _logger.LogWarning("Invalid input for Subtract: {@input}", input);
                return BadRequest("Invalid input.");
            }

            var result = _service.Subtract(input.First, input.Second, GetUserId());

            return Ok(result);
        }

        // Divide
        [HttpPost("divide")]
        public IActionResult Divide([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("Divide API called with input: {@input}", input);

            if (input?.First == null || input?.Second == null)
            {
                _logger.LogWarning("Invalid input for Divide: {@input}", input);
                return BadRequest("Invalid input.");
            }

            var result = _service.Divide(input.First, input.Second, GetUserId());

            return Ok(result);
        }

        // reads userId from JWT — set by AuthService.GenerateJwtToken as ClaimTypes.NameIdentifier
        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out int id))
                return id;
            return null;
        }
    }
}