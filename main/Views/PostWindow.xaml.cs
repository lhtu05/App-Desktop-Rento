using Dapper;
using main.Data;
using main.Models;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace main.Views
{

    public partial class PostWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private MainWindow _mainWindow;
        private int _cityID;

        public PostWindow(DatabaseHelper dbHelper, MainWindow mainWindow)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
            LoadCities();
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {

            var room = new Property
            {
                Title = txtName.Text,
                Address = txtAddress.Text,
                Price = decimal.Parse(txtPrice.Text),
                Description = txtDescription.Text,
                WardID = (int)WardComboBox.SelectedValue,
                HostID = 3

            };
            using (var conn = _dbHelper.Connection)
            {
                string sql = @"
                INSERT INTO Property (HostID, WardID, Title, Address, Price, Description, Status, CreatedAt)
                VALUES (@HostID, @WardID, @Title, @Address, @Price, @Description, 'AVAILABLE', GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);"; 

                    int newPropertyId = conn.QuerySingle<int>(sql, room);

                    MessageBox.Show($"Đăng phòng thành công! ID mới: {newPropertyId}");
            }

        }

        private void LoadCities()
        {
            
            using (var conn = _dbHelper.Connection)
            {
                var cities = conn.Query<City>("SELECT ID, Name FROM City").ToList();
                CityComboBox.ItemsSource = cities;
                CityComboBox.SelectedValuePath = "ID";
                CityComboBox.DisplayMemberPath = "Name";
                CityComboBox.SelectedValue = _cityID;
            }
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityComboBox.SelectedValue == null)
                return;

            int cityId = (int)CityComboBox.SelectedValue;

            using (var conn = _dbHelper.Connection)
            {
                var wards = conn.Query<Ward>(
                    "SELECT ID, Name FROM Ward WHERE CityID = @CityID ORDER BY Name",
                    new { CityID = cityId }).ToList();

                WardComboBox.ItemsSource = wards;
                WardComboBox.DisplayMemberPath = "Name";  
                WardComboBox.SelectedValuePath = "ID";
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
