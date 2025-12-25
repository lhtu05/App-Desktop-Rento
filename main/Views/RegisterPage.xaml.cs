using main.Data;
using main.Models;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class RegisterPage : UserControl
    {
        private DatabaseHelper _dbHelper;
        private MainWindow _mainWindow;

        public RegisterPage(DatabaseHelper dbHelper , MainWindow mainWindow)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
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

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

                User newUser = new User
                {
                    FullName = txtFullName.Text,
                    Username = txtUsername.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    PasswordHash = txtPassword.Password,   // sau này đổi sang hash
                };

                if (_dbHelper.Register(newUser, txtPassword.Password))
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
            using (var conn = _dbHelper.Connection)
            {
                string sql = @"
                INSERT INTO Property (HostID, WardID, Title, Address, Price, Description, Latitude, Longitude, Status, CreatedAt)
                VALUES (@HostID, @WardID, @Title, @Address, @Price, @Description,@Latitude,@Longitude, 'AVAILABLE', GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";

                //int newPropertyId = conn.QuerySingle<int>(sql, room);

                //MessageBox.Show($"Đăng phòng thành công! ID mới: {newPropertyId}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang đăng nhập
            _mainWindow.NavigateToLoginPage();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang đăng nhập
            _mainWindow.NavigateToLoginPage();
        }
    }
}