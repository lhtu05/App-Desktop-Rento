using main.Data;
using main.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace main.Views
{
    public partial class RenterRoomDetail : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper; // Thêm dòng này
        private Renter _renter;
        private Property _property;
        private bool _isFavorite = false;

        public RenterRoomDetail(MainWindow mainWindow, Renter renter, Property property)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dbHelper = new DatabaseHelper(); // Thêm dòng này
            _renter = renter;
            _property = property;

            LoadPropertyDetails();
            LoadAmenities();
            CheckFavoriteStatus();
        }

        private void LoadPropertyDetails()
        {
            try
            {
                // Tải lại thông tin chi tiết từ database để có dữ liệu mới nhất
                var detailedProperty = _dbHelper.GetPropertyById(_property.Id);
                if (detailedProperty != null)
                {
                    _property = detailedProperty;
                }

                // Hiển thị thông tin
                txtPropertyTitle.Text = _property.Title;
                txtAddress.Text = _property.Address;
                txtArea.Text = $"{_property.Area}m²";
                txtRoomType.Text = _property.RoomType;
                txtCapacity.Text = $"{_property.Capacity} người";
                txtPostedDate.Text = _property.PostedDate.ToString("dd/MM/yyyy");
                txtPrice.Text = $"{_property.Price:N0} đ/tháng";
                txtDescription.Text = _property.Description;
                txtHostName.Text = $"Chủ nhà: {_property.HostName}";
                txtHostPhone.Text = _property.HostPhone;

                // TODO: Load rating và reviews từ database
                txtRating.Text = "4.5";
                txtReviewCount.Text = "(12 đánh giá)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải chi tiết phòng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckFavoriteStatus()
        {
            try
            {
                _isFavorite = _dbHelper.IsPropertyFavorited(_property.Id, _renter.Id);
                UpdateFavoriteButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kiểm tra trạng thái yêu thích: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFavoriteButton()
        {
            btnFavorite.Content = _isFavorite ? "Đã yêu thích" : "Yêu thích";
            btnFavorite.Background = _isFavorite ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B")) :
                new SolidColorBrush(Colors.White);
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool success = _dbHelper.ToggleFavorite(_property.Id, _renter.Id);
                if (success)
                {
                    _isFavorite = !_isFavorite;
                    UpdateFavoriteButton();

                    string message = _isFavorite ?
                        "Đã thêm vào danh sách yêu thích" :
                        "Đã xóa khỏi danh sách yêu thích";
                    MessageBox.Show(message, "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật trạng thái yêu thích", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật yêu thích: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

            try
            {
                string viewTime = (cmbViewTime.SelectedItem as ComboBoxItem)?.Content.ToString();
                string note = txtNote.Text;

                bool success = _dbHelper.CreateBookingRequest(
                    _property.Id,
                    _renter.Id,
                    dpViewDate.SelectedDate.Value,
                    viewTime,
                    note
                );

                if (success)
                {
                    MessageBox.Show("Đã gửi yêu cầu đặt lịch xem phòng! Chủ nhà sẽ liên hệ với bạn sớm.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reset form
                    txtNote.Text = "";
                }
                else
                {
                    MessageBox.Show("Không thể gửi yêu cầu đặt lịch", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đặt lịch xem phòng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    //private void LoadProperty()
    //{
    //    using (var conn = new SqlConnection(DatabaseHelper.connectionString))
    //    {
    //        var sql = "SELECT * FROM Property WHERE ID = @ID";
    //        var room = conn.QueryFirstOrDefault<Property>(sql, new { ID = _propertyId });

    //        if (room != null)
    //        {
    //            TitleText.Text = room.Title;
    //            AddressText.Text = room.Address;
    //            PriceText.Text = $"{room.Price:N0} VND";
    //        }
    //    }
    //}

    //private void Book_Click(object sender, RoutedEventArgs e)
    //{
    //    using (var conn = new SqlConnection(DatabaseHelper.connectionString))
    //    {
    //        // 1. Lấy thông tin host của phòng
    //        string sqlHost = @"SELECT u.PhoneNumber
    //                            FROM [User] u
    //                            INNER JOIN Property p ON u.ID = p.HostID
    //                            WHERE p.ID = @PropertyID";


    //        var hostPhone = conn.QueryFirstOrDefault<string>(sqlHost, new { PropertyID = _propertyId });

    //        string sqlBooking = @"INSERT INTO Booking (PropertyID, RenterID, StartDate, EndDate, Status, CreatedAt)
    //               VALUES (@PropertyID, @RenterID, @StartDate, @EndDate, @Status, GETDATE())";

    //        conn.Execute(sqlBooking, new
    //        {
    //            PropertyID = _propertyId,
    //            RenterID = 4,                 // TODO: lấy Renter hiện tại
    //            StartDate = DateTime.Now,
    //            EndDate = DateTime.Now.AddDays(1),
    //            Status = "PENDING"
    //        });
    //        MessageBox.Show($"Đặt phòng thành công! Liên hệ SDT host: {hostPhone}");
    //    }
}

    public class Review
    {
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}