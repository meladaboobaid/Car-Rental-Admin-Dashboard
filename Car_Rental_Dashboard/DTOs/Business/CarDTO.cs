using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard.DTOs.Business
{
    public class CarDTO
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int car_type_id { get; set; }
        public int location_id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public string license_plate { get; set; }
        public string color { get; set; }
        public double daily_rate { get; set; }
        public double mileage { get; set; }
        public string status { get; set; }

        //public CarDTO(int id, int company_id, int car_type_id, int location_id, string brand, string model, int year,
        //    string license_plate, string color, double daily_rate, double mileage, string status)
        //{
        //    this.id = id;
        //    this.company_id = company_id;
        //    this.car_type_id = car_type_id;
        //    this.location_id = location_id;
        //    this.brand = brand;
        //    this.model = model;
        //    this.year = year;
        //    this.license_plate = license_plate;
        //    this.color = color;
        //    this.daily_rate = daily_rate;
        //    this.mileage = mileage;
        //    this.status = status;
        //}
    }
}
