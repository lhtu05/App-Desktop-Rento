using main.Models;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace main.Data
{
    public class DatabaseHelper
    {
        private string connectionString = @"Data Source=.;Initial Catalog=RentalPlatform;Integrated Security=True";

        public static string ConnectionString = ".\\SQLEXPRESS;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public User Login(string username, string password)
        {
            User user = null;
            string hashedPassword = HashPassword(password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT UserID, Username, Email, FullName, PhoneNumber, UserType, CreatedDate, IsActive 
                               FROM Users 
                               WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        ID = (int)reader["UserID"],
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        FullName = reader["FullName"].ToString(),
                    };
                }
            }

            return user;
        }

        public bool Register(User user, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Users (Username, PasswordHash, Email, FullName, PhoneNumber, UserType) 
                                   VALUES (@Username, @PasswordHash, @Email, @FullName, @PhoneNumber, @UserType)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Đăng ký thất bại: " + ex.Message);
            }
        }

        public bool IsUsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool IsEmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        //private readonly List<Property> _properties = new();

        //public IReadOnlyList<Property> GetProperties() => _properties.AsReadOnly();

        //public Property InsertProperty(Property property)
        //{
        //    property.ID = _properties.Count + 1;
        //    _properties.Add(property);
        //    return property;
        //}


        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                var conn = new SqlConnection(_connectionString);
                conn.Open();
                return conn;
            }
        }

    }
}