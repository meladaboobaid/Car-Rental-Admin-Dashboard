using Car_Rental_Dashboard.DTOs.Auth;
using Car_Rental_Dashboard.DTOs.Business;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Json;

namespace Car_Rental_Dashboard.APIs_Client
{
    public class CompaniesAPIs
    {
        public static async Task<DataTable?> CallGetAllCompaniesWithAutoRefreshAsync(
               HttpClient http,
               string email,
               TokenState tokenState)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Companies/GetAllCompanies");

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

            List<CompanyDTO>? companies;

            try
            {
                companies = JsonConvert.DeserializeObject<List<CompanyDTO>>(json);
            }
            catch (Exception)
            {
                // if deserialization fails, return empty table to avoid crashing UI
                return new DataTable();
            }

            if (companies == null || companies.Count == 0)
                return new DataTable();

            // Convert list to DataTable
            var dt_companies = CarsAPI.ToDataTable(companies);

            return dt_companies;
        }

        public static async Task<int?> CallGetCompanyIdByNameWithAutoRefreshAsync(HttpClient http,
               string email,
               TokenState tokenState, 
               string name)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Companies/GetCompanyIdByName/{name}");

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

            var CompanyId = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);

            return CompanyId;
        }


        public static async Task<string?> CallGetCompanyNameByID_WithAutoRefreshAsync(HttpClient http,
                string email,
                TokenState tokenState,
                int companyId)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Companies/GetCompanyNameByID/{companyId}");

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

            var companyName = await response.Content.ReadFromJsonAsync<string?>().ConfigureAwait(false);

            return companyName;
        }



    }
}
