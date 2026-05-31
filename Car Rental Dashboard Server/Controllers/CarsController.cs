using Car_Rental_Dashboard_Data_Access_Layer;
using Car_Rental_Dashboard_Business_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/cars")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetAllCars")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<carsDTO>>> GetAllCars()
        {
            var fleet  = await cars.GetAllCarsAsync();

            if(fleet.Count == 0)
            {
                return NotFound("Cars Not Found");
            }


            return Ok(fleet);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetFleetView")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<fleetDTO>>> GetFleetView()
        {
            var fleet = await cars.GetFleetViewAsync();

            if (fleet.Count == 0)
            {
                return NotFound("Cars Not Found");
            }

            return Ok(fleet);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("GetFleetViewByStatus")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<fleetDTO>>> GetFleetViewByStatus(string status)
        {
            var fleet = await cars.GetFleetViewByStatusAsync(status);

            if (fleet.Count == 0)
            {
                return NotFound("Cars Not Found");
            }

            return Ok(fleet);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("ByID{id}", Name = "GetCarByID")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<carsDTO>> GetCarByID(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID {id}");
            }

            var car = await cars.FindAsync(id);

            if (car == null)
            {
                return NotFound("Car Not Found");
            }

            carsDTO CarDTO = car.carDTO;

            return Ok(CarDTO);
        }

        [Authorize(Roles = "admin,Super_admin")]
        [HttpGet("ByLicensePlate{LicensePlate}", Name = "GetCarByLicensePlate")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<carsDTO>> GetCarByLicensePlate(string LicensePlate)
        {
            if (string.IsNullOrWhiteSpace(LicensePlate))
            {
                return BadRequest($"Not Accepted License Plate {LicensePlate}");
            }

            var car = await cars.FindAsync(LicensePlate);

            if (car == null)
            {
                return NotFound("Car Not Found");
            }

            carsDTO CarDTO = car.carDTO;

            return Ok(CarDTO);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpPost(Name = "AddNewCar")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<carsData>> AddNewCar(int id, carsDTO NewCar)
        {
            if (_ValidateCarData(NewCar))
            {
                return BadRequest("Invalid car data");
            }

            cars car = new cars(NewCar);

            if (await car.SaveAsync())
            {
                NewCar.id = car.id;

                return CreatedAtRoute("GetCarByID", new { id = NewCar.id }, NewCar);
            }
            else
            {
                return StatusCode(500, new { Message = "Error Adding The Car" });
            }
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpPut("{id}", Name = "UpdateCar")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<carsDTO>> UpdateCar(int id, carsDTO carDTO)
        {
            if (_ValidateCarData(carDTO))
            {
                return BadRequest("Invalid car data");
            }

            cars car = await cars.FindAsync(id);

            if (car == null)
            {
                return NotFound($"Car With ID {id} Not Found");
            }

            car.brand = carDTO.brand;
            car.model = carDTO.model;
            car.year = carDTO.year;

            car.color = carDTO.color;
            car.status = carDTO.status;
            car.license_plate = carDTO.license_plate;
            car.daily_rate = carDTO.daily_rate;
            car.mileage = carDTO.mileage;

            car.car_type_id = carDTO.car_type_id;
            car.location_id = carDTO.location_id;
            car.company_id = carDTO.company_id;


            if (await car.SaveAsync())
            {
                return Ok("Car Updated Successfully!");
            }
            else
            {
                return StatusCode(500, new { Message = "Error Updating The Car" });
            }
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpDelete(Name = "DeleteCar")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteCar(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID: {id}");
            }


            cars car = await cars.FindAsync(id);

            if (car == null)
            {
                return NotFound($"Car Wit ID {id} Not Found");
            }

            if (await cars.DeleteCarAsync(id))
            {
                return Ok($"Car With ID: {id} Deleted Successfully");
            }
            else
            {
                return StatusCode(500, new { Message = "Error Deleting The Car" });
            }
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("NumberOfCars{status}", Name = "NumberOfCarsForSpecificStatus")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> NumberOfCarsForSpecificStatus(string status)
        {
            var NumberOfCars = await cars.NumberOfCarsForSpecificStatusAsync(status);

            if(NumberOfCars == 0)
            {
                return NotFound("There are no cars");
            }

            if(NumberOfCars < 0)
            {
                return StatusCode(500, new { Message = "Error Counting Number Of Cars" });
            }

            return Ok(NumberOfCars);
        }


        [Authorize(Roles = "admin, Super_admin")]
        [EnableRateLimiting("UsersLimiter")]
        [HttpGet("FleetSize", Name = "FleetSize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> FleetSize()
        {
            var size = await cars.FleetSizeAsync();

            if(size == 0)
            {
                return NotFound("No Cars In The Fleet");
            }

            if (size < 0)
            {
                return StatusCode(500, new { Message = "Error Counting Number Of Cars" });
            }

            return Ok(size);
        }

        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("AvailabilityRate", Name = "AvailabilityRate")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> AvailabilityRate()
        {
            var rate = await cars.AvailabilityRateAsync();

            if (rate == 0)
            {
                return NotFound("Availability Rate Is Zero");
            }

            if (rate < 0 || rate > 100)
            {
                return StatusCode(500, new { Message = "Error While Calculating The Availability Rate" });
            }
            
            return Ok(rate);
        }



        private bool _ValidateCarData(carsDTO carDTO)
        {
            return (carDTO == null || string.IsNullOrWhiteSpace(carDTO.license_plate)
                                   || string.IsNullOrWhiteSpace(carDTO.model)
                                   || string.IsNullOrWhiteSpace(carDTO.brand)
                                   || string.IsNullOrWhiteSpace(carDTO.color)
                                   || string.IsNullOrWhiteSpace(carDTO.status)
                                   || (carDTO.year <= 1900 && carDTO.year >= 2030)
                                   || carDTO.location_id <= 0
                                   || carDTO.company_id <= 0
                                   || carDTO.daily_rate <= 0
                                   || carDTO.mileage <= 0
                                   || carDTO.car_type_id <= 0);
        }


    }
}
