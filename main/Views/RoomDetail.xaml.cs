using main.Data;
using main.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace main.Views
{
    public partial class RoomDetail : UserControl
    {
        private MainWindow _mainWindow;
        private DatabaseHelper _dbHelper;
        private Property _property;
        private bool _isFavorite = false;

        public RoomDetail(MainWindow mainWindow, DatabaseHelper dbHelper, Property property)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dbHelper = dbHelper; 
            _property = property;

            LoadPropertyDetails();
        }

        private void LoadPropertyDetails()
        {
            try
            {
                // Tải lại thông tin chi tiết từ database để có dữ liệu mới nhất
                var detailedProperty = _dbHelper.GetPropertyById(_property.ID);
                if (detailedProperty != null)
                {
                    _property = detailedProperty;
                }

                // Hiển thị thông tin
                txtPropertyTitle.Text = _property.Title;
                txtAddress.Text = _property.Address;
                txtArea.Text = $"{_property.Area}m²";
                txtRoomType.Text = _property.PropertyType;
                txtCapacity.Text = $"{_property.Capacity} người";
                txtPostedDate.Text = _property.PostedDate.ToString("dd/MM/yyyy");
                txtPrice.Text = $"{_property.Price:N0} đ/tháng";
                txtDescription.Text = _property.Description;
                txtHostName.Text = $"Chủ nhà: {_property.Host.FullName}";
                txtHostPhone.Text = _property.Host.PhoneNumber;

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
        private void NavigateToLoginPage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToLoginPage();
        }
        private void NavigateToRoomList(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _mainWindow?.NavigateToRoomList(_property.Ward.City);
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
