using Car_Rental_Dashboard_Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class users
    {
        public enum SaveState { AddNew = 1, Update = 2 };
        public SaveState SaveMode { get; private set; } = SaveState.AddNew;

        public int id { get; set; }
        public  string name { get; set; }

        // Authentication-related fields 
        public string email { get; set; }
        public string passoword { get; set; }
        public string role { get; set; }

        // Refresh token fields 
        public string? refreshTokenHash { get; set; }
        public DateTime? refreshTokenExpiresAt { get; set; }
        public DateTime? refreshTokenRevokedAt { get; set; }



        public usersDTO userDTO
        {
            get
            { 
                return (new usersDTO(this.id, this.name, this.email, this.passoword, this.role, this.refreshTokenHash,
                    this.refreshTokenExpiresAt, this.refreshTokenRevokedAt));
            } 
        }

        public users(usersDTO userDTO, SaveState mode = SaveState.AddNew)
        {
            this.SaveMode = mode;

            this.id = userDTO.id;
            this.name = userDTO.name;
            this.email = userDTO.email;
            this.passoword = userDTO.password;
            this.role = userDTO.role;


            if (userDTO.refreshTokenHash == null)
            {
                this.refreshTokenHash = null;
            }
            else
            {
                this.refreshTokenHash = userDTO.refreshTokenHash;
            }
            ////
            if (userDTO.refreshTokenExpiresAt == null)
            {
                this.refreshTokenExpiresAt = null;
            }
            else
            {
                this.refreshTokenExpiresAt = userDTO.refreshTokenExpiresAt;
            }
            ////
            if (userDTO.refreshTokenRevokedAt == null)
            {
                this.refreshTokenRevokedAt = null;
            }
            else
            {
                this.refreshTokenRevokedAt = userDTO.refreshTokenRevokedAt;
            }

        }

        public static async Task< List<usersDTO>> GetAllUsersAsync()
        {
            return  await usersData.GetAllUsersAsync();
        }


        public static async Task<users> FindAsync(int id)
        {
            usersDTO userDTO = await usersData.GetUserByIDAsync(id);

            if(userDTO != null)
            {
                return new users(userDTO, SaveState.Update);
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> _AddNewUserAsync()
        {

            this.id = await usersData.AddNewUserAsync(userDTO);
            return (this.id != -1);
        }

        private async Task <bool> _UpdateUserAsync()
        {
            return await usersData.UpdateUserAsync(userDTO);
        }

        public static async Task<bool> DeleteUserAsync(int id)
        {
            return await usersData.DeleteUserAsync(id);
        }

        public async Task<bool> SaveAsync()
        {
            switch(SaveMode)
            {
                case SaveState.AddNew:
                    if(await _AddNewUserAsync())
                        return true;
                    else
                        return false;
                case SaveState.Update:
                    return await _UpdateUserAsync();
                default:
                    return false;
            }
        }

    }
}
