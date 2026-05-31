using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Data_Access_Layer
{

    public class rentalsDTO
    {
        public int CarId { get; set; }
        public string ClientName { get; set; }
        public string LicensePlate { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public rentalsDTO(int CarId, string ClientName, string LicensePlate, DateTime StartDate, DateTime EndDate, double TotalAmount, string Status,
            DateTime CreatedAt)
        {
            this.CarId = CarId;
            this.ClientName = ClientName;
            this.LicensePlate = LicensePlate;
            this.StartDate = StartDate;

            this.EndDate = EndDate;
            this.TotalAmount = TotalAmount;
            this.Status = Status;
            this.CreatedAt = CreatedAt;
        }
    }

    public class rentalsData
    {
        private static ILogger<rentalsDTO>? _logger;

        public static void SetLogger(ILogger<rentalsDTO> logger)
        {
            _logger = logger;
        }


        public static async Task<List<rentalsDTO>> GetTop5ActiveRentalsAsync()
        {
            List<rentalsDTO> activeRentals = new List<rentalsDTO>();

            string query = @"select top 5  clients.first_name + ' ' + clients.last_name as 'Client Name', cars.id as 'Car Id', cars.license_plate 
                            as 'License Plate', rentals.start_date, rentals.end_date, rentals.total_amount, rentals.status, rentals.created_at 
                            from rentals 
                            inner join clients on rentals.client_id = clients.id
			                inner join cars on rentals.car_id = cars.id
                            where rentals.status = 'active' and GETDATE() < rentals.end_date
                            order by rentals.end_date asc";

            using (SqlConnection connection = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {

                cmd.CommandType = CommandType.Text;

                try
                {
                    await connection.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        int clientNameOrdinal = reader.GetOrdinal("Client Name");
                        int CarIdOrdinal = reader.GetOrdinal("Car Id");
                        int license_plateOrdinal = reader.GetOrdinal("License Plate");
                        int endDateOrdinal = reader.GetOrdinal("end_date");

                        int startDateOrdinal = reader.GetOrdinal("start_date");
                        int totalAmountOrdinal = reader.GetOrdinal("total_amount");
                        int statusOrdinal = reader.GetOrdinal("status");
                        int created_atOrdinal = reader.GetOrdinal("created_at");



                        while (await reader.ReadAsync())
                        {
                            // 2. Use the cached ordinals here for maximum performance
                            activeRentals.Add(new rentalsDTO(
                                CarId: reader.GetInt32(CarIdOrdinal),
                                ClientName: reader.IsDBNull(clientNameOrdinal) ? string.Empty : reader.GetString(clientNameOrdinal),
                                LicensePlate: reader.IsDBNull(license_plateOrdinal) ? string.Empty : reader.GetString(license_plateOrdinal),
                                StartDate: reader.IsDBNull(startDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(startDateOrdinal),
                                EndDate: reader.IsDBNull(endDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(endDateOrdinal),
                                TotalAmount: reader.IsDBNull(totalAmountOrdinal) ? 0 : reader.GetDouble(totalAmountOrdinal),
                                Status: reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal),
                                CreatedAt: reader.IsDBNull(created_atOrdinal) ? DateTime.MinValue : reader.GetDateTime(created_atOrdinal)
                            ));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "An error occurred while getting rentals..");
                    throw;
                }

                return activeRentals;
            }
        }
    
        public static async Task<int> NumberOfActiveRentalsAsync()
        {
            int activeRentals = 0;
            string query = @"select count(id) from rentals
                             where start_date >= DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 0)
                             and status = 'active'";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result  = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out activeRentals)) { }

                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);  
                    _logger.LogError(ex, "An error occurred while counting active rentals..");
                    throw;
                }
            }

            return activeRentals;
        }
    
    
        public static async Task<int> NumberOfActiveRentalsLastWeekASync()
        {
            int numberOfRentals = 0;

            string query =
                        @"select count(id) from rentals
                        where start_date >= DATEADD(day, -7 , CAST(GETDATE() as DATE))
                        and start_date < CAST(GETDATE() as DATE) 
                        and status = 'active'";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out numberOfRentals)) { }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while counting active rentals..");
                    throw;
                }
            }

            return numberOfRentals;

        }
    
    }
}
