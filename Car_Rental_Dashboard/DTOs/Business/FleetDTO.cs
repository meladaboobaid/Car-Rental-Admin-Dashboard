using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Business
{
    public class FleetDTO
    {
        public string brand { get; set; }
        public string model { get; set; }
        public string category { get; set; }
        public string company { get; set; }
        public string city { get; set; }
        public int year { get; set; }
        public string license_plate { get; set; }
        public string color { get; set; }
        public double daily_rate { get; set; }
        public string status { get; set; }

        public FleetDTO(string brand, string model, string category, string company, string city, int year, string license_plate,
            string color, double daily_rate, string status)
        {

            this.brand = brand;
            this.model = model;
            this.category = category;
            this.company = company;

            this.city = city;
            this.year = year;
            this.license_plate = license_plate;
            this.color = color;

            this.daily_rate = daily_rate;
            this.status = status;
        }
    }
}
