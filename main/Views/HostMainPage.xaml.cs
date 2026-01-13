using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using main.Models;

namespace main.Views
{
    public partial class HostMainPage : UserControl
    {
        private MainWindow _mainWindow;
        private Host _host;

        public ObservableCollection<Property> Rooms { get; set; }

        public HostMainPage(Host host, MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _host = host;
            Rooms = new ObservableCollection<Property>();
            LoadSampleData();
            RoomListView.ItemsSource = Rooms;
        }

        private void LoadSampleData()
        {
            Rooms.Add(new Property
            {
                ID = 1,
                Title = "Phòng 101 - SMI House",
                Address = "123 Võ Văn Ngân, Thủ Đức",
                Price = 5300000,
                Status = "Trống"
            });
            Rooms.Add(new Property
            {
                ID = 2,
                Title = "Phòng 201 - Air Apartment",
                Address = "456 Trường Sơn, Quận 10",
                Price = 6500000,
                Status = "Đã thuê"
            });
            Rooms.Add(new Property
            {
                ID = 3,
                Title = "Phòng 301 - New House",
                Address = "789 Tân Sơn, Gò Vấp",
                Price = 4200000,
                Status = "Trống"
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToMainPage();
        }

        private void AddNewRoom_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToPostPage();
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out Property property))
            //{
                _mainWindow?.NavigateToHostRoomDetail(property);
            //}
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int ID))
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này?", "Xác nhận xóa",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa phòng từ database
                    MessageBox.Show("Đã xóa phòng!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}