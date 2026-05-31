using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Car_Rental_Dashboard_Data_Access_Layer
{

        public class car_typesDTO
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int typical_seats { get; set; }
            public double typical_daily_rate { get; set; }

            public car_typesDTO(int ID, string Name, string Description, int Typical_seats, double Typical_daily_rate)
            {
                this.id = ID;
                this.name = Name;
                this.description = Description;
                this.typical_seats = Typical_seats;
                this.typical_daily_rate = Typical_daily_rate;
            }
        }

        public class car_typesData
        {

            private static ILogger<car_typesData>? _logger;

            public static void SetLogger(ILogger<car_typesData> logger)
            {
                _logger = logger; 
            }

            public static async Task<List<car_typesDTO>> GetAllCarTypesAsync()
            {
                var car_types = new List<car_typesDTO>();
                string query = @"select id, name, description, typical_seats, typical_daily_rate from car_types";

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
                            int nameOrdinal = reader.GetOrdinal("name");
                            int descriptionOrdinal = reader.GetOrdinal("description");
                            int typical_seatsOrdinal = reader.GetOrdinal("typical_seats");
                            int typical_daily_rateOrdinal = reader.GetOrdinal("typical_daily_rate");

                            while (await reader.ReadAsync())
                            {
                                // 2. Use the cached ordinals here for maximum performance
                                car_types.Add(new car_typesDTO(
                                    ID: reader.GetInt32(idOrdinal),
                                    Name: reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                                    Description: reader.IsDBNull(descriptionOrdinal) ? string.Empty : reader.GetString(descriptionOrdinal),
                                    Typical_seats: reader.IsDBNull(typical_seatsOrdinal) ? 0 : reader.GetInt32(typical_seatsOrdinal),
                                    Typical_daily_rate: reader.IsDBNull(typical_daily_rateOrdinal) ? 0 : reader.GetInt32(typical_daily_rateOrdinal)

                                ));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                        _logger?.LogError(ex, "An error occurred while getting all car types..");
                        throw; // Rethrow to let the caller handle the failure
                    }
                }
                return car_types;
            }

            /// <summary>
            /// This method returns a names list of fleet car types 
            /// </summary>
            public static async Task<List<car_typesDTO>> GetCarsCategoryAsync()
            {
                var category = new List<car_typesDTO>();
                string query = @"select name from car_types";

                using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            int nameOrdinal = reader.GetOrdinal("name");

                            while (await reader.ReadAsync())
                            {
                                category.Add(new car_typesDTO(
                                ID: 0,
                                Name: reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                                Description: string.Empty,
                                Typical_seats: 0,
                                Typical_daily_rate: 0 ));

                            }
                            return category;
                        }
                    }
                    catch (SqlException ex)
                    {
                        _logger?.LogError(ex, "An error occurred while getting cars category");
                        throw; // Rethrow to let the caller handle the failure
                    }
                }
            }

            public static async Task<car_typesDTO> GetCarTypeByIdAsync(int id)
            {
                byte Id = Convert.ToByte(id);
                string query = @"Select name, description, typical_seats, typical_daily_rate from car_types 
                                where id = @ID";

                using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ID", Id);

                    try
                    {
                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new car_typesDTO
                                    (
                                    ID: Convert.ToInt32( Id),
                                    Name: reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                                    Description: reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description")),
                                    Typical_seats: reader.IsDBNull(reader.GetOrdinal("typical_seats")) ? 0 : reader.GetInt32(reader.GetOrdinal("typical_seats")),
                                    Typical_daily_rate: reader.IsDBNull(reader.GetOrdinal("typical_daily_rate")) ? 0 : reader.GetInt32(reader.GetOrdinal("typical_daily_rate"))
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
                        _logger?.LogError(ex, $"An error occurred while getting car type with {id}");
                        throw; // Rethrow to let the caller handle the failure
                    }

                }
            }

            public static async Task<int> AddNewCarTypeAsync(car_typesDTO car_type)
            {
                int id = -1;
                string query = @"insert into car_types(name, description, typical_seats, typical_daily_rate)
                                 values(@name, @description, @typical_seats, @typical_daily_rate)
                                 select SCOPE_IDENTITY()";
                          
                using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@name", car_type.name);
                    cmd.Parameters.AddWithValue("@description", car_type.description);
                    cmd.Parameters.AddWithValue("@typical_seats", car_type.typical_seats);
                    cmd.Parameters.AddWithValue("@typical_daily_rate", car_type.typical_daily_rate);


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
                        _logger?.LogError(ex, $"An error occurred while adding new car type...");
                        throw; // Rethrow to let the caller handle the failure
                    }
                }

                return id;
            }

            public static async Task<bool> UpdateCarTypeAsync(car_typesDTO car_type)
            {
                bool IsUpdated = false;
                string query = @"update car_types
                                  set 
                                      name  = @name,
                                      description = @description,
                                      typical_seats = @typical_seats,
                                      typical_daily_rate = @typical_daily_rate
                                      where id = @id";

                using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // We could here handle nullable attributes if needed 
                    cmd.Parameters.AddWithValue("@id", car_type.id);
                    cmd.Parameters.AddWithValue("@name", car_type.name);
                    cmd.Parameters.AddWithValue("@description", car_type.description);
                    cmd.Parameters.AddWithValue("@typical_seats", car_type.typical_seats);
                    cmd.Parameters.AddWithValue("@typical_daily_rate", car_type.typical_daily_rate);


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
                        _logger?.LogError(ex, $"An error occurred while updating the car type");
                        IsUpdated = false;
                        throw; // Rethrow to let the caller handle the failure
                    }
                }

                return IsUpdated;
            }

            public static async Task<bool> DeleteCarTypeAsync(int id)
            {
                bool IsDeleted = false;
                string query = @"delete from car_types where id = @id";

                using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);

                    try
                    {
                        await conn.OpenAsync();

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 1)
                            IsDeleted = true;
                        else
                            IsDeleted = false;
                    }
                    catch (SqlException ex)
                    {
                        _logger?.LogError(ex, "An error occurred while deleting the car type");
                        IsDeleted = false;
                        throw;
                    }
                }

                return IsDeleted;
            }

            public static async Task<int> GetCarTypeIdByNameAsync(string name)
            {
                int id = -1;
                string query = @"select id from car_types
                                 where name = @Type";

                using(SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
                using(SqlCommand cmd  = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Type", name);

                    try
                    {
                        await conn.OpenAsync();

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out id)) { }
                    }
                    catch (SqlException ex)
                    {
                        _logger?.LogError(ex, "An error occurred while Searching the car type id");
                        id = -1;
                        throw;
                    }
                }

                return id;
            }
        
        
        }
}

