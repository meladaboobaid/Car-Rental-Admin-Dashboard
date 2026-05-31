using Car_Rental_Dashboard.APIs_Client;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Car_Rental_Dashboard
{
    public partial class frmAdminDashboard : Form
    {
        // 1. Declare the Windows API function
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // X-coordinate of upper-left corner
            int nTopRect,      // Y-coordinate of upper-left corner
            int nRightRect,    // X-coordinate of lower-right corner
            int nBottomRect,   // Y-coordinate of lower-right corner
            int nWidthEllipse, // Width of the ellipse (radius)
            int nHeightEllipse // Height of the ellipse (radius)
        );

        // 2. Apply the region when the form loads or resizes
        private void SetFormRoundedRegion(int radius)
        {
            // Create a rounded rectangle region for the form
            IntPtr hRgn = CreateRoundRectRgn(0, 0, Width, Height, radius, radius);
            Region = Region.FromHrgn(hRgn);
        }

        // 3. Call it in the constructor or Load event
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetFormRoundedRegion(33); // Change the number to adjust the roundness
        }

        // 4. Update the region if the form is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetFormRoundedRegion(30); // Re-apply the rounded shape
        }

        // --------------------------------------------------------------------------------------------------

        enum enAdminTabs
        {
            MainDashboard = 0, Fleet = 1, Rentals = 2, Customers = 3,
            Payments = 4, Reports = 5, Settings = 6
        };

        private LoggedInUser loggedInUser;
        private DataTable? _dt_Fleet;


        public frmAdminDashboard(LoggedInUser loggedInUser)
        {
            InitializeComponent();
            this.loggedInUser = loggedInUser;

        }

        private async Task<DataTable?> _LoadCarsFleet()
        {
            _dt_Fleet = await CarsAPI.CallGetCarsFleetWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            return _dt_Fleet;
        }

        private async Task<DataTable?> _LoadCarsFleet(string status)
        {
            _dt_Fleet = await CarsAPI.CallGetCarsFleetByStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token,
                status);

            return _dt_Fleet;
        }


        private async Task<DataTable?> _LoadLastRentals()
        {
            return await CarsAPI.CallGetTop5ActiveRentalsWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);
        }

        private async Task _LoadKPIsInfo()
        {
            var fleetSize = await CarsAPI.CallGetFleetSizeWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            var numberOfActiveRentalsThisWeek = await RentalsAPIs.CallGetNumberOfActiveRentalsWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            var numberOfActiveRentalsLastWeek = await RentalsAPIs.CallGetNumberOfActiveRentalsLastWeekWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            if (numberOfActiveRentalsThisWeek != null)
            {
                lblActiveRentals.Text = numberOfActiveRentalsThisWeek.ToString();
            }
            else
            {
                lblActiveRentals.Text = "N/A";
            }

            if (numberOfActiveRentalsLastWeek != null)
            {
                int DiffBetweenLastWeekAndThisWeek = Convert.ToInt32(numberOfActiveRentalsThisWeek) - Convert.ToInt32(numberOfActiveRentalsLastWeek);

                if (DiffBetweenLastWeekAndThisWeek > 0)
                {
                    lblNumberOfActiveRentalsThisWeek.Text = "+" + DiffBetweenLastWeekAndThisWeek.ToString();
                }
                else
                {
                    lblNumberOfActiveRentalsThisWeek.Text = DiffBetweenLastWeekAndThisWeek.ToString();
                }
            }
            else
            {
                lblNumberOfActiveRentalsThisWeek.Text = "N/A";
            }



            if (fleetSize != null)
                lblFleetSize.Text = fleetSize.ToString();
            else
                lblFleetSize.Text = "N/A";

            var availabilityRate = await CarsAPI.CallGetAvailabilityRateWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            if (availabilityRate != null)
                lblAvailabilityRate.Text = availabilityRate.ToString() + "%";
            else
                lblAvailabilityRate.Text = "N/A";

            var numberOfAvailableCars = await CarsAPI.CallGetNumberOfCarsForSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token,
                "Available");

            var numberOfRentedCars = await CarsAPI.CallGetNumberOfCarsForSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token,
                "Rented");

            var numberOfMaintenanceCars = await CarsAPI.CallGetNumberOfCarsForSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token,
                "Maintenance");

            var numberOfReservedCars = await CarsAPI.CallGetNumberOfCarsForSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token,
                "Reserved");

            UpdateFleetStatusChart(numberOfAvailableCars ?? 0, numberOfRentedCars ?? 0, numberOfMaintenanceCars ?? 0, numberOfReservedCars ?? 0);

            if (numberOfAvailableCars != null)
            {
                lblNumberOfAvailableCars.Text = numberOfAvailableCars.ToString();
                lblNumberOfAvailableVehicls.Text = numberOfAvailableCars.ToString();
                lblFleetStatusAvailable.Text = numberOfAvailableCars.ToString();
            }
            else
            {
                lblNumberOfAvailableCars.Text = "N/A";
                lblNumberOfAvailableVehicls.Text = "N/A";
                lblFleetStatusAvailable.Text = "N/A";
            }

            if (numberOfRentedCars != null)
            {
                lblNumberOfRentedCars.Text = numberOfRentedCars.ToString();
                lblFleetStatusRented.Text = numberOfRentedCars.ToString();
            }
            else
            {
                lblNumberOfRentedCars.Text = "N/A";
                lblFleetStatusRented.Text = "N/A";
            }


            if (numberOfMaintenanceCars != null)
            {
                lblNumberOfCarsInMaintenance.Text = numberOfMaintenanceCars.ToString();
                lblFleetStatusMaintenance.Text = numberOfMaintenanceCars.ToString();
            }
            else
            {
                lblNumberOfCarsInMaintenance.Text = "N/A";
                lblFleetStatusMaintenance.Text = "N/A";
            }

            if (numberOfReservedCars != null)
            {
                lblNumberOfReservedCars.Text = numberOfReservedCars.ToString();
                lblFleetStatusReserved.Text = numberOfReservedCars.ToString();
            }
            else
            {
                lblNumberOfReservedCars.Text = "N/A";
                lblFleetStatusReserved.Text = "N/A";
            }

            var growthRate = await PaymentsAPIs.CallGetTheGrowthRateWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token
                );

            if (growthRate != null)
                lblGrowthPercentageComapreWithLastYear.Text = growthRate.ToString();
            else
                lblGrowthPercentageComapreWithLastYear.Text = "N/A";

            var averageOfRevenue = await PaymentsAPIs.CallGetTheAverageOfRevenueThisYearWithAutoRefreshAsync(
                                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token);

            if (averageOfRevenue != null)
                lblMonthlyAverageRevenue.Text = "$" + Convert.ToDouble(averageOfRevenue).ToString("F2");
            else
                lblMonthlyAverageRevenue.Text = "N/A";


            var totalRevenue = await PaymentsAPIs.CallGetTheTotalRevenueThisYearWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    loggedInUser.Email,
                    loggedInUser.Token);

            if (totalRevenue != null)
                lblTotalAnnualRevenue.Text = "$" + Convert.ToDouble(totalRevenue).ToString("F2");
            else
                lblTotalAnnualRevenue.Text = "N/A";

            var totalRevenueThisMonth = await PaymentsAPIs.CallGetTheTotalRevenueInSpecificStatusWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    loggedInUser.Email,
                    loggedInUser.Token, "Completed");

            if (totalRevenueThisMonth != null)
                lblTotalRevenueThisMonth.Text = "$ " + Convert.ToDouble(totalRevenueThisMonth).ToString("F2");
            else
                lblTotalRevenueThisMonth.Text = "N/A";



            var totalRefundedThisMonth = await PaymentsAPIs.CallGetTheTotalRevenueInSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token, "Refunded");

            if (totalRefundedThisMonth != null)
                lblTotalRefunded.Text = "$ " + Convert.ToDouble(totalRefundedThisMonth).ToString("F2");
            else
                lblTotalRefunded.Text = "N/A";


            var numberOfRefundedTransactions = await PaymentsAPIs.CallGetNumberOfTransactionsInSpecificStatusWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email,
                loggedInUser.Token, "Refunded");

            if (numberOfRefundedTransactions != null)
                lblNumberOfRefundedTransactions.Text = numberOfRefundedTransactions.ToString() + " transactions";
            else
                lblNumberOfRefundedTransactions.Text = "N/A transactions";

            var revenueChangePercentageRate = await PaymentsAPIs.CallGetRevenueChangePercentageRateWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email, loggedInUser.Token);

            if (revenueChangePercentageRate != null)
                lblTotalRevenuePercentageCompareWithLastMonth.Text = revenueChangePercentageRate.Substring(1, revenueChangePercentageRate.Length - 2);
            else
                lblTotalRevenuePercentageCompareWithLastMonth.Text = "N/A";


            var RefundRateChange = await PaymentsAPIs.CallGetRefundRateChangeWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                loggedInUser.Email, loggedInUser.Token);

            if (RefundRateChange != null)
                lblRefundedPercentage.Text = RefundRateChange.Substring(1, RefundRateChange.Length - 2);
            else
                lblRefundedPercentage.Text = "N/A";

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMainDashboard_Click(object sender, EventArgs e)
        {
        }



        private async void guna2Button2_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Fleet;
            lblCaption.Text = "Management fleet";
            dgvFleet.DataSource = await _LoadCarsFleet();
        }

        private async void btnMainDashboard_Click_1(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.MainDashboard;
            lblCaption.Text = "Dashboard overview";
            //await _LoadKPIsInfo();
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Rentals;
            lblCaption.Text = "Management rentals";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Customers;
            lblCaption.Text = "Management customers";
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Payments;
            lblCaption.Text = "Management payments";
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Reports;
            lblCaption.Text = "Management reports";
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Settings;
            lblCaption.Text = "Management settings";
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            lblDateOfToday.Text = DateTime.Now.ToString("dd-MM-yyyy");
            cbVehicleStatus.SelectedIndex = 0;
            lblStartingEmail.Text = loggedInUser.Email[..2];
            lblAdminEmail.Text = loggedInUser.Email;
            lblLastYear_CurrentYear.Text = (DateTime.Now.Year - 1).ToString() + "-" + DateTime.Now.Year.ToString();

            dgvLastRentals.DataSource = await _LoadLastRentals();
            await _LoadKPIsInfo();
        }

        private async void btnManageFleet_Click(object sender, EventArgs e)
        {
            tcAdminTabs.SelectedIndex = (int)enAdminTabs.Fleet;
            lblCaption.Text = "Management fleet";
            dgvFleet.DataSource = await _LoadCarsFleet();
        }

        private void dgvLastRentals_Paint(object sender, PaintEventArgs e)
        {
            // Adjust the 20 value to control the roundness (higher number = more round)
            int radius = 15;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            // Define the rectangle based on the control's client area
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, this.dgvLastRentals.Width, this.dgvLastRentals.Height);

            // Add the rounded rectangle to the path
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // Apply the region to the control
            this.dgvLastRentals.Region = new System.Drawing.Region(path);
        }
        private void dgvFleet_Paint(object sender, PaintEventArgs e)
        {
            // Adjust the 20 value to control the roundness (higher number = more round)
            int radius = 15;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            // Define the rectangle based on the control's client area
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, this.dgvFleet.Width, this.dgvFleet.Height);

            // Add the rounded rectangle to the path
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // Apply the region to the control
            this.dgvFleet.Region = new System.Drawing.Region(path);
        }

        private async void btnAddNewVehicle_Click(object sender, EventArgs e)
        {
            frmAddEditVehicle addNewVehicle = new frmAddEditVehicle();
            addNewVehicle.ShowDialog();
            dgvFleet.DataSource = await _LoadCarsFleet();
            await _LoadKPIsInfo();
        }

        private void UpdateFleetStatusChart(int available, int rented, int maintenance, int reserved)
        {
            if (this.FleetStatusChart == null) return;

            try
            {
                // Update the theme properties BEFORE binding data
                var themeProp = FleetStatusChart.GetType().GetProperty("Theme");
                if (themeProp != null)
                {
                    var theme = themeProp.GetValue(FleetStatusChart);
                    if (theme != null)
                    {
                        var sliceColorsProp = theme.GetType().GetProperty("SliceColors");
                        if (sliceColorsProp != null)
                        {
                            var customColors = new List<Color>
                            {
                                Color.FromArgb(73, 195, 95),  // Green - Available
                                Color.FromArgb(34, 139, 230),  // Blue - Rented
                                Color.FromArgb(253, 126, 20), // Orange - Maintenance
                                Color.FromArgb(121, 80, 242)   // Purple - Reserved
                            };
                            sliceColorsProp.SetValue(theme, customColors);
                        }
                    }
                }

                // Build a DataTable (Siticone binds reliably to DataTable)
                var dt = new DataTable();
                dt.Columns.Add("Label", typeof(string));
                dt.Columns.Add("Value", typeof(int));

                dt.Rows.Add("Available", available);
                dt.Rows.Add("Rented", rented);
                dt.Rows.Add("Maintenance", maintenance);
                dt.Rows.Add("Reserved", reserved);

                // Assign DataSource + members and bind
                FleetStatusChart.DataSource = dt;
                FleetStatusChart.LabelMember = "Label";   // column name
                FleetStatusChart.ValueMember = "Value";   // column name
                FleetStatusChart.BackColor = Color.White;

                // Note: Call the control's DataBind method as listed in the debug output
                FleetStatusChart.GetType().GetMethod("DataBind")?.Invoke(FleetStatusChart, null);

                // Optional visual tweaks using reflection
                FleetStatusChart.GetType().GetProperty("ShowPercentageOnSlice")?.SetValue(FleetStatusChart, false);
                FleetStatusChart.GetType().GetProperty("EnableTooltips")?.SetValue(FleetStatusChart, true);
                FleetStatusChart.GetType().GetProperty("EnableAnimations")?.SetValue(FleetStatusChart, true);
                FleetStatusChart.GetType().GetProperty("CenterText")?.SetValue(FleetStatusChart, dt.AsEnumerable().Sum(r => r.Field<int>("Value")).ToString());
                FleetStatusChart.GetType().GetProperty("CenterSubText")?.SetValue(FleetStatusChart, "vehicles");

                // Ensure control repaints
                FleetStatusChart.Refresh();
                Debug.WriteLine("[FleetStatusChart] populated via DataTable + DataBind with Colors");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[FleetStatusChart] failed to populate chart: " + ex.Message);
                lblFleetStatusAvailable.Text = available.ToString();
                lblFleetStatusRented.Text = rented.ToString();
                lblFleetStatusMaintenance.Text = maintenance.ToString();
                lblFleetStatusReserved.Text = reserved.ToString();
            }
        }

        private async void txtSearchOnFleetGrid_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchOnFleetGrid.Text == "")
            {
                dgvFleet.DataSource = await _LoadCarsFleet();
                return;
            }

            ApplyFleetFilter(txtSearchOnFleetGrid.Text);
        }
        private void ApplyFleetFilter(string query)
        {

            if (_dt_Fleet == null)
            {
                dgvFleet.DataSource = null;
                return;
            }

            var dv = _dt_Fleet.DefaultView;
            if (string.IsNullOrWhiteSpace(query))
            {
                dv.RowFilter = string.Empty;
                dgvFleet.DataSource = dv;
                return;
            }

            // escape single quotes for RowFilter
            var q = query.Replace("'", "''");

            // Look for columns that actually exist in the table. Adjust naming if your DTO uses different casings.
            string[] preferred = { "brand", "model", "license_plate" };
            var columnsToFilter = preferred.Where(c => _dt_Fleet.Columns.Contains(c)).ToArray();

            // If the specific columns aren't found, try to search across all string columns
            if (columnsToFilter.Length == 0)
            {
                columnsToFilter = _dt_Fleet.Columns
                    .Cast<DataColumn>()
                    .Where(c => c.DataType == typeof(string))
                    .Select(c => c.ColumnName)
                    .ToArray();
            }

            var filterClauses = columnsToFilter.Select(col => $"CONVERT([{col}], 'System.String') LIKE '%{q}%'");
            dv.RowFilter = string.Join(" OR ", filterClauses);
            dgvFleet.DataSource = dv;
        }

        private async void cbVehicleStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchOnFleetGrid.Text = "";

            if (cbVehicleStatus.Text == "" || cbVehicleStatus.Text == "All status")
            {
                dgvFleet.DataSource = await _LoadCarsFleet();
                return;
            }
            else
                dgvFleet.DataSource = await _LoadCarsFleet(cbVehicleStatus.Text);


            ApplyFleetFilter(string.Empty);

        }

        private async void editCarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEditVehicle editVehicle = new frmAddEditVehicle(dgvFleet.SelectedRows[0].Cells["license_plate"].Value.ToString());
            editVehicle.ShowDialog();
            dgvFleet.DataSource = await _LoadCarsFleet();
            await _LoadKPIsInfo();
        }

        private async void deleteCarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRemoveVehicle removeVehicle  = new frmRemoveVehicle(dgvFleet.SelectedRows[0].Cells["license_plate"].Value.ToString());
            removeVehicle.ShowDialog();
            dgvFleet.DataSource = await _LoadCarsFleet();
            await _LoadKPIsInfo();
        }
    }
}
