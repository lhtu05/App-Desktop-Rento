using main.Data;
using main.Models;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class HostRegisterPage : UserControl
    {
        private DatabaseHelper _dbHelper;
        private MainWindow _mainWindow;

        public HostRegisterPage(DatabaseHelper dbHelper , MainWindow mainWindow)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
        }

        private void BtnHostRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text) ||
                string.IsNullOrEmpty(txtUsername.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (txtPassword.Password.Length < 6)
            //{
            //    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            try
            {
                //if (_dbHelper.IsUsernameExists(txtUsername.Text))
                //{
                //    MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi",
                //        MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                //if (_dbHelper.IsEmailExists(txtEmail.Text))
                //{
                //    MessageBox.Show("Email đã tồn tại!", "Lỗi",
                //        MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}
                Account newAccount = new Account
                {
                    UserName = txtUsername.Text,
                    PasswordHash = txtPassword.Password, // sau này đổi sang hash
                    Host = new Host
                    {
                        FullName = txtFullName.Text,
                        Email = txtEmail.Text,
                        PhoneNumber = txtPhone.Text,
                    },
                    Role = true // Host
                };
                

                if (_dbHelper.Register(newAccount))
                {
                    MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Quay lại trang đăng nhập
                    _mainWindow.NavigateToLoginPage();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            // Quay lại trang đăng nhập
            _mainWindow.NavigateToMainPage();
        }
    }
}