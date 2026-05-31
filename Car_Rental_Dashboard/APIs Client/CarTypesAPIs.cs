using Car_Rental_Dashboard.DTOs.Auth;
using Car_Rental_Dashboard.DTOs.Business;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.APIs_Client
{
    public class CarTypesAPIs
    {
        public static async Task<DataTable?> CallGetCarsCategoryWithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/CarTypes/Category");

            // UsersAPIs.SendWithAutoRefreshAsync will apply bearer token and try refresh once if needed.
            using var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Access + refresh failed -> caller should trigger re-login
                return null;
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                // Authenticated but not allowed
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                // Log or surface error as needed
                return null;
            }

            // Read and deserialize JSON payload
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(json))
                return new DataTable(); // empty

            List<CarTypeDTO>? carsCategory;

            try
            {
                carsCategory = JsonConvert.DeserializeObject<List<CarTypeDTO>>(json);
            }
            catch (Exception)
            {
                // if deserialization fails, return empty table to avoid crashing UI
                return new DataTable();
            }

            if (carsCategory == null || carsCategory.Count == 0)
                return new DataTable();

            // Convert list to DataTable
            var dt_category = CarsAPI.ToDataTable(carsCategory);

            return dt_category;
        }

        public static async Task<int?> CallGetCarTypeIdByNameWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState, 
            string name)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/CarTypes/ByName{name}");

            using var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Access + refresh failed -> caller should trigger re-login
                return null;
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                // Authenticated but not allowed
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                // Log or surface error as needed
                return null;
            }

            var CarTypeId = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);

            return CarTypeId;
        }

        public static async Task<string?> CallGetCarTypeByID_WithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState, 
            int CarTypeID)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));


            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/CarTypes/{CarTypeID}");

            using var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                return null;
            }

            var carType = await response.Content.ReadFromJsonAsync<CarTypeDTO?>().ConfigureAwait(false);
            
            return carType?.name;
        }
    }
}
