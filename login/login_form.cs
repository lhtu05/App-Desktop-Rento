using RentalPlatform.Data;
using RentalPlatform.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RentalPlatform.Forms
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper dbHelper;

        public LoginForm()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Đăng nhập - Rental Platform";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Username Label
            Label lblUsername = new Label();
            lblUsername.Text = "Tên đăng nhập:";
            lblUsername.Location = new System.Drawing.Point(50, 50);
            lblUsername.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblUsername);

            // Username TextBox
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new System.Drawing.Point(150, 50);
            txtUsername.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(txtUsername);

            // Password Label
            Label lblPassword = new Label();
            lblPassword.Text = "Mật khẩu:";
            lblPassword.Location = new System.Drawing.Point(50, 90);
            lblPassword.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblPassword);

            // Password TextBox
            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new System.Drawing.Point(150, 90);
            txtPassword.Size = new System.Drawing.Size(200, 20);
            txtPassword.UseSystemPasswordChar = true;
            this.Controls.Add(txtPassword);

            // Login Button
            Button btnLogin = new Button();
            btnLogin.Text = "Đăng nhập";
            btnLogin.Location = new System.Drawing.Point(150, 130);
            btnLogin.Size = new System.Drawing.Size(100, 30);
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // Register Button
            Button btnRegister = new Button();
            btnRegister.Text = "Đăng ký tài khoản";
            btnRegister.Location = new System.Drawing.Point(150, 170);
            btnRegister.Size = new System.Drawing.Size(100, 30);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            this.ResumeLayout(false);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var txtUsername = this.Controls.Find("txtUsername", true)[0] as TextBox;
            var txtPassword = this.Controls.Find("txtPassword", true)[0] as TextBox;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User user = dbHelper.Login(username, password);
                if (user != null)
                {
                    MessageBox.Show($"Đăng nhập thành công! Chào mừng {user.FullName}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form chính
                    MainForm mainForm = new MainForm(user);
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}