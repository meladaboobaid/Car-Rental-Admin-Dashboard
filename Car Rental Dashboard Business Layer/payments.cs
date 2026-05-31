using Car_Rental_Dashboard_Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Dashboard_Business_Layer
{
    public class payments
    {
        /// <summary>
        /// This method calculate the revenue in specific status like completed, cancelled, pending, failed, or refunded payment 
        /// from the first day of the month up to the current moment
        /// without waiting the month to end, and revenue for today is included
        /// </summary>
        public static async Task<double> GetRevenueForThisMonthInSpecificStatusAsync(string status)
        {
            return await paymentsData.GetRevenueForThisMonthInSpecificStatusAsync(status);
        }

        /// <summary>
        /// This method count the number of transactions for specific payment status like refunded, completed, pending, failed, or cancelled 
        /// from the first day of the month up to the current moment ,
        /// and today is included 
        /// </summary>
        public static async Task<int> NumberOfTransactionsInMonthForSpecificStatusAsync(string status)
        {
            return await paymentsData.NumberOfTransactionsInMonthForSpecificStatusAsync(status);
        }

        /// <summary>
        /// This method calculates the percentage of month-over-month revenue, for completed payments
        /// </summary>
        public static async Task<string> GetRevenueChangePercentageRateAsync()
        {
            return await paymentsData.GetRevenueChangePercentageRateAsync();
        }

        /// <summary>
        /// This method calculates the refund rate change.
        /// Returns the percentage point change between this month and last one
        /// </summary>
        public static async Task<string> GetRefundRateChangeAsync()
        {
            return await paymentsData.GetRefundRateChangeAsync();
        }
    

        public static async Task<double> GetTheTotalOfRevenueThisYearAsync()
        {
            return await paymentsData.GetTheTotalRevenueThisYearAsync();
        }

        public static async Task<double> GetTheAverageOfRevenueThisYearAsync()
        {
            return await paymentsData.GetTheAverageOfRevenueThisYearAsync();
        }


        /// <summary>
        /// This method calculates the year-over-year revenue growth by comparing current 
        /// year-to-date revenue against the same period last year
        /// </summary>
        public static async Task<double> GetTheGrowthRateAsync()
        {
            return await paymentsData.GetTheGrowthRateAsync();
        }
    }
}
