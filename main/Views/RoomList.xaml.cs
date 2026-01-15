using main.Data;
using main.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class RoomList : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper;
        private City _city;
        public ObservableCollection<Property> Properties { get; set; }

        public RoomList(MainWindow mainWindow, DatabaseHelper dbHelper, City city)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dbHelper = dbHelper;
            _city = city;
            Properties = new ObservableCollection<Property>();

            LoadProperties();
        }

        private void LoadProperties()
        {
            try
            {
                Properties.Clear();

                // Lấy tất cả phòng có sẵn
                var properties = _dbHelper.GetPropertiesByFilter();

                foreach (var property in properties)
                {
                    Properties.Add(property);
                }

                propertyItemsControl.ItemsSource = Properties;
                txtResultCount.Text = $"Tìm thấy {Properties.Count} phòng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Clear();

                // Lấy giá trị filter
                decimal? minPrice = null;
                decimal? maxPrice = null;
                string roomType = null;

                // Xử lý filter theo giá
                var priceRange = (cmbPriceRange.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (priceRange)
                {
                    case "Dưới 2 triệu":
                        maxPrice = 2000000;
                        break;
                    case "2-3 triệu":
                        minPrice = 2000000;
                        maxPrice = 3000000;
                        break;
                    case "3-5 triệu":
                        minPrice = 3000000;
                        maxPrice = 5000000;
                        break;
                    case "5-7 triệu":
                        minPrice = 5000000;
                        maxPrice = 7000000;
                        break;
                    case "Trên 7 triệu":
                        minPrice = 7000000;
                        break;
                }

                // Xử lý filter theo loại phòng
                if (chkRoomType1.IsChecked == true && chkRoomType2.IsChecked == false &&
                    chkRoomType3.IsChecked == false && chkRoomType4.IsChecked == false)
                {
                    roomType = "Phòng trọ";
                }
                else if (chkRoomType2.IsChecked == true && chkRoomType1.IsChecked == false &&
                         chkRoomType3.IsChecked == false && chkRoomType4.IsChecked == false)
                {
                    roomType = "Chung cư mini";
                }
                // ... xử lý các trường hợp khác

                // Lấy danh sách phòng từ database
                var properties = _dbHelper.GetPropertiesByFilter(
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    roomType: roomType
                );

                foreach (var property in properties)
                {
                    Properties.Add(property);
                }

                propertyItemsControl.ItemsSource = Properties;
                txtResultCount.Text = $"Tìm thấy {Properties.Count} phòng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi áp dụng filter: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int propertyId))
            {
                try
                {
                    var property = _dbHelper.GetPropertyById(propertyId);
                    if (property != null)
                    {
                        _mainWindow?.NavigateToRoomDetail(property);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin phòng", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải chi tiết phòng: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToMainPage();
        }
    }
}