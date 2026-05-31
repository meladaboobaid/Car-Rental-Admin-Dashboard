using Car_Rental_Dashboard.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard
{
    public class LoggedInUser
    {

        public string? Email { get; set; }
        public TokenState? Token { get; set; }


    }
}
