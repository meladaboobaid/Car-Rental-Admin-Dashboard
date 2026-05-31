using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Auth
{
    public class TokenState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        //public TokenState(string accessToken, string refreshToken)
        //{
        //    AccessToken = accessToken;
        //    RefreshToken = refreshToken;
        //}
    }
}
