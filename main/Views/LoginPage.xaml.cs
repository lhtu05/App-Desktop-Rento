using main.Data;
using main.Models;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class LoginPage : UserControl
    {
        private DatabaseHelper _dbHelper;
        private MainWindow _mainWindow;

        public LoginPage(DatabaseHelper dbHelper, MainWindow mainWindow)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string UserName = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Account account = _dbHelper.Login(UserName, password);
                if (account != null && !account.Role)
                {
                    Renter renter = account.Renter;

                    MessageBox.Show($"Đăng nhập thành công! Chào mừng {renter.FullName}",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Điều hướng đến trang chính
                    _mainWindow.NavigateToRenterMainPage(renter);
                }
                else if (account != null && account.Role)
                {
                    Host host = account.Host;
                    MessageBox.Show($"Đăng nhập thành công! Chào mừng {host.FullName}",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Điều hướng đến trang chính
                    _mainWindow.NavigateToHostMainPage(host);
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang đăng ký
            _mainWindow.NavigateToRenterRegisterPage();
        }
    }
}