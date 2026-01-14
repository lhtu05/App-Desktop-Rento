using main.Data;
using main.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace main.Views
{
    public partial class HostMainPage : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper; // Thêm dòng này
        private Host _host;
        public ObservableCollection<PropertyWithStatus> Properties { get; set; }

        public HostMainPage(MainWindow mainWindow, Host host)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dbHelper = new DatabaseHelper(); // Thêm dòng này
            _host = host;
            Properties = new ObservableCollection<PropertyWithStatus>();

            InitializePage();
            LoadProperties();
        }

        private void LoadProperties()
        {
            try
            {
                Properties.Clear();

                // Lấy danh sách phòng của chủ nhà từ database
                var properties = _dbHelper.GetPropertiesByHostId(_host.Id);

                foreach (var property in properties)
                {
                    Properties.Add(new PropertyWithStatus
                    {
                        Id = property.Id,
                        Title = property.Title,
                        Address = property.Address,
                        Price = property.Price,
                        Status = property.Status == "Available" ? "Đang trống" :
                                property.Status == "Rented" ? "Đang thuê" : "Khác"
                    });
                }

                propertyItemsControl.ItemsSource = Properties;

                // Cập nhật thống kê
                int availableCount = Properties.Count(p => p.Status == "Đang trống");
                int rentedCount = Properties.Count(p => p.Status == "Đang thuê");

                txtTotalProperties.Text = Properties.Count.ToString();
                txtAvailableProperties.Text = availableCount.ToString();
                txtRentedProperties.Text = rentedCount.ToString();
                txtAverageRating.Text = _host.Rating.ToString("0.0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteProperty_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int propertyId))
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này?", "Xác nhận xóa",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = _dbHelper.DeleteProperty(propertyId);
                        if (success)
                        {
                            MessageBox.Show("Đã xóa phòng thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadProperties(); // Tải lại danh sách
                        }
                        else
                        {
                            MessageBox.Show("Không thể xóa phòng", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xóa phòng: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}