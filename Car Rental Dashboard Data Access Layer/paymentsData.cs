using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Car_Rental_Dashboard_Data_Access_Layer
{
    public class paymentsData
    {
        private static ILogger<carsData>? _logger;

        public static void SetLogger(ILogger<carsData> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// This method calculate the revenue in specific status like completed, cancelled, pending, failed, or refunded payment 
        /// from the first day of the month up to the current moment
        /// without waiting the month to end, and revenue for today is included
        /// </summary>
        public static async Task<double> GetRevenueForThisMonthInSpecificStatusAsync(string status)
        {
            double RevenueForThisMonth = 0;

            string query = @"select SUM(amount) AS MTD_Revenue 
                             from payments
                             where paid_at is not null  
                             and 
                             paid_at >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                             and 
                             paid_at < DATEADD(DAY, 1, Cast(GETDATE() as DATE))
                             and status  = @status";

            using(SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@status", status);

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && double.TryParse(result.ToString(), out RevenueForThisMonth)) { }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the revenue in specific status for this month..");
                    throw;
                }
            }

            return RevenueForThisMonth;
        }

        /// <summary>
        /// This method count the number of transactions for specific payment status like refunded, completed, pending, failed, or cancelled 
        /// from the first day of the month up to the current moment ,
        /// and today is included 
        /// </summary>
        public static async Task<int> NumberOfTransactionsInMonthForSpecificStatusAsync(string status)
        {
            int NumberOfRefundedTransactions = 0;

            string query = @"
                                select count(status) AS MTD_Revenue 
                                from payments
                                where paid_at is not null  
                                and 
                                paid_at >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                                and 
                                paid_at < DATEADD(DAY, 1, Cast(GETDATE() as DATE))
                                and status  = @status";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd  = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@status", status);

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out NumberOfRefundedTransactions)){ }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while counting the number of transactions for specific status");
                    throw;
                }
            }

            return NumberOfRefundedTransactions;
        }

        /// <summary>
        /// This method calculates the percentage of month-over-month revenue
        /// </summary>
        public static async Task<string> GetRevenueChangePercentageRateAsync()
        {
            string revenuePercentage = string.Empty;
            string query = @"select dbo.GetRevenueChangePercent()";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd  = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if(result != null)
                    {
                        revenuePercentage = result.ToString();
                    }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the percentage of month-over-month revenue");
                    throw;
                }
            }

            return revenuePercentage;
        }
        

        /// <summary>
        /// This method calculates the refund rate change.
        /// Returns the percentage point change between this month and last one
        /// </summary>
        public static async Task<string> GetRefundRateChangeAsync()
        {
            string refundChangeRate = string.Empty;

            string query = @"select dbo.GetRefundRateChange()";

            using(SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using(SqlCommand cmd  = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if(result != null)
                    {
                        refundChangeRate = result.ToString();
                    }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the refund rate change");
                    throw;
                }
            }

            return refundChangeRate;
        }


        public static async Task<double> GetTheTotalRevenueThisYearAsync()
        {
            double total_revenue = 0;

            string query = @"select SUM(amount)
                             from payments
                             where paid_at is not null  
                             and 
                             paid_at >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                             and 
                             paid_at < DATEADD(DAY, 1, Cast(GETDATE() as DATE))
                             and status  = 'completed'";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && double.TryParse(result.ToString(), out total_revenue)) { }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the total revenue for this year");
                    throw;
                }
            }

            return total_revenue;
        }


        public static async Task<double> GetTheAverageOfRevenueThisYearAsync()
        {
            double total_revenue = 0;

            string query = @"select AVG(amount)
                             from payments
                             where paid_at is not null  
                             and 
                             paid_at >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
                             and 
                             paid_at < DATEADD(DAY, 1, Cast(GETDATE() as DATE))
                             and status  = 'completed'";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && double.TryParse(result.ToString(), out total_revenue)) { }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the total revenue for this year");
                    throw;
                }
            }

            return total_revenue;
        }


        public static async Task<double> GetTheGrowthRateAsync()
        {
            double growthRate = 0;

            string query = @"select dbo.GrowthRate()";

            using (SqlConnection conn = new SqlConnection(DataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && double.TryParse(result.ToString(), out growthRate)) { }

                }
                catch(SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogError(ex, "An error occurred while calculating the growth rate");
                    throw;
                }
            }

            return growthRate;
        }
    }
}
