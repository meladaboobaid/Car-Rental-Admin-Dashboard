using Car_Rental_Dashboard.APIs_Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Car_Rental_Dashboard
{
    public partial class frmRemoveVehicle : Form
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
            SetFormRoundedRegion(36); // Change yhe number to adjust the roundness
        }

        // 4. Update the region if the form is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetFormRoundedRegion(30); // Re-apply the rounded shape
        }


        private string? _LicensePlate = string.Empty;
        private int? _VehicleID = null;

        public frmRemoveVehicle(string? LicensePlate)
        {
            InitializeComponent();
            _LicensePlate = LicensePlate;

        }

        private async void frmRemoveVehicle_Load(object sender, EventArgs e)
        {
            lblRemoveMessage.Text = $"This will permanently remove {_LicensePlate} from the fleet";

            var carInfo = await CarsAPI.CallGetCarByLicensePlateAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    _LicensePlate);

            if (carInfo != null)
            {
                _VehicleID = carInfo.id;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnAddOrEditVehicle_Click(object sender, EventArgs e)
        {
            if (_VehicleID != null || _VehicleID == -1)
            {

                var deleteResult = await CarsAPI.CallDeleteCar_WithAutoRefreshAsync(
                    UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl),
                    Global.CurrentUser.Email,
                    Global.CurrentUser.Token,
                    _VehicleID);

                if(deleteResult != null)
                {

                    if (!deleteResult.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Car deleted failed, may this car should not be deleted", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Car deleted successfully", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Car deleted failed, may this car should not be deleted", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

            }
        }
    }
}
