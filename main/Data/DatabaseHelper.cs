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
        //public static string connectionString = "Server=DESKTOP-4OM7515\\SQLEXPRESS;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        public static string connectionString = "Server=ADMIN;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public Account Login(string userName, string password)
        {
            Account account = null;
            string hashedPassword = HashPassword(password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        a.UserName,
                        a.PasswordHash,
                        a.[Renter(ID)],
                        a.Role, 

                        r.ID AS RenterID_PK,
                        r.FullName,
                        r.Email,
                        r.PhoneNumber,
                        r.CreatedAt
                    FROM Account a
                    JOIN Renter r ON a.[Renter(ID)] = r.ID
                    WHERE a.UserName = @UserName
                      AND a.PasswordHash = @PasswordHash";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar).Value = hashedPassword;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    account = new Account
                    {
                        UserName = reader["UserName"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        Renter = new Renter
                        {
                            ID = (int)reader["RenterID_PK"],
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        },
                        Role = (bool)reader["Role"],
                    };
                }
            }

            return account;
        }

        // Plan (pseudocode)
        // - Use a single connection and transaction to ensure atomicity.
        // - Insert into Renter in one statement, setting CreatedAt to current time using GETDATE().
        // - Capture the newly created Renter ID using OUTPUT INSERTED.ID.
        // - Insert into Account in the same transaction, linking the captured Renter ID via the foreign key column [Renter(ID)].
        // - Hash the password before inserting into Account.
        // - Commit the transaction if both inserts succeed, otherwise rollback.
        // - Return true if both operations succeed, false otherwise.

        public bool Register(Account account)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try 
                    {
                        var rows = 0;

                        if (!account.Role)
                        {
                            // Insert Renter and capture new ID
                            string cmd = @"
                            INSERT INTO Renter (Email, FullName, PhoneNumber, CreatedAt)
                            OUTPUT INSERTED.ID
                            VALUES (@Email, @FullName, @PhoneNumber, GETDATE());";

                            using var insertRenterCmd = new SqlCommand(cmd, conn, tran);
                            insertRenterCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = account.Renter.Email;
                            insertRenterCmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = account.Renter.FullName;
                            insertRenterCmd.Parameters.Add("@PhoneNumber", SqlDbType.VarChar).Value = account.Renter.PhoneNumber;

                            var renterIdObj = insertRenterCmd.ExecuteScalar();
                            if (renterIdObj == null || renterIdObj == DBNull.Value)
                            {
                                tran.Rollback();
                                return false;
                            }
                            var renterId = Convert.ToInt32(renterIdObj);

                            // Insert Account with FK to Renter
                            cmd = @"
                            INSERT INTO Account (UserName, PasswordHash, [Renter(ID)], Role)
                            VALUES (@UserName, @PasswordHash, @RenterID, 0);";

                            using var insertAccountCmd = new SqlCommand(cmd, conn, tran);
                            insertAccountCmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = account.UserName;
                            insertAccountCmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar).Value = HashPassword(account.PasswordHash);
                            insertAccountCmd.Parameters.Add("@RenterID", SqlDbType.Int).Value = renterId;

                            rows = insertAccountCmd.ExecuteNonQuery();
                        }

                        else
                        {
                            // Insert Host and capture new ID
                            string cmd = @"
                            INSERT INTO Host (Email, FullName, PhoneNumber, CreatedAt)
                            OUTPUT INSERTED.ID
                            VALUES (@Email, @FullName, @PhoneNumber, GETDATE());";

                            using var insertHostCmd = new SqlCommand(cmd, conn, tran);
                            insertHostCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = account.Host.Email;
                            insertHostCmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = account.Host.FullName;
                            insertHostCmd.Parameters.Add("@PhoneNumber", SqlDbType.VarChar).Value = account.Host.PhoneNumber;

                            var hostIdObj = insertHostCmd.ExecuteScalar();
                            if (hostIdObj == null || hostIdObj == DBNull.Value)
                            {
                                tran.Rollback();
                                return false;
                            }
                            var hostId = Convert.ToInt32(hostIdObj);

                            // Insert Account with FK to Host
                            cmd = @"
                            INSERT INTO Account (UserName, PasswordHash, [Host(ID)], Role)
                            VALUES (@UserName, @PasswordHash, @HostID, 1);";

                            using var insertAccountCmd = new SqlCommand(cmd, conn, tran);
                            insertAccountCmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = account.UserName;
                            insertAccountCmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar).Value = HashPassword(account.PasswordHash);
                            insertAccountCmd.Parameters.Add("@HostID", SqlDbType.Int).Value = hostId;

                            rows = insertAccountCmd.ExecuteNonQuery();
                        }

                        if (rows <= 0)
                        {
                            tran.Rollback();
                            return false;
                        }

                        tran.Commit();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        try { tran.Rollback(); } catch { /* ignore rollback failures */ }
                        throw new Exception("Đăng ký thất bại: " + ex.Message);
                    }
                }
            }
        }

        public bool IsUsernameExists(string UserName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Renter WHERE UserName = @UserName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserName", UserName);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool IsEmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Renter WHERE Email = @Email";
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

        // Property methods
        public List<Property> GetPropertiesByFilter(string city = null, decimal? minPrice = null, decimal? maxPrice = null, string roomType = null, List<string> amenities = null)
        {
            var properties = new List<Property>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.*, u.FullName as HostName, u.PhoneNumber as HostPhone
                               FROM Properties p
                               INNER JOIN Users u ON p.HostId = u.UserID
                               WHERE p.Status = 'Available'";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(city))
                {
                    query += " AND p.City = @City";
                    parameters.Add(new SqlParameter("@City", city));
                }

                if (minPrice.HasValue)
                {
                    query += " AND p.Price >= @MinPrice";
                    parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
                }

                if (maxPrice.HasValue)
                {
                    query += " AND p.Price <= @MaxPrice";
                    parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
                }

                if (!string.IsNullOrEmpty(roomType))
                {
                    query += " AND p.RoomType = @RoomType";
                    parameters.Add(new SqlParameter("@RoomType", roomType));
                }

                query += " ORDER BY p.PostedDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    properties.Add(new Property
                    {
                        ID = (int)reader["PropertyID"],
                        Host = new Host 
                        {
                            ID = (int)reader["HostID"],
                            FullName = reader["HostName"].ToString(),
                            PhoneNumber = reader["HostPhone"].ToString(),
                        },
                        Title = reader["Title"].ToString(),
                        Address = reader["Address"].ToString(),
                        Ward = new Ward
                        {
                            City = new City
                            {
                                Name = reader["City"].ToString(),
                            }
                        },
                        Price = (ulong)reader["Price"],
                        Deposit = (ulong)reader["Deposit"],
                        Area = (double)reader["Area"],
                        RoomType = reader["RoomType"].ToString(),
                        Capacity = (int)reader["Capacity"],
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        PostedDate = (DateTime)reader["PostedDate"],
                        ViewCount = (int)reader["ViewCount"],
                        
                    });
                }
            }

            return properties;
        }

        public Property GetPropertyById(int propertyId)
        {
            Property property = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.*, u.FullName as HostName, u.PhoneNumber as HostPhone, 
                                        u.Email as HostEmail
                               FROM Properties p
                               INNER JOIN Users u ON p.HostId = u.UserID
                               WHERE p.PropertyID = @PropertyID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    property = new Property
                    {
                        ID = (int)reader["PropertyID"],
                        Host = new Host
                        {
                            ID = (int)reader["HostID"],
                            FullName = reader["HostName"].ToString(),
                            PhoneNumber = reader["HostPhone"].ToString(),
                            Email = reader["HostEmail"].ToString(),
                        },
                        Title = reader["Title"].ToString(),
                        Address = reader["Address"].ToString(),
                        Ward = new Ward
                        {
                            City = new City
                            {
                                Name = reader["City"].ToString(),
                            }
                        },
                        Price = (ulong)reader["Price"],
                        Deposit = (ulong)reader["Deposit"],
                        Area = (double)reader["Area"],
                        RoomType = reader["RoomType"].ToString(),
                        Capacity = (int)reader["Capacity"],
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        PostedDate = (DateTime)reader["PostedDate"],
                        UpdatedDate = (DateTime)reader["UpdatedDate"],
                        ViewCount = (int)reader["ViewCount"]
                    };

                    // Tăng lượt xem
                    IncreaseViewCount(propertyId);
                }
            }

            // Load amenities
            if (property != null)
            {
                property.Amenities = GetPropertyAmenities(propertyId);
                property.ImageUrls = GetPropertyImages(propertyId);
            }

            return property;
        }

        private List<string> GetPropertyAmenities(int propertyId)
        {
            var amenities = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.Name FROM PropertyAmenities pa
                               INNER JOIN Amenities a ON pa.AmenityID = a.AmenityID
                               WHERE pa.PropertyID = @PropertyID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    amenities.Add(reader["Name"].ToString());
                }
            }

            return amenities;
        }

        private List<string> GetPropertyImages(int propertyId)
        {
            var images = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ImageUrl FROM PropertyImages 
                               WHERE PropertyID = @PropertyID 
                               ORDER BY DisplayOrder";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    images.Add(reader["ImageUrl"].ToString());
                }
            }

            return images;
        }

        private void IncreaseViewCount(int propertyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Properties SET ViewCount = ViewCount + 1 
                               WHERE PropertyID = @PropertyID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Host methods
        public List<Property> GetPropertiesByHostId(int hostId)
        {
            var properties = new List<Property>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Properties 
                               WHERE HostId = @HostId 
                               ORDER BY PostedDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HostId", hostId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    properties.Add(new Property
                    {
                        ID = (int)reader["PropertyID"],
                        Host = new Host
                        {
                            ID = (int)reader["HostID"],
                            FullName = reader["HostName"].ToString(),
                            PhoneNumber = reader["HostPhone"].ToString(),
                        },
                        Title = reader["Title"].ToString(),
                        Address = reader["Address"].ToString(),
                        Ward = new Ward
                        {
                            City = new City
                            {
                                Name = reader["City"].ToString(),
                            }
                        },
                        Price = (ulong)reader["Price"],
                        Deposit = (ulong)reader["Deposit"],
                        Area = (double)reader["Area"],
                        RoomType = reader["RoomType"].ToString(),
                        Status = reader["Status"].ToString(),
                        PostedDate = (DateTime)reader["PostedDate"],
                        ViewCount = (int)reader["ViewCount"]
                    });
                }
            }

            return properties;
        }

        public int AddProperty(Property property)
        {
            int newId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Properties 
                               (HostId, Title, Address, City, Price, Deposit, 
                                Area, RoomType, Capacity, Description, Status, PostedDate)
                               OUTPUT INSERTED.PropertyID
                               VALUES (@HostId, @Title, @Address, @City, @Price, 
                                       @Deposit, @Area, @RoomType, @Capacity, @Description, 
                                       'Available', @PostedDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HostId", property.Host.ID);
                cmd.Parameters.AddWithValue("@Title", property.Title);
                cmd.Parameters.AddWithValue("@Address", property.Address);
                cmd.Parameters.AddWithValue("@City", property.Ward.City);
                cmd.Parameters.AddWithValue("@Price", property.Price);
                cmd.Parameters.AddWithValue("@Deposit", property.Deposit);
                cmd.Parameters.AddWithValue("@Area", property.Area);
                cmd.Parameters.AddWithValue("@RoomType", property.RoomType);
                cmd.Parameters.AddWithValue("@Capacity", property.Capacity);
                cmd.Parameters.AddWithValue("@Description", property.Description ?? "");
                cmd.Parameters.AddWithValue("@PostedDate", DateTime.Now);

                conn.Open();
                newId = (int)cmd.ExecuteScalar();

                // Add amenities
                if (property.Amenities != null && property.Amenities.Count > 0)
                {
                    AddPropertyAmenities(newId, property.Amenities);
                }
            }

            return newId;
        }

        public bool UpdateProperty(Property property)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Properties SET 
                               Title = @Title,
                               Address = @Address,
                               City = @City,
                               Price = @Price,
                               Deposit = @Deposit,
                               Area = @Area,
                               RoomType = @RoomType,
                               Capacity = @Capacity,
                               Description = @Description,
                               Status = @Status,
                               UpdatedDate = @UpdatedDate
                               WHERE PropertyID = @PropertyID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", property.ID);
                cmd.Parameters.AddWithValue("@Title", property.Title);
                cmd.Parameters.AddWithValue("@Address", property.Address);
                cmd.Parameters.AddWithValue("@City", property.Ward.City);
                cmd.Parameters.AddWithValue("@Price", property.Price);
                cmd.Parameters.AddWithValue("@Deposit", property.Deposit);
                cmd.Parameters.AddWithValue("@Area", property.Area);
                cmd.Parameters.AddWithValue("@RoomType", property.RoomType);
                cmd.Parameters.AddWithValue("@Capacity", property.Capacity);
                cmd.Parameters.AddWithValue("@Description", property.Description ?? "");
                cmd.Parameters.AddWithValue("@Status", property.Status);
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                conn.Open();
                int result = cmd.ExecuteNonQuery();

                // Update amenities
                UpdatePropertyAmenities(property.ID, property.Amenities);

                return result > 0;
            }
        }

        public bool DeleteProperty(int propertyId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // First delete related records
                string deleteAmenities = "DELETE FROM PropertyAmenities WHERE PropertyID = @PropertyID";
                SqlCommand cmd1 = new SqlCommand(deleteAmenities, conn);
                cmd1.Parameters.AddWithValue("@PropertyID", propertyId);

                string deleteImages = "DELETE FROM PropertyImages WHERE PropertyID = @PropertyID";
                SqlCommand cmd2 = new SqlCommand(deleteImages, conn);
                cmd2.Parameters.AddWithValue("@PropertyID", propertyId);

                // Then delete property
                string deleteProperty = "DELETE FROM Properties WHERE PropertyID = @PropertyID";
                SqlCommand cmd3 = new SqlCommand(deleteProperty, conn);
                cmd3.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
                int result = cmd3.ExecuteNonQuery();

                return result > 0;
            }
        }

        private void AddPropertyAmenities(int propertyId, List<string> amenities)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (string amenity in amenities)
                {
                    // First get or create amenity
                    string getAmenityId = @"SELECT AmenityID FROM Amenities WHERE Name = @Name";
                    SqlCommand cmd1 = new SqlCommand(getAmenityId, conn);
                    cmd1.Parameters.AddWithValue("@Name", amenity);

                    object result = cmd1.ExecuteScalar();
                    int amenityId;

                    if (result == null)
                    {
                        // Create new amenity
                        string createAmenity = @"INSERT INTO Amenities (Name) OUTPUT INSERTED.AmenityID VALUES (@Name)";
                        SqlCommand cmd2 = new SqlCommand(createAmenity, conn);
                        cmd2.Parameters.AddWithValue("@Name", amenity);
                        amenityId = (int)cmd2.ExecuteScalar();
                    }
                    else
                    {
                        amenityId = (int)result;
                    }

                    // Link amenity to property
                    string linkQuery = @"INSERT INTO PropertyAmenities (PropertyID, AmenityID) 
                                       VALUES (@PropertyID, @AmenityID)";
                    SqlCommand cmd3 = new SqlCommand(linkQuery, conn);
                    cmd3.Parameters.AddWithValue("@PropertyID", propertyId);
                    cmd3.Parameters.AddWithValue("@AmenityID", amenityId);
                    cmd3.ExecuteNonQuery();
                }
            }
        }

        private void UpdatePropertyAmenities(int propertyId, List<string> amenities)
        {
            // First delete existing amenities
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM PropertyAmenities WHERE PropertyID = @PropertyID";
                SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Add new amenities
                if (amenities != null && amenities.Count > 0)
                {
                    AddPropertyAmenities(propertyId, amenities);
                }
            }
        }

        // Booking methods
        public bool CreateBookingRequest(int propertyId, int renterId, DateTime viewDate,
            string viewTime, string note)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO BookingRequests 
                               (PropertyID, RenterID, ViewDate, ViewTime, Note, Status, RequestDate)
                               VALUES (@PropertyID, @RenterID, @ViewDate, @ViewTime, @Note, 
                                       'Pending', @RequestDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);
                cmd.Parameters.AddWithValue("@RenterID", renterId);
                cmd.Parameters.AddWithValue("@ViewDate", viewDate);
                cmd.Parameters.AddWithValue("@ViewTime", viewTime);
                cmd.Parameters.AddWithValue("@Note", note ?? "");
                cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);

                conn.Open();
                int result = cmd.ExecuteNonQuery();

                return result > 0;
            }
        }

        // Favorite methods
        public bool ToggleFavorite(int propertyId, int renterId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Check if already favorited
                string checkQuery = @"SELECT COUNT(*) FROM Favorites 
                                    WHERE PropertyID = @PropertyID AND RenterID = @RenterID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@PropertyID", propertyId);
                checkCmd.Parameters.AddWithValue("@RenterID", renterId);

                conn.Open();
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Remove from favorites
                    string deleteQuery = @"DELETE FROM Favorites 
                                         WHERE PropertyID = @PropertyID AND RenterID = @RenterID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@PropertyID", propertyId);
                    deleteCmd.Parameters.AddWithValue("@RenterID", renterId);

                    return deleteCmd.ExecuteNonQuery() > 0;
                }
                else
                {
                    // Add to favorites
                    string insertQuery = @"INSERT INTO Favorites (PropertyID, RenterID, AddedDate)
                                         VALUES (@PropertyID, @RenterID, @AddedDate)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@PropertyID", propertyId);
                    insertCmd.Parameters.AddWithValue("@RenterID", renterId);
                    insertCmd.Parameters.AddWithValue("@AddedDate", DateTime.Now);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsPropertyFavorited(int propertyId, int renterId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) FROM Favorites 
                               WHERE PropertyID = @PropertyID AND RenterID = @RenterID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyID", propertyId);
                cmd.Parameters.AddWithValue("@RenterID", renterId);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }
    }
}