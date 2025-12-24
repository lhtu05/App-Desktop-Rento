using main.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace main.Views
{
    public partial class MainPage : UserControl
    {
        private MainWindow _mainWindow;

        public MainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        private void NavigateToRegisterPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToRegisterPage();
        }
        private void NavigateToLoginPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToLoginPage();
        }

    }
}