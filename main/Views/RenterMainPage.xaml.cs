using main.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class RenterMainPage : UserControl
    {
        private MainWindow _mainWindow;

        public ObservableCollection<Property> Rooms { get; set; }

        public RenterMainPage(Renter renter, MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Rooms = new ObservableCollection<Property>();
        }

        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToMainPage();
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