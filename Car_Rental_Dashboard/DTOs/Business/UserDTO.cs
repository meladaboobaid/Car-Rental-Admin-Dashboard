using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Business
{
    public class UserDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string? refreshTokenHash { get; set; }

        public DateTime? refreshTokenExpiresAt { get; set; }
        public DateTime? refreshTokenRevokedAt { get; set; }

        public UserDTO(int id, string name, string email, string password, string role, string? refreshTokenHash,
            DateTime? refreshTokenExpiresAt, DateTime? refreshTokenRevokedAt)
        {
            this.id                    = id;
            this.name                  = name;
            this.email                 = email;
            this.password              = password;
            this.role                  = role;
            this.refreshTokenHash      = refreshTokenHash;
            this.refreshTokenExpiresAt = refreshTokenExpiresAt;
            this.refreshTokenRevokedAt = refreshTokenRevokedAt;
        }
    }
}
