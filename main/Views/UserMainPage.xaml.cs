using main.Models;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class UserMainPage : UserControl
    {
        private User _currentUser;
        private MainWindow _mainWindow;

        public UserMainPage(User User, MainWindow mainWindow)
        {
            InitializeComponent();
            _currentUser = User;
            _mainWindow = mainWindow;
            UpdateWelcomeMessage();
        }

        private void UpdateWelcomeMessage()
        {
            //txtWelcome.Text = $"Xin chào, {_currentUser.FullName} ({_currentUser.UserType})";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Quay lại trang đăng nhập
                _mainWindow.NavigateToLoginPage();
            }
        }
    }
}