using Car_Rental_Dashboard.APIs_Client;
using Car_Rental_Dashboard.DTOs.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Car_Rental_Dashboard
{
    public partial class frmAddEditVehicle : Form
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
            SetFormRoundedRegion(36); // Change the number to adjust the roundness
        }

        // 4. Update the region if the form is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetFormRoundedRegion(30); // Re-apply the rounded shape
        }

        enum enAddOrEdit { AddNew = 1, Edit = 2 }

        enAddOrEdit _AddOrEdit;
        int _vehicleId = -1;
        CarDTO? _carDTO;
        string? _LicensePlate = string.Empty;

        string[] Car_statuses = ["available", "rented", "reserved", "maintenance"];

        public frmAddEditVehicle()
        {
            InitializeComponent();

            //if(VehicleID == -1)
            //    _AddOrEdit = enAddOrEdit.AddNew;
            //else
            //    _AddOrEdit = enAddOrEdit.Edit;

            _AddOrEdit = enAddOrEdit.AddNew;

        }

        public frmAddEditVehicle(string? LicensePlate)
        {
            InitializeComponent();

            _LicensePlate = LicensePlate;
            _AddOrEdit = enAddOrEdit.Edit;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void _LoadCarsCategory()
        {
            DataTable? dtCategory = await CarTypesAPIs.CallGetCarsCategoryWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                Global.CurrentUser.Email,
                Global.CurrentUser.Token);

            foreach (DataRow row in dtCategory.Rows)
            {
                cbCategory.Items.Add(row["name"]);
            }

            cbCategory.SelectedIndex = 0;
        }

        private async void _LoadCompanies()
        {
            DataTable? dtCompanies = await CompaniesAPIs.CallGetAllCompaniesWithAutoRefreshAsync(
                UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                Global.CurrentUser.Email,
                Global.CurrentUser.Token);


            foreach (DataRow row in dtCompanies.Rows)
            {
                cbCompanies.Items.Add(row["company_name"]);
            }

            cbCompanies.SelectedIndex = 0;
        }

        private async Task _LoadVehicleInfo()
        {
            _LoadCompanies();
            _LoadCarsCategory();
            _LoadCarsStatuses();

            if (_AddOrEdit == enAddOrEdit.AddNew)
            {
                lblCaption.Text = "Add New Vehicle";
                lblSubCaption.Text = "Enter vehicle details below";
                btnAddOrEditVehicle.Text = "Add Vehicle";
                return;
            }

            lblCaption.Text = "Edit Vehicle";
            lblSubCaption.Text = _LicensePlate; // replace it with the real car license plate
            btnAddOrEditVehicle.Text = "Save changes";

            var carInfo = await CarsAPI.CallGetCarByLicensePlateAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    _LicensePlate);

            if(carInfo != null)
            {
                _vehicleId = carInfo.id;

                var carTypeName = await CarTypesAPIs.CallGetCarTypeByID_WithAutoRefreshAsync(
                        UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                        Global.CurrentUser.Email,
                        Global.CurrentUser.Token,
                        carInfo.car_type_id);

                var companyName = await CompaniesAPIs.CallGetCompanyNameByID_WithAutoRefreshAsync(
                        UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                        Global.CurrentUser.Email,
                        Global.CurrentUser.Token,
                        carInfo.company_id);


                txtBrand.Text = carInfo.brand;
                txtModel.Text = carInfo.model;
                txtYear.Text = carInfo.year.ToString();
                txtLicensePlate.Text = carInfo.license_plate;
                txtColor.Text = carInfo.color;
                txtDailyRatePerDollar.Text = carInfo.daily_rate.ToString();
                txtMileage.Text = carInfo.mileage.ToString();
                cbCategory.Text = carTypeName;
                cbCompanies.Text = companyName;
                cbCarStatus.Text = carInfo.status;
            }
            else
            {
                MessageBox.Show("Failed to load vehicle information. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }


        private void _LoadCarsStatuses()
        {
            foreach(var s in Car_statuses)
            {
                cbCarStatus.Items.Add(s);
            }

        }
        private async void frmAddEditVehicle_Load(object sender, EventArgs e)
        {
            // Load vehicle information if we are in edit mode
            await _LoadVehicleInfo();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnAddOrEditVehicle_Click(object sender, EventArgs e)
        {
            if(_AddOrEdit == enAddOrEdit.AddNew)
            {

                var carTypeID = await CarTypesAPIs.CallGetCarTypeIdByNameWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    cbCategory.Text.Trim());


                var companyID = await CompaniesAPIs.CallGetCompanyIdByNameWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    cbCompanies.Text.Trim());


                // car instance initializing
                _carDTO = new CarDTO
                {
                    id = 0,
                    company_id = Convert.ToInt32(companyID),
                    car_type_id = Convert.ToInt32(carTypeID),
                    location_id = 1,
                    brand = txtBrand.Text.Trim(),
                    model = txtModel.Text.Trim(),
                    year = Convert.ToInt32(txtYear.Text.Trim()),
                    license_plate = txtLicensePlate.Text.Trim(),
                    color = txtColor.Text.Trim(),
                    daily_rate = Convert.ToDouble(txtDailyRatePerDollar.Text.Trim()),
                    mileage = Convert.ToDouble(txtMileage.Text.Trim()),
                    status = cbCarStatus.Text.Trim(),
                };

                var addCarResult = await CarsAPI.CallAddCarWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    _carDTO);

                if (!addCarResult.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to add car. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Car added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            else
            {
                var carTypeID = await CarTypesAPIs.CallGetCarTypeIdByNameWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    cbCategory.Text.Trim());


                var companyID = await CompaniesAPIs.CallGetCompanyIdByNameWithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    cbCompanies.Text.Trim());


                _carDTO = new CarDTO
                {
                    id = _vehicleId,
                    company_id = Convert.ToInt32(companyID),
                    car_type_id = Convert.ToInt32(carTypeID),
                    location_id = 1,
                    brand = txtBrand.Text.Trim(),
                    model = txtModel.Text.Trim(),
                    year = Convert.ToInt32(txtYear.Text.Trim()),
                    license_plate = txtLicensePlate.Text.Trim(),
                    color = txtColor.Text.Trim(),
                    daily_rate = Convert.ToDouble(txtDailyRatePerDollar.Text.Trim()),
                    mileage = Convert.ToDouble(txtMileage.Text.Trim()),
                    status = cbCarStatus.Text.Trim(),
                };

                var updateCarResult = await CarsAPI.CallUpdateCar_WithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    _carDTO, _vehicleId);


                if (!updateCarResult.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to update car. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Car updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }
    }
}
