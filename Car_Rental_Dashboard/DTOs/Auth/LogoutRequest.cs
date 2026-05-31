using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Auth
{
    public class LogoutRequest
    {
        public string email { get; set; }
        public string refreshToken { get; set; }

        public LogoutRequest(string email, string refreshToken)
        {
            this.email = email;
            this.refreshToken = refreshToken;
        }
    }
}
