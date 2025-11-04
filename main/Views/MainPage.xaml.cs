using main.Models;
using System.Windows;
using System.Windows.Controls;

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

    }
}