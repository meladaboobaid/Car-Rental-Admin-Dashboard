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
    public class PaymentsAPIs
    {

        public static async Task<string?> CallGetTheGrowthRateWithAutoRefreshAsync(HttpClient http,
         string email,
         TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Payments/GetTheGrowthRate");

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

            var growthRate = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return growthRate;
        }


        public static async Task<double?> CallGetTheAverageOfRevenueThisYearWithAutoRefreshAsync(HttpClient http,
             string email,
             TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Payments/GetTheAverageOfRevenueThisYear");

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

            var averageOfRevenue = await response.Content.ReadFromJsonAsync<double?>().ConfigureAwait(false);

            return averageOfRevenue;
        }



        public static async Task<double?> CallGetTheTotalRevenueThisYearWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Payments/GetTheTotalOfRevenueThisYear");

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

            var totalRevenue = await response.Content.ReadFromJsonAsync<double?>().ConfigureAwait(false);

            return totalRevenue;
        }


        public static async Task<double?> CallGetTheTotalRevenueInSpecificStatusWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState,
            string status)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Payments/GetRevenueForThisMonthInSpecificStatus?status={status}");

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

            var totalRevenueInSpecificStatus = await response.Content.ReadFromJsonAsync<double?>().ConfigureAwait(false);

            return totalRevenueInSpecificStatus;
        }


        //api/Payments/NumberOfTransactionsInMonthForSpecificStatus? status = Refunded

        public static async Task<int?> CallGetNumberOfTransactionsInSpecificStatusWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState,
            string status)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Payments/NumberOfTransactionsInMonthForSpecificStatus?status={status}");

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

            var numberOfTransactions = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);

            return numberOfTransactions;
        }


       

        public static async Task<string?> CallGetRevenueChangePercentageRateWithAutoRefreshAsync(HttpClient http,
         string email,
         TokenState tokenState)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Payments/GetRevenueChangePercentageRate");
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
            var revenueChangePercentageRate = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return revenueChangePercentageRate;
        }


        //
        public static async Task<string?> CallGetRefundRateChangeWithAutoRefreshAsync(
                 HttpClient http,
                 string email,
                 TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Payments/GetRefundRateChange");
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
            var RefundRateChange = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return RefundRateChange;
        }

    }
}
