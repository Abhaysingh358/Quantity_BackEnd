using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Repositories.Interfaces;

namespace QuantityMeasurementApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // requires valid JWT — frontend must send Bearer token
    public class HistoryController : ControllerBase
    {
        private readonly IQuantityMeasurementRepository _repo;

        public HistoryController(IQuantityMeasurementRepository repo)
        {
            _repo = repo;
        }

        // GET api/history
        // Returns the logged-in user's history, newest first
        [HttpGet]
        public IActionResult GetHistory()
        {
            int userId  = GetUserId();

            var history = _repo.GetByUserId(userId)
                               .Select(e => new
                               {
                                   Operation = e.Operation,
                                   Operand1  = $"{e.Operand1?.Value} {e.Operand1?.Unit}",
                                   Operand2  = e.Operand2 != null
                                                   ? $"{e.Operand2.Value} {e.Operand2.Unit}"
                                                   : null,
                                   Result    = e.IsError
                                                   ? $"Error: {e.ErrorMessage}"
                                                   : e.Operation switch
                                                   {
                                                       "Compare" => e.ResultComparison ? "Equal" : "Not Equal",
                                                       "Divide"  => $"{e.ResultScalar:G}",
                                                       _         => $"{e.ResultQuantity?.Value} {e.ResultQuantity?.Unit}"
                                                   },
                                   IsError   = e.IsError
                               })
                               .ToList();

            return Ok(history);
        }

        // Reads userId from the JWT claim set during login
        // ClaimTypes.NameIdentifier = user.Id.ToString() — set in AuthService.GenerateJwtToken
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                throw new UnauthorizedAccessException("Invalid token");
            return userId;
        }
    }
}