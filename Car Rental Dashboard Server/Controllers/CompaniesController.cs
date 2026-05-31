using Car_Rental_Dashboard_Business_Layer;
using Car_Rental_Dashboard_Data_Access_Layer;
using Car_Rental_Dashboard_Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/Companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetAllCompanies", Name = "GetAllCompanies")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<IEnumerable<companiesDTO>>> GetAllCompanies()
        {
            var companiesList = await companies.GetAllCompaniesAsync();

            if (companiesList == null || companiesList.Count == 0)
            {
                return NotFound("No companies found.");
            }

            return Ok(companiesList);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetCompanyIdByName/{name}", Name = "GetCompanyIdByName")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<int?>> GetCompanyIdByName(string name)
        {
            var companyId = await companies.GetCompanyIdByNameAsync(name);

            if (companyId == null || companyId <= 0)
            {
                return NotFound($"No company found with the name '{name}'.");
            }

            return Ok(companyId);

        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetCompanyNameByID/{id}", Name = "GetCompanyNameByID")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<string?>> GetCompanyNameByID(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Invalid company ID. ID must be a positive integer.");
            }

            var companyName = await companies.GetCompanyNameByIdAsync(id);

            if (companyName == null)
            {
                return NotFound($"No company found with the ID '{id}'.");
            }

            return Ok(companyName);
        }

    }
}


