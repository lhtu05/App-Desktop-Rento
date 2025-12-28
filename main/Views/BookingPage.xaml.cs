using Dapper;
using main.Data;
using main.Models;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace main.Views
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        private DatabaseHelper _dbHelper;
        private MainWindow _mainWindow;
        private int _cityID;
        public BookingPage(DatabaseHelper dbHelper, MainWindow mainWindow, int cityId)
        {
            InitializeComponent();
            _dbHelper = dbHelper;
            _mainWindow = mainWindow;
            _cityID = cityId;
            LoadCitiesAndProperties();
        }
       
        private void LoadCitiesAndProperties()
        {
            LoadCities();
            LoadWards(_cityID);
            LoadProperties(_cityID, null);
        }

        private void LoadCities()
        {
            using (var conn = new SqlConnection(DatabaseHelper.connectionString))
            {
                var cities = conn.Query<City>("SELECT ID, Name FROM City").ToList();
                CityComboBox.ItemsSource = cities;
                CityComboBox.SelectedValuePath = "ID";
                CityComboBox.DisplayMemberPath = "Name";
                CityComboBox.SelectedValue = _cityID;
            }
        }

        private void LoadWards(int cityID)
        {
            using (var conn = new SqlConnection(DatabaseHelper.connectionString))
            {
                var wards = conn.Query<Ward>("SELECT ID, Name, CityID FROM Ward WHERE CityID=@CityID", new { CityID = cityID }).ToList();
                WardComboBox.ItemsSource = wards;
                WardComboBox.SelectedValuePath = "ID";
                WardComboBox.DisplayMemberPath = "Name";
            }
        }

        private void LoadProperties(int cityID, int? wardID)
        {
            using (var conn = new SqlConnection(DatabaseHelper.connectionString))
            {
                string sql = @"
                SELECT p.ID, p.Title, p.Price, p.Street, p.Address
                FROM Property p
                INNER JOIN Ward w ON p.WardID = w.ID
                WHERE w.CityID = @CityID AND p.Status='AVAILABLE'";

                if (wardID != null)
                    sql += " AND w.ID = @WardID";

                var properties = conn.Query<Property>(sql, new { CityID = cityID, WardID = wardID }).ToList();
                PropertyListView.ItemsSource = properties;
            }
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityComboBox.SelectedValue == null) return;
            int selectedCityID = (int)CityComboBox.SelectedValue;
            LoadWards(selectedCityID);
            LoadProperties(selectedCityID, null);
        }

        private void WardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WardComboBox.SelectedValue == null) return;
            int wardID = (int)WardComboBox.SelectedValue;
            int cityID = (int)CityComboBox.SelectedValue;
            LoadProperties(cityID, wardID);
        }

        private void ViewRoom_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int roomId = (int)btn.Tag;

            var detailWindow = new RoomDetailWindow(roomId);
            detailWindow.Show();
        }


    }
}
