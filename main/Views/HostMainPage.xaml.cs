using main.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class HostMainPage : UserControl
    {
        private MainWindow _mainWindow;

        public ObservableCollection<Property> Rooms { get; set; }

        public HostMainPage(Host host,MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Rooms = new ObservableCollection<Property>();
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

        private void NavigateToPostPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToPostPage();
        }

        private void HNButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow.NavigateToBookingPage(1); 
        }

        private void TPHCMButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow.NavigateToBookingPage(2);
        }


    }
}