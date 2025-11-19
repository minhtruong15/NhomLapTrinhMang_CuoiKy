namespace ClientApp
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblRoom = new System.Windows.Forms.Label();
            this.flowMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.messageBox = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.headerPanel.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.messageBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.headerPanel.Controls.Add(this.lblRoom);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.headerPanel.Size = new System.Drawing.Size(1040, 72);
            this.headerPanel.TabIndex = 0;
            // 
            // lblRoom
            // 
            this.lblRoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRoom.Font = new System.Drawing.Font("Segoe UI Semibold", 18F);
            this.lblRoom.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblRoom.Location = new System.Drawing.Point(20, 10);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(1000, 52);
            this.lblRoom.TabIndex = 0;
            this.lblRoom.Text = "Đang trò chuyện";
            this.lblRoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowMessages
            // 
            this.flowMessages.AutoScroll = true;
            this.flowMessages.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.flowMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMessages.Location = new System.Drawing.Point(0, 72);
            this.flowMessages.Margin = new System.Windows.Forms.Padding(0);
            this.flowMessages.Name = "flowMessages";
            this.flowMessages.Padding = new System.Windows.Forms.Padding(36, 28, 36, 36);
            this.flowMessages.Size = new System.Drawing.Size(1040, 548);
            this.flowMessages.TabIndex = 1;
            this.flowMessages.WrapContents = false;
            // 
            // inputPanel
            // 
            this.inputPanel.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.inputPanel.Controls.Add(this.messageBox);
            this.inputPanel.Controls.Add(this.btnSend);
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 620);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Padding = new System.Windows.Forms.Padding(32, 16, 32, 22);
            this.inputPanel.Size = new System.Drawing.Size(1040, 140);
            this.inputPanel.TabIndex = 2;
            // 
            // messageBox
            // 
            this.messageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBox.BackColor = System.Drawing.Color.White;
            this.messageBox.Controls.Add(this.txtMessage);
            this.messageBox.Location = new System.Drawing.Point(12, 18);
            this.messageBox.Name = "messageBox";
            this.messageBox.Padding = new System.Windows.Forms.Padding(18, 14, 18, 14);
            this.messageBox.Size = new System.Drawing.Size(783, 96);
            this.messageBox.TabIndex = 3;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.White;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtMessage.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txtMessage.Location = new System.Drawing.Point(18, 14);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(747, 68);
            this.txtMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(817, 18);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(197, 96);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(229, 239, 255);
            this.ClientSize = new System.Drawing.Size(1040, 760);
            this.Controls.Add(this.flowMessages);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.inputPanel);
            this.MinimumSize = new System.Drawing.Size(1024, 720);
            this.Name = "ChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trò chuyện";
            this.headerPanel.ResumeLayout(false);
            this.inputPanel.ResumeLayout(false);
            this.messageBox.ResumeLayout(false);
            this.messageBox.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.FlowLayoutPanel flowMessages;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel messageBox;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
    }
}
