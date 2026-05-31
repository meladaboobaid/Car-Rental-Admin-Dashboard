using Car_Rental_Dashboard_Business_Layer;
using Car_Rental_Dashboard_Data_Access_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.RateLimiting;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/CarTypes")]
    [ApiController]
    public class CarTypesController : ControllerBase
    {

        [AllowAnonymous]
        [HttpGet("AllCarTypes", Name = "GetAllCarTypes")]
        [EnableRateLimiting("GlobalAnonymousLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<car_typesDTO>>> GetAllCarTypes()
        {
            var carTypes = await car_types.GetAllCarTypesAsync();

            if (carTypes.Count == 0)
            {
                return NotFound("Car types not found");
            }

            return Ok(carTypes);
        }

        [AllowAnonymous]
        [HttpGet("Category", Name = "CarsCategory")]
        [EnableRateLimiting("GlobalAnonymousLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<car_typesDTO>>> GetCarsCategory()
        {
            var carsCategory = await car_types.GetCarsCategoryAsync();

            if (carsCategory.Count == 0)
            {
                return NotFound("Cars Category Not Found");
            }

            return Ok(carsCategory);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("{id}", Name = "GetCarTypeByID")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<car_typesDTO>> GetCarTypeByID(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID {id}");
            }

            var car_type = await car_types.FindAsync(id);

            if (car_type == null)
            {
                return NotFound("Car Type Not Found");
            }

            car_typesDTO CDTO = car_type.car_typeDTO;

            return Ok(CDTO);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("ByName{name}", Name = "GetCarTypeIDByName")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> GetCarTypeByID(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest($"Not accepted name");
            }

            var id = await car_types.GetCarTypeIdByNameAsync(name);

            if (id == null || id <= 0)
            {
                return NotFound("Car Type Not Found");
            }

            return Ok(id);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpPost(Name = "AddCarType")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<car_typesDTO>> AddNewCarType(car_typesDTO carTypeDTO)
        {
            if (carTypeDTO == null || string.IsNullOrWhiteSpace(carTypeDTO.name)
                || string.IsNullOrWhiteSpace(carTypeDTO.description) 
                || carTypeDTO.typical_seats <= 0
                || carTypeDTO.typical_daily_rate <= 0)
            {
                return BadRequest("Invalid car type data");
            }

            car_types carType = new car_types(carTypeDTO);

            if(await carType.SaveAsync())
            {
                carTypeDTO.id = carType.id;

                return CreatedAtRoute("GetCarTypeByID", new { id = carTypeDTO.id }, carTypeDTO);
            }
            else
            {
                return StatusCode(500, new { Message = "Error Adding Car Type" });
            }
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpPut("{id}" ,Name = "UpdateCarType")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<car_typesDTO>> UpdateCarType(int id, car_typesDTO carTypeDTO)
        {
            if (carTypeDTO == null || string.IsNullOrWhiteSpace(carTypeDTO.name)
               || string.IsNullOrWhiteSpace(carTypeDTO.description)
               || carTypeDTO.typical_seats <= 0
               || carTypeDTO.typical_daily_rate <= 0)
            {
                return BadRequest("Invalid car type data");
            }

            car_types carType = await car_types.FindAsync(id);

            if(carType == null)
            {
                return NotFound($"Car Type With ID {id} Not Found");
            }

            carType.name = carTypeDTO.name;
            carType.description = carTypeDTO.description;
            carType.typical_seats = carTypeDTO.typical_seats;
            carType.typical_daily_rate = carTypeDTO.typical_daily_rate;

            if (await carType.SaveAsync())
            {
                return Ok("Car Type Updated Successfully!");
            }
            else
            {
                return StatusCode(500, new { Message = "Error Updating Car Type" });
            }
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpDelete(Name = "DeleteCarType")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteCarType(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Not Accepted ID: {id}");
            }


            car_types carType = await car_types.FindAsync(id);

            if(carType == null)
            {
                return NotFound($"Car Type Wit ID {id} Not Found");
            }

            if(await car_types.DeleteCarTypeAsync(id))
            {
                return Ok($"Car Type With ID: {id} Deleted Successfully"); 
            }
            else
            {
                return NotFound($"Car Type Wit ID {id} Not Found");
            }
        }

    }
}
