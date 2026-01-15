using main.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class MainPage : UserControl
    {
        private MainWindow _mainWindow;

        public ObservableCollection<Property> Rooms { get; set; }

        public MainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Rooms = new ObservableCollection<Property>();
        }

        private void NavigateToHostRegisterPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToHostRegisterPage();
        }

        private void NavigateToRenterRegisterPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToRenterRegisterPage();
        }

        private void NavigateToLoginPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToLoginPage();
        }

        private void HNButton_Click(object sender, RoutedEventArgs e)
        {
            City city = new City
            {
                ID = 1,
            };
            e.Handled = true;
            _mainWindow.NavigateToRoomList(city); 
        }

        private void TPHCMButton_Click(object sender, RoutedEventArgs e)
        {
            City city = new City
            {
                ID = 28,
            };
            e.Handled = true;
            _mainWindow.NavigateToRoomList(city);
        }


    }
}