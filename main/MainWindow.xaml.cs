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

            MainPage = new MainPage(this);
            MainFrame.Content = MainPage;
            dbHelper = new DatabaseHelper("Server=.\\SQLEXPRESS;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

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

        public void NavigateToPostPage()
        {
            var postPage = new PostPage(dbHelper, this);
            MainFrame.Navigate(postPage);
        }


        // Phương thức điều hướng đến trang đăng ký
        public void NavigateToRegisterPage()
        {
            var registerPage = new RegisterPage(dbHelper, this);
            MainFrame.Navigate(registerPage);
        }

        // Phương thức điều hướng đến trang chính sau khi đăng nhập thành công
        public void NavigateToUserMainPage(User User)
        {
            var userMainPage = new UserMainPage(User, this);
            MainFrame.Navigate(userMainPage);
        }

        public void NavigateToBookingPage(int cityId)
        {
            var bookingPage = new BookingPage(dbHelper, this, cityId);
            MainFrame.Navigate(bookingPage);
        }
    }
}