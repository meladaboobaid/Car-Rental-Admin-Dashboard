using Car_Rental_Dashboard.APIs_Client;
using Car_Rental_Dashboard.DTOs.Auth;
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
    public partial class frmLogin : Form
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
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            btnSignIn.Enabled = false;
            using var http = UsersAPIs.CreateHttpClientForLocalDev(UsersAPIs.BaseUrl);

            try
            {
                // 10s client-side timeout
                const int timeoutMs = 10_000;
                var token = await UsersAPIs.LoginAsync(http, txtEmail.Text.Trim(), txtPassword.Text, timeoutMs);
                if (token == null)
                {
                    MessageBox.Show("Login failed: invalid credentials or timeout.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Global.CurrentUser = new LoggedInUser
                {
                    Email = txtEmail.Text.Trim(),
                    Token = new TokenState
                    {
                        AccessToken = token.AccessToken,
                        RefreshToken = token.RefreshToken
                    }
                };

                // Example: open dashboard
                frmAdminDashboard main = new frmAdminDashboard(Global.CurrentUser);
                this.Hide();
                main.ShowDialog();
                this.Show();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Login timed out or canceled.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSignIn.Enabled = true;
            }
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            if(!txtPassword.UseSystemPasswordChar)
            {
                txtPassword.UseSystemPasswordChar = true;
                txtPassword.IconRight = Properties.Resources.icons8_closed_eye_30;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.IconRight = Properties.Resources.icons8_eye_30;
            }
        }
    }
}
