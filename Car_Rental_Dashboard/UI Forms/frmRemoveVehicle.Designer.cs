namespace Car_Rental_Dashboard
{
    partial class frmRemoveVehicle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            lblRemoveMessage = new Label();
            lblCaption = new Label();
            btnRemoveVehicle = new Guna.UI2.WinForms.Guna2GradientButton();
            btnCancelRemove = new Guna.UI2.WinForms.Guna2GradientButton();
            guna2CirclePictureBox2 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox2).BeginInit();
            SuspendLayout();
            // 
            // guna2CirclePictureBox1
            // 
            guna2CirclePictureBox1.BackColor = Color.FromArgb(255, 228, 230);
            guna2CirclePictureBox1.FillColor = Color.FromArgb(255, 228, 230);
            guna2CirclePictureBox1.Image = Properties.Resources.icons8_delete_50;
            guna2CirclePictureBox1.ImageRotate = 0F;
            guna2CirclePictureBox1.Location = new Point(216, 55);
            guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges1;
            guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CirclePictureBox1.Size = new Size(63, 63);
            guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            guna2CirclePictureBox1.TabIndex = 0;
            guna2CirclePictureBox1.TabStop = false;
            // 
            // lblRemoveMessage
            // 
            lblRemoveMessage.AutoSize = true;
            lblRemoveMessage.BackColor = Color.White;
            lblRemoveMessage.Font = new Font("Segoe UI", 9F);
            lblRemoveMessage.ForeColor = Color.Gray;
            lblRemoveMessage.Location = new Point(62, 180);
            lblRemoveMessage.Name = "lblRemoveMessage";
            lblRemoveMessage.Size = new Size(393, 20);
            lblRemoveMessage.TabIndex = 27;
            lblRemoveMessage.Text = "This will permanently remove the ABC-1234 form the fleet";
            // 
            // lblCaption
            // 
            lblCaption.AutoSize = true;
            lblCaption.BackColor = Color.White;
            lblCaption.Font = new Font("Segoe UI", 11.3F, FontStyle.Bold);
            lblCaption.ForeColor = Color.Black;
            lblCaption.Location = new Point(169, 147);
            lblCaption.Name = "lblCaption";
            lblCaption.Size = new Size(163, 25);
            lblCaption.TabIndex = 26;
            lblCaption.Text = "Remove Vehicle ?";
            // 
            // btnRemoveVehicle
            // 
            btnRemoveVehicle.Animated = true;
            btnRemoveVehicle.BackColor = Color.Transparent;
            btnRemoveVehicle.BorderRadius = 11;
            btnRemoveVehicle.CustomizableEdges = customizableEdges2;
            btnRemoveVehicle.DisabledState.BorderColor = Color.DarkGray;
            btnRemoveVehicle.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRemoveVehicle.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRemoveVehicle.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            btnRemoveVehicle.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRemoveVehicle.FillColor = Color.FromArgb(219, 32, 70);
            btnRemoveVehicle.FillColor2 = Color.FromArgb(218, 15, 20);
            btnRemoveVehicle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRemoveVehicle.ForeColor = Color.White;
            btnRemoveVehicle.Location = new Point(260, 225);
            btnRemoveVehicle.Name = "btnRemoveVehicle";
            btnRemoveVehicle.ShadowDecoration.Color = Color.Silver;
            btnRemoveVehicle.ShadowDecoration.CustomizableEdges = customizableEdges3;
            btnRemoveVehicle.ShadowDecoration.Depth = 12;
            btnRemoveVehicle.ShadowDecoration.Enabled = true;
            btnRemoveVehicle.Size = new Size(175, 56);
            btnRemoveVehicle.TabIndex = 42;
            btnRemoveVehicle.Text = "Remove";
            btnRemoveVehicle.Click += btnAddOrEditVehicle_Click;
            // 
            // btnCancelRemove
            // 
            btnCancelRemove.Animated = true;
            btnCancelRemove.BackColor = Color.Transparent;
            btnCancelRemove.BorderColor = Color.FromArgb(217, 221, 226);
            btnCancelRemove.BorderRadius = 11;
            btnCancelRemove.BorderThickness = 1;
            btnCancelRemove.CustomizableEdges = customizableEdges4;
            btnCancelRemove.DisabledState.BorderColor = Color.DarkGray;
            btnCancelRemove.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCancelRemove.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCancelRemove.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            btnCancelRemove.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCancelRemove.FillColor = Color.White;
            btnCancelRemove.FillColor2 = Color.White;
            btnCancelRemove.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancelRemove.ForeColor = Color.FromArgb(64, 64, 64);
            btnCancelRemove.Location = new Point(79, 225);
            btnCancelRemove.Name = "btnCancelRemove";
            btnCancelRemove.ShadowDecoration.Color = Color.Silver;
            btnCancelRemove.ShadowDecoration.CustomizableEdges = customizableEdges5;
            btnCancelRemove.ShadowDecoration.Depth = 10;
            btnCancelRemove.ShadowDecoration.Enabled = true;
            btnCancelRemove.Size = new Size(175, 56);
            btnCancelRemove.TabIndex = 43;
            btnCancelRemove.Text = "Cancel";
            btnCancelRemove.Click += btnCancel_Click;
            // 
            // guna2CirclePictureBox2
            // 
            guna2CirclePictureBox2.BackColor = Color.White;
            guna2CirclePictureBox2.FillColor = Color.FromArgb(255, 228, 230);
            guna2CirclePictureBox2.ImageRotate = 0F;
            guna2CirclePictureBox2.Location = new Point(197, 35);
            guna2CirclePictureBox2.Name = "guna2CirclePictureBox2";
            guna2CirclePictureBox2.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2CirclePictureBox2.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CirclePictureBox2.Size = new Size(101, 104);
            guna2CirclePictureBox2.TabIndex = 44;
            guna2CirclePictureBox2.TabStop = false;
            // 
            // frmRemoveVehicle
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(499, 333);
            Controls.Add(btnCancelRemove);
            Controls.Add(btnRemoveVehicle);
            Controls.Add(lblRemoveMessage);
            Controls.Add(lblCaption);
            Controls.Add(guna2CirclePictureBox1);
            Controls.Add(guna2CirclePictureBox2);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmRemoveVehicle";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmRemoveVehicle";
            Load += frmRemoveVehicle_Load;
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Label lblRemoveMessage;
        private Label lblCaption;
        private Guna.UI2.WinForms.Guna2GradientButton btnRemoveVehicle;
        private Guna.UI2.WinForms.Guna2GradientButton btnCancelRemove;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox2;
    }
}