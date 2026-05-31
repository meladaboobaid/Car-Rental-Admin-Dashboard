using Car_Rental_Dashboard.DTOs.Auth;
using Car_Rental_Dashboard.DTOs.Business;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.APIs_Client
{
    public class CarsAPI
    {
        /// <summary>
        /// Calls the Cars API to get the fleet view, transparently attempts token refresh
        /// via <see cref="UsersAPIs.SendWithAutoRefreshAsync"/> and returns the result as a DataTable.
        /// Returns null when the call requires re-login (unauthorized) or on failure.
        /// </summary>
        public static async Task<DataTable?> CallGetCarsFleetWithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Cars/GetFleetView");

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

            List<FleetDTO>? fleet;

            try
            {
                fleet = JsonConvert.DeserializeObject<List<FleetDTO>>(json);
            }
            catch (Exception)
            {
                // if deserialization fails, return empty table to avoid crashing UI
                return new DataTable();
            }

            if (fleet == null || fleet.Count == 0)
                return new DataTable();

            // Convert list to DataTable
            var dt_Fleet = ToDataTable(fleet);

            return dt_Fleet;
        }

        public static async Task<DataTable?> CallGetCarsFleetByStatusWithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState, 
            string status)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/cars/GetFleetViewByStatus?status={status}");

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

            List<FleetDTO>? fleet;

            try
            {
                fleet = JsonConvert.DeserializeObject<List<FleetDTO>>(json);
            }
            catch (Exception)
            {
                // if deserialization fails, return empty table to avoid crashing UI
                return new DataTable();
            }

            if (fleet == null || fleet.Count == 0)
                return new DataTable();

            // Convert list to DataTable
            var dt_Fleet = ToDataTable(fleet);

            return dt_Fleet;
        }


        /// <summary>
        /// Converts a list into a DataTable.
        /// - Primitive and string properties are preserved with appropriate column types.
        /// - Complex/class properties (other than string) are serialized to JSON strings and stored in string columns.
        /// </summary>
        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var dt = new DataTable();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.GetMethod != null)
                                 .ToArray();

            // Define columns
            foreach (var p in props)
            {
                var propType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                // For complex reference types (except string) use string column to store JSON
                if (propType.IsClass && propType != typeof(string))
                {
                    dt.Columns.Add(p.Name, typeof(string));
                }
                else
                {
                    // DataTable doesn't accept enum types as column type directly well - store enums as string
                    if (propType.IsEnum)
                        dt.Columns.Add(p.Name, typeof(string));
                    else
                        dt.Columns.Add(p.Name, propType);
                }
            }

            // Fill rows
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    var val = props[i].GetValue(item);
                    if (val == null)
                    {
                        values[i] = DBNull.Value;
                        continue;
                    }

                    var propType = Nullable.GetUnderlyingType(props[i].PropertyType) ?? props[i].PropertyType;

                    if (propType.IsClass && propType != typeof(string))
                    {
                        // flatten complex objects into JSON string so grid can show them
                        values[i] = JsonConvert.SerializeObject(val);
                    }
                    else if (propType.IsEnum)
                    {
                        values[i] = val.ToString();
                    }
                    else
                    {
                        values[i] = val;
                    }
                }

                dt.Rows.Add(values);
            }

            return dt;
        }


        public static async Task<DataTable?> CallGetTop5ActiveRentalsWithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Rentals/GetTop5ActiveRentals");

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

            List<RentalDTO>? rentals;

            try
            {
                rentals = JsonConvert.DeserializeObject<List<RentalDTO>>(json);
            }
            catch (Exception)
            {
                // if deserialization fails, return empty table to avoid crashing UI
                return new DataTable();
            }

            if (rentals == null || rentals.Count == 0)
                return new DataTable();

            // Convert list to DataTable
            var dt_ActiveRentals = ToDataTable(rentals);

            return dt_ActiveRentals;
        }
   
    
        public static async Task<int?> CallGetFleetSizeWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/cars/FleetSize");

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

            var FleetSize = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);


            return FleetSize;
        }

        public static async Task<double?> CallGetAvailabilityRateWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/cars/AvailabilityRate");

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

            var availabilityRate = await response.Content.ReadFromJsonAsync<double?>().ConfigureAwait(false);


            return availabilityRate;
        }

        public static async Task<int?> CallGetNumberOfCarsForSpecificStatusWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState, string status)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/cars/NumberOfCars{status}");

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

            var numberOfCars = await response.Content.ReadFromJsonAsync<int?>().ConfigureAwait(false);

            return numberOfCars;
        }


        public static async Task<HttpResponseMessage?> CallAddCarWithAutoRefreshAsync(HttpClient http,
            string email,
            TokenState tokenState, 
            CarDTO newCar)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));
            if (newCar == null) throw new ArgumentNullException(nameof(newCar));

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/cars") 
            {
                Content = JsonContent.Create(newCar)
            };

            var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                response.Dispose();
                return null;
            }

            return response; // Caller should check IsSuccessStatusCode and dispose
        }

    
        public static async Task<CarDTO?>CallGetCarByLicensePlateAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState, string LicensePlate)
        {

            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));
            if (string.IsNullOrWhiteSpace(LicensePlate)) throw new ArgumentException("LicensePlate is required", nameof(LicensePlate));

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/cars/ByLicensePlate{LicensePlate}");

            using var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                return null;
            }

            var car = await response.Content.ReadFromJsonAsync<CarDTO?>().ConfigureAwait(false);

            return car;
        }


        public static async Task<HttpResponseMessage?> CallUpdateCar_WithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState,
            CarDTO UpdatedDTO, int id)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));
            if (UpdatedDTO == null) throw new ArgumentNullException(nameof(UpdatedDTO));

            using var request = new HttpRequestMessage(HttpMethod.Put, $"api/cars/{id}")
            {
                Content = JsonContent.Create(UpdatedDTO)
            };

            var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                response.Dispose();
                return null;
            }

            return response; // Caller should check IsSuccessStatusCode and dispose
        }


        public static async Task<HttpResponseMessage?> CallDeleteCar_WithAutoRefreshAsync(
            HttpClient http,
            string email,
            TokenState tokenState,
            int? id)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));
            if (tokenState == null) throw new ArgumentNullException(nameof(tokenState));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required", nameof(email));

            using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/cars?id={id}");


            var response = await UsersAPIs.SendWithAutoRefreshAsync(http, request, email, tokenState).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                response.Dispose();
                return null;
            }

            return response; // Caller should check IsSuccessStatusCode and dispose
        }
    }
}