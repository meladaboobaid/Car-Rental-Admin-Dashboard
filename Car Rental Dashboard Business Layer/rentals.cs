using Car_Rental_Dashboard_Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class rentals
    {
        public enum SaveState { AddNew = 1, Update = 2 };
        public SaveState SaveMode { get; private set; } = SaveState.AddNew;

        public int CarId { get; set; }
        public string ClientName { get; set; }
        public string LicensePlate { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public rentalsDTO rentalDTO
        {
            get
            {
                return (new rentalsDTO(this.CarId, this.ClientName, this.LicensePlate, this.StartDate, this.EndDate,
                    this.TotalAmount, this.Status, this.CreatedAt));
            }
        }

        public rentals(rentalsDTO rentalsDTO, SaveState mode  = SaveState.AddNew)
        {
            this.CarId = rentalsDTO.CarId;
            this.ClientName = rentalsDTO.ClientName;
            this.LicensePlate = rentalsDTO.LicensePlate;

            this.StartDate = rentalsDTO.StartDate;
            this.EndDate = rentalsDTO.EndDate;
            this.TotalAmount = rentalsDTO.TotalAmount;

            this.Status = rentalsDTO.Status;
            this.CreatedAt = rentalsDTO.CreatedAt;
        }


        public static async Task<List<rentalsDTO>> GetTop5ActiveRentalsAsync()
        {
            return await rentalsData.GetTop5ActiveRentalsAsync();
        }

        public static async Task<int> NumberOfActiveRentalsAsync()
        {
            return await rentalsData.NumberOfActiveRentalsAsync();
        }

        public static async Task<int> NumberOfActiveRentalsLastWeekAsync()
        {
            return await rentalsData.NumberOfActiveRentalsLastWeekASync();
        }

    }
}
