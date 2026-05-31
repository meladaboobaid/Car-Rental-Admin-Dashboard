using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Auth
{
    public class RefreshRequest
    {
        public string Email { get; set; } = "";
        public string RefreshToken { get; set; } = "";

        //public RefreshRequest(string email, string refreshToken)
        //{
        //    this.Email = email;
        //    this.RefreshToken = refreshToken;
        //}
    }
}
