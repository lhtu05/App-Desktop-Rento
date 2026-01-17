using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using main.Models;
using main.Data;

namespace main.Views
{
    public partial class PostPage : UserControl
    {
        private MainWindow _mainWindow;
        private Host _host;
        private DatabaseHelper _dbHelper;

        public ObservableCollection<PropertyImage> UploadedImages { get; set; }

        public PostPage(DatabaseHelper dbHelper, MainWindow mainWindow, Host host)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _host = host;
            _dbHelper = dbHelper;
            UploadedImages = new ObservableCollection<PropertyImage>();
            cmbCity.ItemsSource = dbHelper.LoadCities();
            cmbPropertyType.ItemsSource = dbHelper.LoadPropertyType();

            InitializePage();
            SetupEventHandlers();
        }

        private void InitializePage()
        {
            uploadedImagesList.ItemsSource = UploadedImages;
            UpdatePreview();
        }

        private void SetupEventHandlers()
        {
            // Update preview when text changes
            txtTitle.TextChanged += (s, e) => UpdatePreview();
            txtAddress.TextChanged += (s, e) => UpdatePreview();
            txtPrice.TextChanged += (s, e) => UpdatePreview();
            txtArea.TextChanged += (s, e) => UpdatePreview();
            txtDescription.TextChanged += (s, e) => UpdatePreview();

            cmbWard.SelectionChanged += (s, e) => UpdatePreview();
            cmbCity.SelectionChanged += (s, e) => UpdatePreview();
        }

        private void UpdatePreview()
        {
            // Update title preview
            txtPreviewTitle.Text = string.IsNullOrWhiteSpace(txtTitle.Text)
                ? "[Tiêu đề phòng]"
                : txtTitle.Text;

            // Update address preview
            string district = (cmbWard.SelectedItem as ComboBoxItem)?.Content.ToString();
            string city = (cmbCity.SelectedItem as ComboBoxItem)?.Content.ToString();
            string street = txtAddress.Text;

            if (!string.IsNullOrWhiteSpace(street) && !string.IsNullOrWhiteSpace(district))
            {
                txtPreviewAddress.Text = $"{street}, {district}, {city}";
            }
            else
            {
                txtPreviewAddress.Text = "[Địa chỉ]";
            }

            // Update price preview
            if (decimal.TryParse(txtPrice.Text, out decimal price) && price > 0)
            {
                txtPreviewPrice.Text = $"{price:N0} đ/tháng";
            }
            else
            {
                txtPreviewPrice.Text = "[Giá]";
            }

            // Update area preview
            if (double.TryParse(txtArea.Text, out double area) && area > 0)
            {
                txtPreviewArea.Text = $"{area}m²";
            }
            else
            {
                txtPreviewArea.Text = "[Diện tích]";
            }

            // Update description preview
            if (!string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                string shortDesc = txtDescription.Text.Length > 100
                    ? txtDescription.Text.Substring(0, 100) + "..."
                    : txtDescription.Text;
                txtPreviewDescription.Text = shortDesc;
            }
            else
            {
                txtPreviewDescription.Text = "[Mô tả ngắn...]";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToHostMainPage(_host);
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            // Create a preview property object
            var previewProperty = CreatePropertyFromForm();

            // TODO: Show preview in a dialog or new window
            MessageBox.Show("Chức năng xem trước đầy đủ đang được phát triển", "Thông báo");
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            // Create property object
            var newProperty = CreatePropertyFromForm();

            // TODO: Save property to database
            // newProperty.HostId = _host.ID;
            // SaveToDatabase(newProperty);

            MessageBox.Show("Đăng phòng thành công! Phòng của bạn đang được duyệt.",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            _mainWindow?.NavigateToHostMainPage(_host);
        }

        private bool ValidateForm()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề phòng", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtAddress.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Giá thuê không hợp lệ", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrice.Focus();
                return false;
            }

            if (!double.TryParse(txtArea.Text, out double area) || area <= 0)
            {
                MessageBox.Show("Diện tích không hợp lệ", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtArea.Focus();
                return false;
            }

            if (!decimal.TryParse(txtDeposit.Text, out decimal deposit) || deposit < 0)
            {
                MessageBox.Show("Tiền cọc không hợp lệ", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtDeposit.Focus();
                return false;
            }

            if (UploadedImages.Count == 0)
            {
                var result = MessageBox.Show("Bạn chưa tải lên hình ảnh nào. Bạn có muốn tiếp tục không?",
                    "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                    return false;
            }

            return true;
        }

        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCity.SelectedValue == null) return;

            int cityID = (int)cmbCity.SelectedValue;
            cmbWard.ItemsSource = _dbHelper.LoadWardsByCityID(cityID);
        }

        private Property CreatePropertyFromForm()
        {
            var property = new Property
            {
                Title = txtTitle.Text,
                Address = txtAddress.Text,
                Ward = new Ward
                {
                    ID = (int)cmbWard.SelectedValue,
                },
                Price = ulong.Parse(txtPrice.Text),
                Area = double.Parse(txtArea.Text),
                Deposit = ulong.Parse(txtDeposit.Text),
                Status = int.Parse(txtStatus.Text),
                PropertyType = new PropertyType
                {
                    ID = (int)cmbPropertyType.SelectedValue,
                },
                PropertyAmenity = new PropertyAmenity
                {
                    // Collect amenities
                    Wifi = chkWifi.IsChecked,
                    Parking = chkParking.IsChecked,
                    AirCon = chkAirCon.IsChecked,
                    WaterHeater = chkWaterHeater.IsChecked,
                    Fridge = chkFridge.IsChecked,
                    Washing = true,
                    Kitchen = true,
                    Security = true,
                    Camera = true,
                    Elevator = true, 
                },
                Description = txtDescription.Text,
                PostedDate = System.DateTime.Now,
                UpdatedDate = System.DateTime.Now
            };

            // Collect image URLs
            property.ImageUrls = new System.Collections.Generic.List<string>();
            foreach (var img in UploadedImages)
            {
                property.ImageUrls.Add(img.FilePath);
            }

            return property;
        }

        private void UploadImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Chọn hình ảnh phòng";

            if (openFileDialog.ShowDialog() == true)
            {
                long maxSize = 5 * 1024 * 1024; // 5MB

                foreach (string filePath in openFileDialog.FileNames)
                {
                    var fileInfo = new System.IO.FileInfo(filePath);

                    if (fileInfo.Length > maxSize)
                    {
                        MessageBox.Show($"File {fileInfo.Name} vượt quá 5MB", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    if (UploadedImages.Count >= 10)
                    {
                        MessageBox.Show("Chỉ được tải lên tối đa 10 hình ảnh", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }

                    UploadedImages.Add(new PropertyImage
                    {
                        FileName = fileInfo.Name,
                        FilePath = filePath,
                        FileSize = fileInfo.Length
                    });
                }
            }
        }

        private void DeleteUploadedImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                string fileName = button.Tag.ToString();

                for (int i = 0; i < UploadedImages.Count; i++)
                {
                    if (UploadedImages[i].FileName == fileName)
                    {
                        UploadedImages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}