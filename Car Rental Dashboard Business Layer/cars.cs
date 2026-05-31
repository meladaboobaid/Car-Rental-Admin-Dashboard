using Car_Rental_Dashboard_Data_Access_Layer;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Data;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class cars
    {

        public enum SaveState { AddNew = 1, Update = 2 };
        public SaveState SaveMode { get; private set; } = SaveState.AddNew;

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
        

        // properties for fleet DTO
        public string category { get; set; }
        public string city { get; set; }
        public string company { get; set; }

        public carsDTO carDTO
        {
            get
            {
                return (new carsDTO(this.id, this.company_id, this.car_type_id, this.location_id, this.brand,
                    this.model, this.year, this.license_plate, this.color, this.daily_rate, this.mileage, this.status));
            }
        }

        public fleetDTO fleetDTO
        {
            get
            {
                return (new fleetDTO(this.brand, this.model, this.category, this.company,
                this.city, this.year, this.license_plate, this.color, this.daily_rate, this.status));
            }
        }

        public cars(carsDTO carDTO, SaveState mode = SaveState.AddNew)
        {
            this.id = carDTO.id;
            this.car_type_id = carDTO.car_type_id;
            this.company_id = carDTO.company_id;
            this.location_id = carDTO.location_id;

            this.brand = carDTO.brand;
            this.model = carDTO.model;
            this.year = carDTO.year;
            this.license_plate = carDTO.license_plate;

            this.color = carDTO.color;
            this.mileage = carDTO.mileage;
            this.daily_rate = carDTO.daily_rate;
            this.status = carDTO.status;

            this.SaveMode = mode;
        }

        public static async Task<List<carsDTO>> GetAllCarsAsync()
        {
            return await carsData.GetAllCarsAsync();
        }
        public static async Task<List<fleetDTO>> GetFleetViewAsync()
        {
            return await carsData.GetFleetView();
        }
        public static async Task<List<fleetDTO>> GetFleetViewByStatusAsync(string status)
        {
            return await carsData.GetFleetViewByStatusAsync(status);
        }
        public static async Task<cars> FindAsync(int id)
        {
            carsDTO car_dto = await carsData.GetCarByIDAsync(id);

            if (car_dto != null)
            {
                return new cars(car_dto, SaveState.Update);
            }
            else
            {
                return null;
            }
        }
        public static async Task<cars> FindAsync(string LicensePlate)
        {
            carsDTO car_dto = await carsData.GetCarByLicensePlateAsync(LicensePlate);

            if (car_dto != null)
            {
                return new cars(car_dto, SaveState.Update);
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> _AddNewCarAsync()
        {

            this.id = await carsData.AddNewCarAsync(carDTO);
            return (this.id != -1);
        }

        private async Task<bool> _UpdateCarAsync()
        {
            return await carsData.UpdateCarAsync(carDTO);
        }

        public static async Task<bool> DeleteCarAsync(int id)
        {
            return await carsData.DeleteCarAsync(id);
        }

        public async Task<bool> SaveAsync()
        {
            switch (SaveMode)
            {
                case SaveState.AddNew:
                    if (await _AddNewCarAsync())
                        return true;
                    else
                        return false;
                case SaveState.Update:
                    return await _UpdateCarAsync();
                default:
                    return false;
            }
        }

        public static async Task<int> NumberOfCarsForSpecificStatusAsync(string status)
        {
            return await carsData.NumberOfCarsForSpecificStatusAsync(status);
        }

        public static async Task<int> FleetSizeAsync()
        {
            return await carsData.FleetSizeAsync();
        }

        public static async Task<int> AvailabilityRateAsync()
        {
            return await carsData.AvailabilityRateAsync();
        }
   
    
    }
}
