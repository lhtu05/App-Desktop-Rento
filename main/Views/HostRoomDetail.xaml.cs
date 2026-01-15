using main.Data;
using main.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class HostRoomDetail : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper;
        private Property _property;
        private bool _isEditMode = false;
        private ObservableCollection<PropertyImage> _images = new ObservableCollection<PropertyImage>();

        public HostRoomDetail(MainWindow mainWindow, DatabaseHelper dbHelper, Property property, bool isEditMode = false)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dbHelper = dbHelper;
            _property = property;
            _isEditMode = isEditMode;
            imageListControl.ItemsSource = _images;

            LoadPropertyData();
        }

        private void LoadPropertyData()
        {
            try
            {
                // Tải lại thông tin chi tiết từ database
                var detailedProperty = _dbHelper.GetPropertyById(_property.ID);
                if (detailedProperty != null)
                {
                    _property = detailedProperty;
                }

                // Hiển thị thông tin
                txtEditTitle.Text = _property.Title;
                txtEditAddress.Text = _property.Address;
                txtEditArea.Text = _property.Area.ToString();
                txtEditPrice.Text = _property.Price.ToString();
                txtEditDeposit.Text = _property.Deposit.ToString();
                txtEditDescription.Text = _property.Description;
                txtViewCount.Text = _property.ViewCount.ToString();

                // Set room type
                foreach (ComboBoxItem item in cmbEditRoomType.Items)
                {
                    if (item.Content.ToString() == _property.RoomType)
                    {
                        cmbEditRoomType.SelectedItem = item;
                        break;
                    }
                }

                // Set status
                if (_property.Status == "Available")
                    cmbEditStatus.SelectedIndex = 0;
                else if (_property.Status == "Rented")
                    cmbEditStatus.SelectedIndex = 1;
                else
                    cmbEditStatus.SelectedIndex = 2;

                // Set amenities
                if (_property.Amenities != null)
                {
                    chkEditWiFi.IsChecked = _property.Amenities.Contains("WiFi");
                    chkEditParking.IsChecked = _property.Amenities.Contains("Chỗ để xe");
                    chkEditAirCon.IsChecked = _property.Amenities.Contains("Máy lạnh");
                    chkEditWaterHeater.IsChecked = _property.Amenities.Contains("Nóng lạnh");
                    chkEditFridge.IsChecked = _property.Amenities.Contains("Tủ lạnh");
                    chkEditWashing.IsChecked = _property.Amenities.Contains("Máy giặt");
                    chkEditKitchen.IsChecked = _property.Amenities.Contains("Bếp");
                    chkEditSecurity.IsChecked = _property.Amenities.Contains("Bảo vệ 24/7");
                }

                // TODO: Load images
                // txtFavoriteCount.Text = GetFavoriteCount(_property.ID).ToString();
                // txtBookingRequests.Text = GetBookingRequestCount(_property.ID).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu phòng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cập nhật thông tin property
                _property.Title = txtEditTitle.Text;
                _property.Address = txtEditAddress.Text;
                _property.Area = double.Parse(txtEditArea.Text);
                _property.Price = ulong.Parse(txtEditPrice.Text);
                _property.Deposit = ulong.Parse(txtEditDeposit.Text);
                _property.Description = txtEditDescription.Text;
                _property.RoomType = (cmbEditRoomType.SelectedItem as ComboBoxItem)?.Content.ToString();
                _property.Status = cmbEditStatus.SelectedIndex == 0 ? "Available" :
                                 cmbEditStatus.SelectedIndex == 1 ? "Rented" : "Maintenance";

                // Collect amenities
                var amenities = new List<string>();
                if (chkEditWiFi.IsChecked == true) amenities.Add("WiFi");
                if (chkEditParking.IsChecked == true) amenities.Add("Chỗ để xe");
                if (chkEditAirCon.IsChecked == true) amenities.Add("Máy lạnh");
                if (chkEditWaterHeater.IsChecked == true) amenities.Add("Nóng lạnh");
                if (chkEditFridge.IsChecked == true) amenities.Add("Tủ lạnh");
                if (chkEditWashing.IsChecked == true) amenities.Add("Máy giặt");
                if (chkEditKitchen.IsChecked == true) amenities.Add("Bếp");
                if (chkEditSecurity.IsChecked == true) amenities.Add("Bảo vệ 24/7");
                if (chkEditCamera.IsChecked == true) amenities.Add("Camera an ninh");
                if (chkEditElevator.IsChecked == true) amenities.Add("Thang máy");

                _property.Amenities = amenities;

                // Lưu vào database
                bool success = _dbHelper.UpdateProperty(_property);

                if (success)
                {
                    MessageBox.Show("Đã lưu thay đổi!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể lưu thay đổi", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu thay đổi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này? Hành động này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _dbHelper.DeleteProperty(_property.ID);
                    if (success)
                    {
                        MessageBox.Show("Đã xóa phòng", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        // Quay về trang quản lý phòng
                        _mainWindow?.NavigateToHostMainPage(_property.Host);
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

        private void NavigateToHostMainPage(object sender, RoutedEventArgs e)
        {
            Property property = new Property();
            e.Handled = true;
            _mainWindow?.NavigateToHostMainPage(property.Host);
        }

        private void UploadImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
                return;

            // Giới hạn số lượng
            if (_images.Count + dialog.FileNames.Length > 10)
            {
                MessageBox.Show("Chỉ được tối đa 10 hình ảnh.");
                return;
            }

            foreach (var filePath in dialog.FileNames)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                // Giới hạn dung lượng (5MB)
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    MessageBox.Show($"Ảnh {fileInfo.Name} vượt quá 5MB.");
                    continue;
                }

                // Tránh trùng tên
                if (_images.Any(i => i.FileName == fileInfo.Name))
                    continue;

                _images.Add(new PropertyImage
                {
                    FileName = fileInfo.Name,
                    FilePath = filePath
                });
            }
        }
        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string fileName)
            {
                var item = _images.FirstOrDefault(i => i.FileName == fileName);
                if (item != null)
                    _images.Remove(item);
            }
        }
        private void NavigateToRoomDetail(object sender, RoutedEventArgs e)
        {
            Property property = new Property();
            e.Handled = true;
            _mainWindow?.NavigateToRoomDetail(property);
        }
    }
}