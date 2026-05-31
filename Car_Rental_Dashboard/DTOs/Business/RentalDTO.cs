using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Business
{
    public class RentalDTO
    {
        public int CarId { get; set; }
        public string ClientName { get; set; }
        public string LicensePlate { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public RentalDTO(int CarId, string ClientName, string LicensePlate, DateTime StartDate, DateTime EndDate, double TotalAmount, string Status,
            DateTime CreatedAt)
        {
            this.CarId = CarId;
            this.ClientName = ClientName;
            this.LicensePlate = LicensePlate;
            this.StartDate = StartDate;

            this.EndDate = EndDate;
            this.TotalAmount = TotalAmount;
            this.Status = Status;
            this.CreatedAt = CreatedAt;
        }
    }
}
