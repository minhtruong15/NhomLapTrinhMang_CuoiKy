namespace ServerApp
{
    partial class ServerMainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(120, 340);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(180, 35);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Khởi động Server";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Firebrick;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(400, 340);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(180, 35);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Dừng Server";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtLog.ForeColor = System.Drawing.Color.Black;
            this.txtLog.Location = new System.Drawing.Point(20, 20);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(660, 300);
            this.txtLog.TabIndex = 0;
            // 
            // ServerMainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Name = "ServerMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "💬 Server Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
