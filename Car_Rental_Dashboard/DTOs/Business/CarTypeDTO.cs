using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Business
{
    public class CarTypeDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int typical_seats { get; set; }
        public double typical_daily_rate { get; set; }

        //public CarTypeDTO(int ID, string Name, string Description, int Typical_seats, double Typical_daily_rate)
        //{
        //    this.id = ID;
        //    this.name = Name;
        //    this.description = Description;
        //    this.typical_seats = Typical_seats;
        //    this.typical_daily_rate = Typical_daily_rate;
        //}
    }
}
