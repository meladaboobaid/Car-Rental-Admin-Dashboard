using Car_Rental_Dashboard.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.APIs_Client
{
    public class RentalsAPIs
    {

        
        public static async Task<int?> CallGetNumberOfActiveRentalsWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Rentals/NumberOfActiveRentals");

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

            var numberOfActiveRentals = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);


            return numberOfActiveRentals;
        }


        public static async Task<int?> CallGetNumberOfActiveRentalsLastWeekWithAutoRefreshAsync(HttpClient http,
                 string email,
                 TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Rentals/NumberOfActiveRentalsLastWeek");

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

            var numberOfActiveRentals = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);


            return numberOfActiveRentals;
        }

    }
}
