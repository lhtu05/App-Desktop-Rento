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
        public void NavigateToRenterRegisterPage()
        {
            var RenterRegisterPage = new RenterRegisterPage(dbHelper, this);
            MainFrame.Navigate(RenterRegisterPage);
        }
        public void NavigateToHostRegisterPage()
        {
            var HostRegisterPage = new HostRegisterPage(dbHelper, this);
            MainFrame.Navigate(HostRegisterPage);
        }

        // Phương thức điều hướng đến trang chính sau khi đăng nhập thành công
        public void NavigateToHostMainPage(Host host)
        {
            var hostMainPage = new HostMainPage(host, this);
            MainFrame.Navigate(hostMainPage);
        }
        public void NavigateToRenterMainPage(Renter renter)
        {
            var renterMainPage = new RenterMainPage(renter, this);
            MainFrame.Navigate(renterMainPage);
        }
        public void NavigateToBookingPage(int cityId)
        {
            var bookingPage = new BookingPage(dbHelper, this, cityId);
            MainFrame.Navigate(bookingPage);
        }

        public void NavigateToRoomDetailPage(int propertyId)
        {
            var roomDetailPage = new RoomDetailPage(propertyId);
            MainFrame.Navigate(roomDetailPage);
        }
    }
}