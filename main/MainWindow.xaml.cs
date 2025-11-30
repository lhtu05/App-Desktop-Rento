using main.Data;
using main.Models;
using main.Views;
using System.Windows;
using System.Windows.Controls;

namespace main
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainPage MainPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper("Server=.;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

            MainPage = new MainPage(this);
            MainFrame.Content = MainPage;

            // Load trang chính đầu tiên
            NavigateToMainPage();
        }

        // Phương thức điều hướng đến trang chính
        public void NavigateToMainPage()
        {
            var mainPage = new MainPage(this);
            MainFrame.Navigate(mainPage);
        }

        // Phương thức điều hướng đến trang đăng nhập
        public void NavigateToLoginPage()
        {
            var loginPage = new LoginPage(dbHelper, this);
            MainFrame.Navigate(loginPage);
        }

        public void NavigateToPostWindow()
        {
            var postWindow = new PostWindow(dbHelper, this);
            postWindow.Owner = this;
            postWindow.Show(); // or ShowDialog();
        }


        // Phương thức điều hướng đến trang đăng ký
        public void NavigateToRegisterPage()
        {
            var registerPage = new RegisterPage(dbHelper, this);
            MainFrame.Navigate(registerPage);
        }

        // Phương thức điều hướng đến trang chính sau khi đăng nhập thành công
        public void NavigateToUserMainPage(User user)
        {
            var userMainPage = new UserMainPage(user, this);
            MainFrame.Navigate(userMainPage);
        }

        public void NavigateToBookingPage(int cityId)
        {
            var bookingPage = new BookingPage(dbHelper, this, cityId);
            MainFrame.Navigate(bookingPage);
        }
    }
}