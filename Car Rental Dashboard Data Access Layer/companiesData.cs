using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Data_Access_Layer
{
    public class companiesDTO
    {
        public int company_id { get; set; }
        public string company_name { get; set; }
    }

    public class companiesData
    {
        private static ILogger<companiesData>? _logger;

        public static void SetLogger(ILogger<companiesData> logger)
        {
            _logger = logger;
        }

        public static async Task<List<companiesDTO>> GetAllCompaniesAsync()
        {
            string query = @"select name from companies";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                List<companiesDTO> companies = new List<companiesDTO>();
                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            companies.Add(new companiesDTO
                            {
                                company_name = reader.GetString(0)
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting companies..");
                    throw; // Rethrow to let the caller handle the failure                }
                }

                return companies;
            }
        }

        public static async Task<int?> GetCompanyIdByNameAsync(string name)
        {
            string query = @"select id from companies where name = @name";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int companyId))
                    {
                        return companyId;
                    }
                    else
                    {
                        return null; // Company not found
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting company ID by name..");
                    throw; // Rethrow to let the caller handle the failure
                }
            }
        }

        public static async Task<string?> GetCompanyNameByIdAsync(int id)
        {
            string query = @"select name from companies
                                where id  = @id";
            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        return result.ToString(); // Company name found
                    }
                    else
                    {
                        return null; // Company not found
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting company name by ID..");
                    throw; // Rethrow to let the caller handle the failure
                }
            }
        }
    
    }
}
