namespace Car_Rental_Dashboard_Server.DTOs.Auth
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
        public string Email { get; set; }
    }
}
