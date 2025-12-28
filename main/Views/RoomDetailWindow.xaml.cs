using main.Data;
using main.Models;
using Dapper;
using System.Windows;
using Microsoft.Data.SqlClient;


namespace main.Views
{
    /// <summary>
    /// Interaction logic for RoomDetailWindow.xaml
    /// </summary>
    public partial class RoomDetailWindow : Window
    {
        private int _propertyId;

        public RoomDetailWindow(int propertyId)
        {
            InitializeComponent();
            _propertyId = propertyId;

            LoadProperty();
        }

        private void LoadProperty()
        {
            using (var conn = new SqlConnection(DatabaseHelper.connectionString))
            {
                var sql = "SELECT * FROM Property WHERE ID = @ID";
                var room = conn.QueryFirstOrDefault<Property>(sql, new { ID = _propertyId });

                if (room != null)
                {
                    TitleText.Text = room.Title;
                    AddressText.Text = room.Address;
                    PriceText.Text = $"{room.Price:N0} VND";
                }
            }
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new SqlConnection(DatabaseHelper.connectionString))
            {
                // 1. Lấy thông tin host của phòng
                string sqlHost = @"SELECT u.PhoneNumber
                                    FROM [User] u
                                    INNER JOIN Property p ON u.ID = p.HostID
                                    WHERE p.ID = @PropertyID";


                var hostPhone = conn.QueryFirstOrDefault<string>(sqlHost, new { PropertyID = _propertyId });

                string sqlBooking = @"INSERT INTO Booking (PropertyID, RenterID, StartDate, EndDate, Status, CreatedAt)
                       VALUES (@PropertyID, @RenterID, @StartDate, @EndDate, @Status, GETDATE())";

                conn.Execute(sqlBooking, new
                {
                    PropertyID = _propertyId,
                    RenterID = 4,                 // TODO: lấy user hiện tại
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    Status = "PENDING"
                });
                MessageBox.Show($"Đặt phòng thành công! Liên hệ SDT host: {hostPhone}");
            }

            
        }

    }
}
