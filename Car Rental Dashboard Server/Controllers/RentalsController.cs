using Car_Rental_Dashboard_Business_Layer;
using Car_Rental_Dashboard_Data_Access_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/Rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetTop5ActiveRentals")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<rentalsDTO>>> GetTop5ActiveRentals()
        {
            var activeRentals = await rentals.GetTop5ActiveRentalsAsync();

            if(activeRentals.Count == 0)
            {
                return NotFound("Active Rentals Not Found");
            }

            return Ok(activeRentals);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("NumberOfActiveRentals", Name = "NumberOfActiveRentals")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> NumberOfActiveRentals()
        {
            var NumberOfActiveRentals = await rentals.NumberOfActiveRentalsAsync();

            if(NumberOfActiveRentals == 0)
            {
                return NotFound("No Active Rentals");
            }

            if (NumberOfActiveRentals < 0)
            {
                return StatusCode(500, new { Message = "Error While Counting The Number Of Active Rentals" });
            }

            return Ok(NumberOfActiveRentals);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("NumberOfActiveRentalsLastWeek", Name = "NumberOfActiveRentalsLastWeek")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> NumberOfActiveRentalsLastWeek()
        {
            var NumberOfActiveRentalsLastWeek = await rentals.NumberOfActiveRentalsLastWeekAsync();

            if (NumberOfActiveRentalsLastWeek == 0)
            {
                return NotFound("No Active Rentals Last Week");
            }

            if (NumberOfActiveRentalsLastWeek < 0)
            {
                return StatusCode(500, new { Message = "Error While Counting The Number Of Active Rentals Last Week" });
            }

            return Ok(NumberOfActiveRentalsLastWeek);
        }

    }
}
