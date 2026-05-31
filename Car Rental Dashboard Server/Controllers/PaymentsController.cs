using Car_Rental_Dashboard_Business_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using System.Net.NetworkInformation;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/Payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetRevenueForThisMonthInSpecificStatus", Name = "GetRevenueForThisMonthInSpecificStatus")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<double>> GetRevenueForThisMonthInSpecificStatus(string status)
        {
            var RevenueThisMonthInSpecificStatus = await payments.GetRevenueForThisMonthInSpecificStatusAsync(status);

            if (RevenueThisMonthInSpecificStatus == 0)
            {
                return NotFound("No Revenue For This Month");
            }

            if (RevenueThisMonthInSpecificStatus < 0)
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Revenue For {status} transactions This Month" });
            }

            return Ok(RevenueThisMonthInSpecificStatus);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("NumberOfTransactionsInMonthForSpecificStatus", Name = "NumberOfTransactionsInMonthForSpecificStatus")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> NumberOfTransactionsInMonthForSpecificStatus(string status)
        {
            var transactions = await payments.NumberOfTransactionsInMonthForSpecificStatusAsync(status);

            if (transactions == 0)
            {
                return NotFound($"No {status} Transactions This Month");
            }

            if (transactions < 0)
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Number Of {status} transactions This Month" });
            }

            return Ok(transactions);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetRevenueChangePercentageRate", Name = "GetRevenueChangePercentageRate")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> GetRevenueChangePercentageRate()
        {
            var RevenueChangePercentageRate = await payments.GetRevenueChangePercentageRateAsync();

            if (string.IsNullOrWhiteSpace( RevenueChangePercentageRate ))
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Revenue Rate Change" });
            }

            return Ok(RevenueChangePercentageRate);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetRefundRateChange", Name = "GetRefundRateChange")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> GetRefundRateChange()
        {
            var RefundRateChange = await payments.GetRefundRateChangeAsync();

            if (string.IsNullOrWhiteSpace(RefundRateChange))
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Refund Rate Change" });
            }

            return Ok(RefundRateChange);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetTheTotalOfRevenueThisYear", Name = "GetTheTotalOfRevenueThisYear")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<double>> GetTheTotalOfRevenueThisYear()
        {
            var TotalOfRevenueThisYear = await payments.GetTheTotalOfRevenueThisYearAsync();

            if (TotalOfRevenueThisYear == 0)
            {
                return NotFound("No Revenue For This Year");
            }

            if (TotalOfRevenueThisYear < 0)
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Total Revenue For This Year" });
            }

            return Ok(TotalOfRevenueThisYear);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetTheAverageOfRevenueThisYear", Name = "GetTheAverageOfRevenueThisYear")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<double>> GetTheAverageOfRevenueThisYear()
        {
            var AverageOfRevenueThisYear = await payments.GetTheAverageOfRevenueThisYearAsync();

            if (AverageOfRevenueThisYear == 0)
            {
                return NotFound("No Revenue For This Year");
            }

            if (AverageOfRevenueThisYear < 0)
            {
                return StatusCode(500, new { Message = $"Error While Calculating The Average Revenue For This Year" });
            }

            return Ok(AverageOfRevenueThisYear);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetTheGrowthRate", Name = "GetTheGrowthRate")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<double>> GetTheGrowthRate()
        {
            var GrowthRate = await payments.GetTheGrowthRateAsync();

            return Ok(GrowthRate);
        }


    }
}
