using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        //public LoginRequest(string Email, string Password)
        //{
        //    this.Email = Email;
        //    this.Password = Password;
        //}
    }
}
