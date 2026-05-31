namespace Car_Rental_Dashboard_Server.DTOs.Auth
{
    public class LogoutRequest
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
