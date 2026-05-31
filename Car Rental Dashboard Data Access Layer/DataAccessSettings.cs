using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Data_Access_Layer
{
    public class DataAccessSettings
    {
        // The connection string is initialized at startup from appsettings.json
        public static string connectionString { get; set; } = string.Empty;
    }
}
