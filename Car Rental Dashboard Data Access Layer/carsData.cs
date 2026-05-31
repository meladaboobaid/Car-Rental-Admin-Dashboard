using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Data_Access_Layer
{
    public class carsDTO
    {
        // id, company_id, car_type_id, location_id, brand, model, year ,
        // license_plate, color, daily_rate, mileage, status 

        public int id {  get; set; }
        public int company_id { get; set; }
        public int car_type_id { get; set; }
        public int location_id { get; set; }
        public string brand { get;  set; }
        public string model { get; set; }
        public int year { get; set; }
        public string license_plate { get; set; }
        public string color { get; set; }
        public double daily_rate { get; set; }
        public double mileage {  get; set; }
        public string status { get; set; }

        public carsDTO(int id, int company_id, int car_type_id, int location_id, string brand, string model, int year, 
            string license_plate, string color, double daily_rate, double mileage, string status)
        {
            this.id = id;
            this.company_id = company_id;
            this.car_type_id = car_type_id;
            this.location_id = location_id;
            this.brand  = brand;
            this.model = model;
            this.year = year;
            this.license_plate = license_plate;
            this.color = color;
            this.daily_rate = daily_rate;
            this.mileage = mileage;
            this.status = status;
        }
    }

    public class fleetDTO
    {
        public string brand {get; set;}
        public string model {get; set;}
        public string category {get; set; }
        public string company  {get; set; }
        public string city { get; set; }
        public int year  {get; set;}
        public string license_plate {get; set; }
        public string color { get; set; }
        public double daily_rate { get; set; }
        public string status        {get; set; }

        public fleetDTO(string brand, string model, string category, string company, string city, int year, string license_plate,
            string color, double daily_rate, string status)
        {

            this.brand = brand;
            this.model = model;
            this.category = category;
            this.company = company;

            this.city = city;
            this.year = year;
            this.license_plate = license_plate;
            this.color = color;

            this.daily_rate = daily_rate;
            this.status = status;
        }
    }

    public class carsData
    {

        // This connection string needs to secure using management secret tool (Third Party tool like infisical)

        private static ILogger<carsData>? _logger;

        public static void SetLogger(ILogger<carsData> logger)
        {
            _logger = logger;
        }

        public static async Task<List<carsDTO>> GetAllCarsAsync()
        {
            var cars = new List<carsDTO>();
            string query = @"select * from cars";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        // 1. Correctly cache ordinals ONCE before the loop starts
                        int idOrdinal = reader.GetOrdinal("id");
                        int company_idOrdinal = reader.GetOrdinal("company_id");
                        int car_type_idOrdinal = reader.GetOrdinal("car_type_id");
                        int location_idOrdinal = reader.GetOrdinal("location_id");

                        int brandOrdinal = reader.GetOrdinal("brand");
                        int modelOrdinal = reader.GetOrdinal("model");
                        int yearOrdinal = reader.GetOrdinal("year");
                        int colorOrdinal = reader.GetOrdinal("color");

                        int license_plateOrdinal = reader.GetOrdinal("license_plate");
                        int statusOrdinal = reader.GetOrdinal("status");
                        int daily_rateOrdinal = reader.GetOrdinal("daily_rate");
                        int milageOrdinal = reader.GetOrdinal("mileage");


                        while (await reader.ReadAsync())
                        {
                            // 2. Use the cached ordinals here for maximum performance
                            cars.Add(new carsDTO(
                                id: reader.GetInt32(idOrdinal),
                                company_id: reader.GetInt32(company_idOrdinal),
                                car_type_id: reader.GetInt32(car_type_idOrdinal),
                                location_id: reader.GetInt32(location_idOrdinal),

                                brand: reader.IsDBNull(brandOrdinal) ? string.Empty : reader.GetString(brandOrdinal),
                                model: reader.IsDBNull(modelOrdinal) ? string.Empty : reader.GetString(modelOrdinal),
                                year: reader.IsDBNull(yearOrdinal) ? 0 : reader.GetInt32(yearOrdinal),
                                license_plate: reader.IsDBNull(license_plateOrdinal) ? string.Empty : reader.GetString(license_plateOrdinal),

                                color: reader.IsDBNull(colorOrdinal) ? string.Empty : reader.GetString(colorOrdinal),
                                daily_rate: reader.IsDBNull(daily_rateOrdinal) ? 0 : reader.GetDouble(daily_rateOrdinal),
                                mileage: reader.IsDBNull(milageOrdinal) ? 0 : reader.GetDouble(milageOrdinal),
                                status: reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal)

                            ));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting all cars..");
                    throw; // Rethrow to let the caller handle the failure
                }
            }
            return cars;
        }

        public static async Task<List<fleetDTO>> GetFleetViewByStatusAsync(string status)
        {
            var fleet = new List<fleetDTO>();
            string query = @"select * from Fleet_View where status = @status";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@status", status);

                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    { 
                        int brandOrdinal = reader.GetOrdinal("brand");
                        int modelOrdinal = reader.GetOrdinal("model");
                        int categoryOrdinal = reader.GetOrdinal("category");
                        int companyOrdinal = reader.GetOrdinal("company");
                        int cityOrdinal = reader.GetOrdinal("city");
                        int yearOrdinal = reader.GetOrdinal("year");
                        int license_plateOrdinal = reader.GetOrdinal("license_plate");
                        int colorOrdinal = reader.GetOrdinal("color");
                        int daily_rateOrdinal = reader.GetOrdinal("daily_rate");
                        int statusOrdinal = reader.GetOrdinal("status");


                        while (await reader.ReadAsync())
                        {
                            fleet.Add(new fleetDTO(

                                brand: reader.IsDBNull(brandOrdinal) ? string.Empty : reader.GetString(brandOrdinal),
                                model: reader.IsDBNull(modelOrdinal) ? string.Empty : reader.GetString(modelOrdinal),
                                category: reader.IsDBNull(categoryOrdinal) ? string.Empty : reader.GetString(categoryOrdinal),
                                company: reader.IsDBNull(companyOrdinal) ? string.Empty : reader.GetString(companyOrdinal),
                                city: reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal),
                                year: reader.IsDBNull(yearOrdinal) ? 0 : reader.GetInt32(yearOrdinal),
                                license_plate: reader.IsDBNull(license_plateOrdinal) ? string.Empty : reader.GetString(license_plateOrdinal),
                                color: reader.IsDBNull(colorOrdinal) ? string.Empty : reader.GetString(colorOrdinal),
                                daily_rate: reader.IsDBNull(daily_rateOrdinal) ? 0 : reader.GetDouble(daily_rateOrdinal),
                                status: reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal)
                           ));
                    }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting all cars..");
                    throw; // Rethrow to let the caller handle the failure
                }
            }
            return fleet;
        }

        /// <summary>
        /// This method returns fleet cars 
        /// </summary>
        public static async Task<List<fleetDTO>> GetFleetView()
        {
            var fleet = new List<fleetDTO>();
            string query = @"select * from Fleet_View";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        int brandOrdinal = reader.GetOrdinal("brand");
                        int modelOrdinal = reader.GetOrdinal("model");
                        int categoryOrdinal = reader.GetOrdinal("category");
                        int companyOrdinal = reader.GetOrdinal("company");
                        int cityOrdinal = reader.GetOrdinal("city");
                        int yearOrdinal = reader.GetOrdinal("year");
                        int license_plateOrdinal = reader.GetOrdinal("license_plate");
                        int colorOrdinal = reader.GetOrdinal("color");
                        int daily_rateOrdinal = reader.GetOrdinal("daily_rate");
                        int statusOrdinal = reader.GetOrdinal("status");


                        while (await reader.ReadAsync())
                        {
                            fleet.Add(new fleetDTO(

                                brand: reader.IsDBNull(brandOrdinal) ? string.Empty : reader.GetString(brandOrdinal),
                                model: reader.IsDBNull(modelOrdinal) ? string.Empty : reader.GetString(modelOrdinal),
                                category: reader.IsDBNull(categoryOrdinal) ? string.Empty : reader.GetString(categoryOrdinal),
                                company: reader.IsDBNull(companyOrdinal) ? string.Empty : reader.GetString(companyOrdinal),
                                city: reader.IsDBNull(cityOrdinal) ? string.Empty : reader.GetString(cityOrdinal),
                                year: reader.IsDBNull(yearOrdinal) ? 0 : reader.GetInt32(yearOrdinal),
                                license_plate: reader.IsDBNull(license_plateOrdinal) ? string.Empty : reader.GetString(license_plateOrdinal),
                                color: reader.IsDBNull(colorOrdinal) ? string.Empty : reader.GetString(colorOrdinal),
                                daily_rate: reader.IsDBNull(daily_rateOrdinal) ? 0 : reader.GetDouble(daily_rateOrdinal),
                                status: reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal)
                           ));


                        }
                    }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, "An error occurred while getting cars fleet");
                    throw; // Rethrow to let the caller handle the failure
                }
                
                return fleet;
            }
        }

        public static async Task<carsDTO> GetCarByIDAsync(int id)
        {
            string query = @"Select * from cars where id = @ID";
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);

                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new carsDTO
                                (
                                id: reader.GetInt32(reader.GetOrdinal("id")),
                                company_id: reader.GetInt32(reader.GetOrdinal("company_id")),
                                car_type_id: reader.GetInt32(reader.GetOrdinal("car_type_id")),
                                location_id: reader.GetInt32(reader.GetOrdinal("location_id")),

                                brand: reader.IsDBNull(reader.GetOrdinal("brand")) ? string.Empty : reader.GetString(reader.GetOrdinal("brand")),
                                model: reader.IsDBNull(reader.GetOrdinal("model")) ? string.Empty : reader.GetString(reader.GetOrdinal("model")),
                                year: reader.IsDBNull(reader.GetOrdinal("year")) ? 0 : reader.GetInt32(reader.GetOrdinal("year")),
                                license_plate: reader.IsDBNull(reader.GetOrdinal("license_plate")) ? string.Empty : reader.GetString(reader.GetOrdinal("license_plate")),

                                color: reader.IsDBNull(reader.GetOrdinal("color")) ? string.Empty : reader.GetString(reader.GetOrdinal("color")),
                                daily_rate: reader.IsDBNull(reader.GetOrdinal("daily_rate")) ? 0 : reader.GetDouble(reader.GetOrdinal("daily_rate")),
                                mileage: reader.IsDBNull(reader.GetOrdinal("mileage")) ? 0 : reader.GetDouble(reader.GetOrdinal("mileage")),
                                status: reader.IsDBNull(reader.GetOrdinal("status")) ? string.Empty : reader.GetString(reader.GetOrdinal("status"))
                                );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while getting car with {id}");
                    throw; // Rethrow to let the caller handle the failure
                }

            }
        }

        public static async Task<carsDTO> GetCarByLicensePlateAsync(string license_plate)
        {
            string query = @"select * from cars where license_plate = @license_plate";
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@license_plate", license_plate);

                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new carsDTO
                                (
                                id: reader.GetInt32(reader.GetOrdinal("id")),
                                company_id: reader.GetInt32(reader.GetOrdinal("company_id")),
                                car_type_id: reader.GetInt32(reader.GetOrdinal("car_type_id")),
                                location_id: reader.GetInt32(reader.GetOrdinal("location_id")),

                                brand: reader.IsDBNull(reader.GetOrdinal("brand")) ? string.Empty : reader.GetString(reader.GetOrdinal("brand")),
                                model: reader.IsDBNull(reader.GetOrdinal("model")) ? string.Empty : reader.GetString(reader.GetOrdinal("model")),
                                year: reader.IsDBNull(reader.GetOrdinal("year")) ? 0 : reader.GetInt32(reader.GetOrdinal("year")),
                                license_plate: reader.IsDBNull(reader.GetOrdinal("license_plate")) ? string.Empty : reader.GetString(reader.GetOrdinal("license_plate")),

                                color: reader.IsDBNull(reader.GetOrdinal("color")) ? string.Empty : reader.GetString(reader.GetOrdinal("color")),
                                daily_rate: reader.IsDBNull(reader.GetOrdinal("daily_rate")) ? 0 : reader.GetDouble(reader.GetOrdinal("daily_rate")),
                                mileage: reader.IsDBNull(reader.GetOrdinal("mileage")) ? 0 : reader.GetDouble(reader.GetOrdinal("mileage")),
                                status: reader.IsDBNull(reader.GetOrdinal("status")) ? string.Empty : reader.GetString(reader.GetOrdinal("status"))
                                );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while getting car with {license_plate}");
                    throw; // Rethrow to let the caller handle the failure
                }

            }
        }

        public static async Task<int> AddNewCarAsync(carsDTO car)
        {
            int id = -1;

            string query = @"insert into cars(company_id, car_type_id, location_id, brand, model, year, license_plate, color, daily_rate, mileage, status)
                             values (@company_id, @car_type_id, @location_id, @brand, @model, @year,  @license_plate, @color, @daily_rate, @mileage, @status)
                             select SCOPE_IDENTITY()";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@company_id", car.company_id);
                cmd.Parameters.AddWithValue("@car_type_id", car.car_type_id);
                cmd.Parameters.AddWithValue("@location_id", car.location_id);
                cmd.Parameters.AddWithValue("@brand", car.brand);

                cmd.Parameters.AddWithValue("@model", car.model);
                cmd.Parameters.AddWithValue("@year", car.year);
                cmd.Parameters.AddWithValue("@license_plate", car.license_plate);

                cmd.Parameters.AddWithValue("@color", car.color);
                cmd.Parameters.AddWithValue("@daily_rate", car.daily_rate);
                cmd.Parameters.AddWithValue("@mileage", car.mileage);
                cmd.Parameters.AddWithValue("@status", car.status);

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out id))
                    {
                    }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while adding a new car...");
                    throw; // Rethrow to let the caller handle the failure
                }
            }

            return id;
        }

        public static async Task<bool> UpdateCarAsync(carsDTO car)
        {
            bool IsUpdated = false;

            string query = @"update cars
                                  set 
                                      company_id  = @company_id,
                                      car_type_id =  @car_type_id,
                                      location_id = @location_id,
                                      brand = @brand,
                                      model = @model,
                                      year = @year,
                                      license_plate = @license_plate,
                                      color = @color,
                                      daily_rate = @daily_rate,
                                      mileage = @mileage,
                                      status = @status
                                      where id = @id";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                // We could here handle nullable attributes if needed 
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id", car.id);
                cmd.Parameters.AddWithValue("@company_id", car.company_id);
                cmd.Parameters.AddWithValue("@car_type_id", car.car_type_id);
                cmd.Parameters.AddWithValue("@location_id", car.location_id);

                cmd.Parameters.AddWithValue("@brand", car.brand);
                cmd.Parameters.AddWithValue("@model", car.model);
                cmd.Parameters.AddWithValue("@year", car.year);
                cmd.Parameters.AddWithValue("@license_plate", car.license_plate);

                cmd.Parameters.AddWithValue("@color", car.color);
                cmd.Parameters.AddWithValue("@daily_rate", car.daily_rate);
                cmd.Parameters.AddWithValue("@mileage", car.mileage);
                cmd.Parameters.AddWithValue("@status", car.status);


                try
                {
                    await conn.OpenAsync();

                    object result = await cmd.ExecuteNonQueryAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        IsUpdated = true;
                    }
                    else
                    {
                        IsUpdated = false;
                    }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while updating the car with Id: {car.id}");
                    IsUpdated = false;
                    throw; // Rethrow to let the caller handle the failure
                }
            }

            return IsUpdated;
        }

        public static async Task<bool> DeleteCarAsync(int id)
        {
            bool IsDeleted = false;
            string query = @"delete from cars where id = @id";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    await conn.OpenAsync();

                    int rowsAffected = Convert.ToInt32(await cmd.ExecuteNonQueryAsync());

                    if (rowsAffected == 1)
                        IsDeleted = true;
                    else
                        IsDeleted = false;
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while deleting the car with Id: {id}");
                    IsDeleted = false;
                    throw;
                }
            }

            return IsDeleted;
        }

        public static async Task<int> NumberOfCarsForSpecificStatusAsync(string status)
        {
            int NumberOfCars = 0;

            string query = @"select count(id) from cars where status = @status";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@status", status);

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if(result != null && int.TryParse(result.ToString(), out NumberOfCars)) {  }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, $"An error occurred while counting number of {status} cars ");
                    throw;
                }
            }

            return NumberOfCars;
        }

        public static async Task<int> FleetSizeAsync()
        {
            int FleetSize = 0;

            string query = @"select count(id) from cars";

            using(SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out FleetSize))
                    {
                    }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while counting the fleet cars");
                }
            }

            return FleetSize;

        }

        public static async Task<int> AvailabilityRateAsync()
        {
            int AvailabilityRate = 0;
            string query = @"select dbo.AvailabilityRate()";

            using (SqlConnection conn = new SqlConnection( DataAccessSettings.connectionString))
            using (SqlCommand cmd  = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if(result != null &&  int.TryParse( result.ToString(), out AvailabilityRate )) { }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the availability rate...");
                    throw;
                }
            }

            return AvailabilityRate;
        }



    }
}
