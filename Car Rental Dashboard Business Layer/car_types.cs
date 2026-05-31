using Car_Rental_Dashboard_Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class car_types
    {
        public enum SaveState { AddNew = 1, Update = 2 };
        public SaveState SaveMode { get; private set; } = SaveState.AddNew;

        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int typical_seats { get; set; }
        public double typical_daily_rate { get; set; }

        public car_typesDTO car_typeDTO
        {
            get
            {
                return (new car_typesDTO(this.id, this.name, this.description, this.typical_seats, this.typical_daily_rate));
            }
        }

        public car_types(car_typesDTO car_typeDTO, SaveState mode = SaveState.AddNew)
        {
            this.id                 = car_typeDTO.id;

            this.name               = car_typeDTO.name;
            this.description        = car_typeDTO.description;
            this.typical_seats      = car_typeDTO.typical_seats;
            this.typical_daily_rate = car_typeDTO.typical_daily_rate;

            this.SaveMode = mode;
        }

        public static async Task<List<car_typesDTO>> GetAllCarTypesAsync()
        {
            return await car_typesData.GetAllCarTypesAsync();
        }

        public static async Task<List<car_typesDTO>> GetCarsCategoryAsync()
        {
            return await car_typesData.GetCarsCategoryAsync();
        }

        public static async Task<car_types> FindAsync(int id)
        {
            car_typesDTO car_typeDTO = await car_typesData.GetCarTypeByIdAsync(id);

            if (car_typeDTO != null)
            {
                return new car_types(car_typeDTO, SaveState.Update);
            }
            else
            {
                return null;
            }
        }

        private async Task <bool> _AddNewCarTypeAsync()
        {

            this.id = await car_typesData.AddNewCarTypeAsync(car_typeDTO);
            return (this.id != -1);
        }

        private async Task <bool> _UpdateCarTypeAsync()
        {
            return await car_typesData.UpdateCarTypeAsync(car_typeDTO);
        }

        public static async Task<bool> DeleteCarTypeAsync(int id)
        {
            return await car_typesData.DeleteCarTypeAsync(id);
        }

        public async Task<bool> SaveAsync()
        {
            switch (SaveMode)
            {
                case SaveState.AddNew:
                    if (await _AddNewCarTypeAsync())
                        return true;
                    else
                        return false;
                case SaveState.Update:
                    return await _UpdateCarTypeAsync();
                default:
                    return false;
            }
        }

        public static async Task<int> GetCarTypeIdByNameAsync(string name)
        {
            return await car_typesData.GetCarTypeIdByNameAsync(name);
        }
    }
}
