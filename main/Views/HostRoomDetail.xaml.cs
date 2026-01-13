using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using main.Models;

namespace main.Views
{
    public partial class HostRoomDetail : UserControl
    {
        private MainWindow _mainWindow;
        private Property _property;
        private bool _isEditMode = false;

        public HostRoomDetail(MainWindow mainWindow, Property property, bool isEditMode = false)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _property = property;
            _isEditMode = isEditMode;

            InitializePage();
            LoadRoomData();
        }

        private void InitializePage()
        {
            if (_isEditMode)
            {
                txtTitle.Text = "Chỉnh sửa phòng";
                btnEdit.Visibility = Visibility.Collapsed;
                // TODO: Enable edit mode for controls
            }
        }

        private void LoadRoomData()
        {
            // TODO: Load room data from database based on _ID
            txtDetailTitle.Text = "Phòng 101 - SMI House";
            txtDetailAddress.Text = "123 Võ Văn Ngân, Phường Linh Chiểu, Thủ Đức, TP.HCM";
            txtDetailPrice.Text = "5.300.000đ/tháng";
            txtDetailArea.Text = "30m²";
            txtDetailStatus.Text = "Trống";
            borderStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
            txtDetailDescription.Text = "Phòng trọ mới xây, sạch sẽ, yên tĩnh. Có đầy đủ tiện nghi: máy lạnh, nóng lạnh, wifi. Gần trường đại học, chợ, siêu thị.";

            // Add amenities
            wrapAmenities.Children.Add(CreateAmenityChip("WiFi"));
            wrapAmenities.Children.Add(CreateAmenityChip("Chỗ để xe"));
            wrapAmenities.Children.Add(CreateAmenityChip("Máy lạnh"));
            wrapAmenities.Children.Add(CreateAmenityChip("Nóng lạnh"));

            // Add sample images
            for (int i = 1; i <= 4; i++)
            {
                imagePanel.Children.Add(CreateImagePlaceholder());
            }
        }

        private Border CreateAmenityChip(string text)
        {
            return new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD")),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 0, 10, 10),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2")),
                    FontSize = 12
                }
            };
        }

        private Border CreateImagePlaceholder()
        {
            return new Border
            {
                Width = 200,
                Height = 120,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F9FA")),
                Margin = new Thickness(0, 0, 10, 0),
                CornerRadius = new CornerRadius(5),
                Child = new TextBlock
                {
                    Text = "Hình ảnh phòng",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Gray)
                }
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToHostMainPage(_property.Host);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToEditRoomPage(_ID);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Delete from database
                MessageBox.Show("Đã xóa phòng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _mainWindow?.NavigateToHostMainPage();
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (dpViewDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày xem phòng", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Đã đặt lịch xem phòng thành công! Chủ nhà sẽ liên hệ với bạn sớm.",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}