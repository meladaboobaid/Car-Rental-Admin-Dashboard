using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Car_Rental_Dashboard_Data_Access_Layer;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class companies
    {
        public static async Task<List<companiesDTO>> GetAllCompaniesAsync()
        {
            return await companiesData.GetAllCompaniesAsync();
        }
    
        public static async Task<int?> GetCompanyIdByNameAsync(string name)
        {
            return await companiesData.GetCompanyIdByNameAsync(name);
        }

        public static async Task<string?> GetCompanyNameByIdAsync(int id)
        {
            return await companiesData.GetCompanyNameByIdAsync(id);
        }


    }
}
