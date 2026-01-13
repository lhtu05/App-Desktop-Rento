using main.Data;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace main.Views
{
    public partial class PostPage : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper;

        public PostPage(DatabaseHelper dbHelper, MainWindow mainWindow)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToHostManagerPage();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.NavigateToHostManagerPage();
        }

        private void SelectImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Chọn hình ảnh phòng";

            if (openFileDialog.ShowDialog() == true)
            {
                // Xử lý file hình ảnh đã chọn
                MessageBox.Show($"Đã chọn {openFileDialog.FileNames.Length} hình ảnh",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PostRoom_Click(object sender, RoutedEventArgs e)
        {
            // Validate data
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtArea.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc (*)",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save room to database
            // TODO: Implement database save logic

            MessageBox.Show("Đăng phòng thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            _mainWindow?.NavigateToHostManagerPage();
        }
    }
}