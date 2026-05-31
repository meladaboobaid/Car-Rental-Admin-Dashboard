using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Data_Access_Layer
{

    public class usersDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        //
        public string? refreshTokenHash { get; set; }
        public DateTime? refreshTokenExpiresAt { get; set; }
        public DateTime? refreshTokenRevokedAt { get; set; }

        public usersDTO(int id, string name, string email, string password, string role, string? refreshTokenHash,
            DateTime? refreshTokenExpiresAt, DateTime? refreshTokenRevokedAt)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.password = password;
            this.role = role;
            this.refreshTokenHash = refreshTokenHash;
            this.refreshTokenExpiresAt = refreshTokenExpiresAt;
            this.refreshTokenRevokedAt = refreshTokenRevokedAt;
        }
    }

    public class usersData
    {
        private static ILogger<usersData>? _logger;

        public static void SetLogger(ILogger<usersData> logger)
        {
            _logger = logger;
        }

        public static async Task<List<usersDTO>> GetAllUsersAsync()
        {
            var users = new List<usersDTO>();
            string query = @"select * from admins";

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

                        int emailOrdinal = reader.GetOrdinal("email");
                        int PasswordHashOrdinal = reader.GetOrdinal("password");
                        int RoleOrdinal = reader.GetOrdinal("role");
                        int RefreshTokenHashOrdinal = reader.GetOrdinal("refreshTokenHash");
                        int RefreshTokenExpiresAtOrdinal = reader.GetOrdinal("refreshTokenExpiresAt");
                        int RefreshTokenRevokedAtOrdinal = reader.GetOrdinal("refreshTokenRevokedAt");


                        while (await reader.ReadAsync())
                        {
                            // 2. Use the cached ordinals here for maximum performance
                            users.Add(new usersDTO(
                                id: reader.GetInt32(idOrdinal),
                                name: reader.IsDBNull(nameOrdinal) ?                                   string.Empty : reader.GetString(nameOrdinal),
                                email: reader.IsDBNull(emailOrdinal) ?                                 string.Empty : reader.GetString(emailOrdinal),
                                password: reader.IsDBNull(PasswordHashOrdinal) ?                       string.Empty : reader.GetString(PasswordHashOrdinal),
                                role: reader.IsDBNull(RoleOrdinal) ?                                   string.Empty : reader.GetString(RoleOrdinal),
                                refreshTokenHash: reader.IsDBNull(RefreshTokenHashOrdinal) ? string.Empty : reader.GetString(RefreshTokenHashOrdinal),
                                refreshTokenExpiresAt: reader.IsDBNull(RefreshTokenExpiresAtOrdinal) ? null : reader.GetDateTime(RefreshTokenExpiresAtOrdinal),
                                refreshTokenRevokedAt: reader.IsDBNull(RefreshTokenRevokedAtOrdinal) ? null : reader.GetDateTime(RefreshTokenRevokedAtOrdinal)

                            ));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger?.LogError(ex, "An error occurred while getting all users");
                    throw; // Rethrow to let the caller handle the failure
                }
            }
            return users;
        }


        public static async Task<usersDTO> GetUserByIDAsync(int id)
        {

            string query = @"select * from admins where id = @id";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new usersDTO
                                (
                                id: reader.GetInt32(reader.GetOrdinal("id")),
                                name: reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                                email: reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                                password: reader.IsDBNull(reader.GetOrdinal("password")) ? string.Empty : reader.GetString(reader.GetOrdinal("password")),
                                role: reader.IsDBNull(reader.GetOrdinal("role")) ? string.Empty : reader.GetString(reader.GetOrdinal("Role")),

                                refreshTokenHash: reader.IsDBNull("refreshTokenHash") ? string.Empty : reader.GetString("refreshTokenHash"),
                                refreshTokenExpiresAt: reader.IsDBNull("refreshTokenExpiresAt") ? null : reader.GetDateTime("refreshTokenExpiresAt"),
                                refreshTokenRevokedAt: reader.IsDBNull("refreshTokenRevokedAt") ? null : reader.GetDateTime("refreshTokenRevokedAt")

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
                    _logger?.LogError(ex, $"An error occurred while getting user with {id}");
                    throw; // Rethrow to let the caller handle the failure
                }

            }
        }

        public static async Task<int> AddNewUserAsync(usersDTO user)
        {
            int UserID = -1;
            string query = @"insert into admins ( name, email, password, role, refreshTokenHash, refreshTokenExpiresAt, refreshTokenRevokedAt)
                            values (@name, @email, @password, @role, @refreshTokenHash, @refreshTokenExpiresAt, @refreshTokenRevokedAt)
                            select SCOPE_IDENTITY()";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@name", user.name);
                cmd.Parameters.AddWithValue("@email", user.email);
                cmd.Parameters.AddWithValue("@password", user.password);
                cmd.Parameters.AddWithValue("@role", user.role);
                cmd.Parameters.AddWithValue("@refreshTokenHash", user.refreshTokenHash);
                cmd.Parameters.AddWithValue("@refreshTokenExpiresAt", user.refreshTokenExpiresAt);
                cmd.Parameters.AddWithValue("@refreshTokenRevokedAt", user.refreshTokenRevokedAt);


                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out UserID)){ }
                }
                catch (SqlException ex)
                {
                    _logger?.LogError(ex, $"An error occurred while adding the new user");
                    throw; // Rethrow to let the caller handle the failure
                }
            }

            return UserID;
        }

        public static async Task<bool> UpdateUserAsync(usersDTO user)
        {
            bool IsUpdated = false;
            string query = @"update admins
                             set 
	                         name =  @name,
	                         email = @email,
	                         password = @password,
	                         role = @role,
	                         refreshTokenHash = @refreshTokenHash,
	                         refreshTokenExpiresAt = @refreshTokenExpiresAt,
	                         refreshTokenRevokedAt = @refreshTokenRevokedAt
	                         where id  = @id";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", user.id);
                cmd.Parameters.AddWithValue("@name", user.name);
                cmd.Parameters.AddWithValue("@email", user.email);
                cmd.Parameters.AddWithValue("@password", user.password);
                cmd.Parameters.AddWithValue("@role", user.role);

                cmd.Parameters.AddWithValue("@refreshTokenHash", user.refreshTokenHash);


                if (user.refreshTokenExpiresAt == null)
                {
                    cmd.Parameters.AddWithValue("@refreshTokenExpiresAt", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@refreshTokenExpiresAt", user.refreshTokenExpiresAt);
                }


                if (user.refreshTokenRevokedAt == null)
                {
                    cmd.Parameters.AddWithValue("@refreshTokenRevokedAt", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@refreshTokenRevokedAt", user.refreshTokenRevokedAt);
                }


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
                    _logger?.LogError(ex, $"An error occurred while updating the user ");
                    IsUpdated = false;
                    throw; // Rethrow to let the caller handle the failure
                }
            }

            return IsUpdated;
        }

        public static async Task<bool> DeleteUserAsync(int UserID)
        {
            bool IsDeleted = false;
            string query = @"delete from admins where id = @id";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", UserID);

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
                    _logger?.LogError(ex, "An error occurred while deleting the user");
                    IsDeleted = false;
                    throw;
                }
            }

            return IsDeleted;
        }


    }
}
