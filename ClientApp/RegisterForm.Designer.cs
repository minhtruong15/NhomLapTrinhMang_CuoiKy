namespace ClientApp
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.LinkLabel linkBack;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelCard = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.linkBack = new System.Windows.Forms.LinkLabel();
            this.btnRegister = new System.Windows.Forms.Button();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCard
            // 
            this.panelCard.BackColor = System.Drawing.Color.White;
            this.panelCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelCard.Controls.Add(this.lblStatus);
            this.panelCard.Controls.Add(this.linkBack);
            this.panelCard.Controls.Add(this.btnRegister);
            this.panelCard.Controls.Add(this.chkShowPassword);
            this.panelCard.Controls.Add(this.txtPass);
            this.panelCard.Controls.Add(this.txtUser);
            this.panelCard.Controls.Add(this.lblPass);
            this.panelCard.Controls.Add(this.lblUser);
            this.panelCard.Controls.Add(this.lblSubtitle);
            this.panelCard.Controls.Add(this.lblTitle);
            this.panelCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCard.Location = new System.Drawing.Point(0, 0);
            this.panelCard.Name = "panelCard";
            this.panelCard.Padding = new System.Windows.Forms.Padding(80, 60, 80, 60);
            this.panelCard.Size = new System.Drawing.Size(960, 640);
            this.panelCard.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(248, 113, 113);
            this.lblStatus.Location = new System.Drawing.Point(80, 520);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            this.lblStatus.TabIndex = 9;
            // 
            // linkBack
            // 
            this.linkBack.ActiveLinkColor = System.Drawing.Color.FromArgb(79, 70, 229);
            this.linkBack.AutoSize = true;
            this.linkBack.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkBack.LinkColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.linkBack.Location = new System.Drawing.Point(80, 485);
            this.linkBack.Name = "linkBack";
            this.linkBack.Size = new System.Drawing.Size(149, 20);
            this.linkBack.TabIndex = 8;
            this.linkBack.TabStop = true;
            this.linkBack.Text = "Quay lại đăng nhập";
            this.linkBack.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBack_LinkClicked);
            // 
            // btnRegister
            // 
            this.btnRegister.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(79, 70, 229);
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(80, 420);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(800, 50);
            this.btnRegister.TabIndex = 7;
            this.btnRegister.Text = "Tạo tài khoản";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkShowPassword.ForeColor = System.Drawing.Color.DimGray;
            this.chkShowPassword.Location = new System.Drawing.Point(80, 360);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(143, 24);
            this.chkShowPassword.TabIndex = 6;
            this.chkShowPassword.Text = "Hiển thị mật khẩu";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);
            // 
            // txtPass
            // 
            this.txtPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPass.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPass.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPass.Location = new System.Drawing.Point(80, 320);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '●';
            this.txtPass.Size = new System.Drawing.Size(800, 32);
            this.txtPass.TabIndex = 5;
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUser.Location = new System.Drawing.Point(80, 240);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(800, 32);
            this.txtUser.TabIndex = 3;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblPass.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblPass.Location = new System.Drawing.Point(80, 290);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(77, 23);
            this.lblPass.TabIndex = 4;
            this.lblPass.Text = "Mật khẩu";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblUser.Location = new System.Drawing.Point(80, 210);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(80, 23);
            this.lblUser.TabIndex = 2;
            this.lblUser.Text = "Tài khoản";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(99, 102, 241);
            this.lblSubtitle.Location = new System.Drawing.Point(80, 145);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(234, 23);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Tạo tài khoản mới trong vài giây";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(29, 78, 137);
            this.lblTitle.Location = new System.Drawing.Point(75, 80);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(307, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Bắt đầu ngay";
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(232, 244, 255);
            this.ClientSize = new System.Drawing.Size(960, 640);
            this.Controls.Add(this.panelCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(860, 560);
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng ký tài khoản";
            this.panelCard.ResumeLayout(false);
            this.panelCard.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}



