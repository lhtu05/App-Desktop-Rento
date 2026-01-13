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

        public class RoomItem
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public string Address { get; set; }
            public decimal Price { get; set; }
            public string Status { get; set; }
            public string StatusColor => Status == "Trống" ? "#4CAF50" :
                                       Status == "Đã thuê" ? "#F44336" : "#FFC107";
        }

        public ObservableCollection<RoomItem> Rooms { get; set; }

        public HostMainPage(Host host, MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _host = host;
            Rooms = new ObservableCollection<RoomItem>();
            LoadSampleData();
            RoomListView.ItemsSource = Rooms;
        }

        private void LoadSampleData()
        {
            Rooms.Add(new RoomItem
            {
                RoomId = 1,
                RoomName = "Phòng 101 - SMI House",
                Address = "123 Võ Văn Ngân, Thủ Đức",
                Price = 5300000,
                Status = "Trống"
            });
            Rooms.Add(new RoomItem
            {
                RoomId = 2,
                RoomName = "Phòng 201 - Air Apartment",
                Address = "456 Trường Sơn, Quận 10",
                Price = 6500000,
                Status = "Đã thuê"
            });
            Rooms.Add(new RoomItem
            {
                RoomId = 3,
                RoomName = "Phòng 301 - New House",
                Address = "789 Tân Sơn, Gò Vấp",
                Price = 4200000,
                Status = "Trống"
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToMainPage();
        }

        private void AddNewRoom_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToPostPage();
        }

        private void ViewRoom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int roomId))
            {
                _mainWindow?.NavigateToHostRoomDetail(roomId);
            }
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int roomId))
            {
                _mainWindow?.NavigateToEditRoomPage(roomId);
            }
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int roomId))
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