using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ClientApp
{
    public partial class AdminForm : Form
    {
        private readonly string connStr =
            "Server=127.0.0.1;Port=3306;Database=chatdb;Uid=root;Pwd=200219;";

        private readonly ChatClient _client;
        private readonly string _username;

        public AdminForm(ChatClient client, string username)
        {
            InitializeComponent();
            _client = client;
            _username = username;

            StyleGrid(dgvUsers);

            LoadUsers();
        }

        // === STYLE GRID ===
        private void StyleGrid(DataGridView grid)
        {
            // ===== GRID CHUNG =====
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Color.FromArgb(250, 252, 255);
            grid.GridColor = Color.FromArgb(210, 210, 210);
            grid.EnableHeadersVisualStyles = false;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;

            // phân cách dòng
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // ===== HEADER =====
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 237, 240); // xám nhạt sang trọng
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Viền dưới header (đậm hơn xíu cho đẹp)
            grid.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;

            grid.ColumnHeadersHeight = 42;

            // ===== DÒNG DỮ LIỆU =====
            grid.DefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // màu khi chọn
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 240);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            // ===== TẮT XEN KẼ (để 3 dòng cùng màu) =====
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
        }






        // === LOAD USERS ===
        private void LoadUsers(string keyword = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT id AS 'ID', username AS 'Tài khoản', password AS 'Mật khẩu', role AS 'Vai trò' FROM users";
                    if (!string.IsNullOrEmpty(keyword))
                        sql += " WHERE username LIKE @kw";
                    sql += " ORDER BY id ASC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        using (MySqlDataAdapter adp = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            dgvUsers.DataSource = dt;
                        }
                    }
                }

                lblUserInfo.Text = $"👤 Tổng tài khoản: {dgvUsers.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải danh sách:\n" + ex.Message);
            }
        }

        // === SEARCH USER ===
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        // === REFRESH ===
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadUsers();
        }

        // === ADD USER ===
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string username = Prompt.ShowDialog("Nhập tên tài khoản mới:", "Thêm user");
            if (string.IsNullOrWhiteSpace(username)) return;

            string password = Prompt.ShowDialog("Nhập mật khẩu:", "Thêm user");
            if (string.IsNullOrWhiteSpace(password)) return;

            string role = Prompt.ShowDialog("Nhập vai trò (admin/user):", "Thêm user");
            if (string.IsNullOrWhiteSpace(role)) role = "user";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO users (username, password, role) VALUES (@u, @p, @r)", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.Parameters.AddWithValue("@r", role.ToLower());
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadUsers();
                MessageBox.Show("✅ Thêm user thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi thêm user:\n" + ex.Message);
            }
        }

        // === EDIT USER ===
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ Hãy chọn 1 user để sửa!");
                return;
            }

            string username = dgvUsers.SelectedRows[0].Cells["Tài khoản"].Value.ToString();
            string newPass = Prompt.ShowDialog("Nhập mật khẩu mới (để trống nếu giữ nguyên):", "Sửa user");
            string newRole = Prompt.ShowDialog("Nhập vai trò mới (admin/user, để trống nếu giữ nguyên):", "Sửa user");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE users SET ";
                    if (!string.IsNullOrEmpty(newPass)) sql += "password=@p, ";
                    if (!string.IsNullOrEmpty(newRole)) sql += "role=@r, ";
                    sql = sql.TrimEnd(',', ' ') + " WHERE username=@u";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(newPass)) cmd.Parameters.AddWithValue("@p", newPass);
                        if (!string.IsNullOrEmpty(newRole)) cmd.Parameters.AddWithValue("@r", newRole.ToLower());
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadUsers();
                MessageBox.Show("✅ Sửa thông tin user thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi sửa user:\n" + ex.Message);
            }
        }

        // === DELETE USER ===
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ Hãy chọn ít nhất một user để xóa!");
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa user này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    foreach (DataGridViewRow row in dgvUsers.SelectedRows)
                    {
                        string username = row.Cells["Tài khoản"].Value.ToString();
                        if (username.ToLower() == "admin" || username == _username)
                        {
                            MessageBox.Show($"⛔ Không thể xóa tài khoản '{username}'!");
                            continue;
                        }

                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM users WHERE username=@u", conn))
                        {
                            cmd.Parameters.AddWithValue("@u", username);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                LoadUsers();
                MessageBox.Show("✅ Đã xóa user thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi xóa user:\n" + ex.Message);
            }
        }

        // === LOGOUT ===
        private void btnLogout_Click(object sender, EventArgs e)
        {
            _client?.Close();
            new LoginForm().Show();
            this.Close();
        }

        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Close();
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    // --- Class hỗ trợ nhập input nhỏ ---
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(18, 22, 27),
                ForeColor = Color.White
            };

            Label lblText = new Label() { Left = 20, Top = 20, Text = text, Width = 340 };
            TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340, ForeColor = Color.White, BackColor = Color.FromArgb(30, 35, 40) };
            Button confirmation = new Button() { Text = "OK", Left = 140, Width = 100, Top = 90, DialogResult = DialogResult.OK, BackColor = Color.FromArgb(0, 200, 150), FlatStyle = FlatStyle.Flat };

            confirmation.FlatAppearance.BorderSize = 0;
            prompt.Controls.Add(lblText);
            prompt.Controls.Add(txtInput);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? txtInput.Text : "";
        }
    }
}
