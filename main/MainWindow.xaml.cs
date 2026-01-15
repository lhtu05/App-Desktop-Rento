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
        public void NavigateToRoomList(City city)
        {
            var RoomList = new RoomList(this, dbHelper, city);
            MainFrame.Navigate(RoomList);
        }
        public void NavigateToRoomDetail(Property property)
        {
            var roomDetail = new RoomDetail(this, dbHelper, property);
            MainFrame.Navigate(roomDetail);
        }

        // Phương thức điều hướng đến trang đăng nhập
        public void NavigateToLoginPage()
        {
            var loginPage = new LoginPage(dbHelper, this);
            MainFrame.Navigate(loginPage);
        }

        // Phương thức điều hướng đến những trang cho người thuê
        public void NavigateToRenterRegisterPage()
        {
            var RenterRegisterPage = new RenterRegisterPage(dbHelper, this);
            MainFrame.Navigate(RenterRegisterPage);
        }

        public void NavigateToRenterMainPage(Renter renter)
        {
            var renterMainPage = new RenterMainPage(renter, this);
            MainFrame.Navigate(renterMainPage);
        }
        public void NavigateToRenterRoomList(Renter renter, City city)
        {
            var RenterRoomList = new RenterRoomList(this, dbHelper, renter, city);
            MainFrame.Navigate(RenterRoomList);
        }

        public void NavigateToRenterRoomDetail(Renter renter, Property property)
        {
            var renterRoomDetail = new RenterRoomDetail(this, dbHelper, renter, property);
            MainFrame.Navigate(renterRoomDetail);
        }

        // Phương thức điều hướng đến những trang cho người cho thuê
        public void NavigateToHostRegisterPage()
        {
            var HostRegisterPage = new HostRegisterPage(dbHelper, this);
            MainFrame.Navigate(HostRegisterPage);
        }

        public void NavigateToPostPage(Host host)
        {
            var postPage = new PostPage(dbHelper, this, host);
            MainFrame.Navigate(postPage);
        }

        public void NavigateToHostMainPage(Host host)
        {
            var hostMainPage = new HostMainPage(this, dbHelper, host);
            MainFrame.Navigate(hostMainPage);
        }
        
        public void NavigateToHostRoomDetail(Property property)
        {
            var hostRoomDetail = new HostRoomDetail(this, dbHelper, property, true); // true for edit mode
            MainFrame.Navigate(hostRoomDetail);
        }
    }
}