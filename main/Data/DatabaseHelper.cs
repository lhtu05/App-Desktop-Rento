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
        public static string connectionString = "Server=DESKTOP-4OM7515\\SQLEXPRESS;Database=Rento_DB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
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

                            // Insert Account with FK to Renter
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

    }
}