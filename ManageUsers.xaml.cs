using Microsoft.Maui.Controls;
using SECW.Helpers;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows.Input;

namespace SECW
{
    public partial class ManageUsers : ContentPage
    {
        public ObservableCollection<User> Users { get; set; }
        public ICommand DeleteUserCommand { get; }
        public ICommand AddUserCommand { get; }

        public static class DataBaseHelper
        {
            public static string ConnectionString { get; } = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";
        }

        public ManageUsers()
        {
            InitializeComponent();

            // Initialize the Users collection
            Users = new ObservableCollection<User>();

            // Initialize commands
            DeleteUserCommand = new Command<User>(DeleteUser);
            AddUserCommand = new Command(AddUser);

            // Load users from the database
            LoadUsersFromDatabase();

            // Set the BindingContext for data binding
            BindingContext = this;
        }

        private void LoadUsersFromDatabase()
        {
            try
            {
                Console.WriteLine("Attempting to load users from the database...");
                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = @"SELECT Users.Username, Users.Email, Roles.RoleName, Users.RoleID
                                FROM Users
                                LEFT JOIN Roles ON Users.RoleID = Roles.RoleID";
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                Users.Clear();
                while (reader.Read())
                {
                    var roleName = reader["RoleName"]?.ToString() ?? "No Role";
                    var roleId = reader["RoleID"] != DBNull.Value ? Convert.ToInt32(reader["RoleID"]) : 0;
                    Console.WriteLine($"Loaded user: {reader["Username"]}, Role: {roleName} (ID: {roleId})");

                    Users.Add(new User
                    {
                        Name = reader["Username"].ToString() ?? string.Empty,
                        Email = reader["Email"].ToString() ?? string.Empty,
                        RoleName = roleName,
                        RoleID = roleId
                    });
                }

                Console.WriteLine("Users loaded successfully.");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private void AddUser()
        {
            try
            {
                Console.WriteLine("Attempting to add a new user...");

                // Validate input fields
                if (string.IsNullOrWhiteSpace(Username.Text) ||
                    string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                    string.IsNullOrWhiteSpace(Password.Text) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword.Text))
                {
                    DisplayAlert("Error", "Please fill in all required fields.", "OK");
                    Console.WriteLine("Validation failed: Missing required fields.");
                    return;
                }

                if (RolePicker.SelectedItem == null)
                {
                    DisplayAlert("Error", "Please select a role for the user.", "OK");
                    Console.WriteLine("Validation failed: No role selected.");
                    return;
                }

                if (Password.Text != ConfirmPassword.Text)
                {
                    DisplayAlert("Error", "Passwords do not match.", "OK");
                    Console.WriteLine("Validation failed: Passwords do not match.");
                    return;
                }

                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                // Check if the username already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using var checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Username", Username.Text);

                var count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count > 0)
                {
                    DisplayAlert("Error", "Username already exists. Please choose a different username.", "OK");
                    Console.WriteLine("Validation failed: Username already exists.");
                    return;
                }

                // Insert the new user
                string query = @"INSERT INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                                VALUES (@Username, @PasswordHash, @Email, 
                                        (SELECT RoleID FROM Roles WHERE RoleName = @RoleName), datetime('now'))";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", Username.Text);
                command.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(Password.Text));
                command.Parameters.AddWithValue("@Email", EmailEntry.Text);
                command.Parameters.AddWithValue("@RoleName", RolePicker.SelectedItem.ToString());

                command.ExecuteNonQuery();

                // Add the new user to the ObservableCollection
                Users.Add(new User
                {
                    Name = Username.Text,
                    Email = EmailEntry.Text,
                    RoleName = RolePicker.SelectedItem.ToString() ?? string.Empty
                });

                // Clear input fields
                Username.Text = string.Empty;
                EmailEntry.Text = string.Empty;
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                RolePicker.SelectedItem = null;

                DisplayAlert("Success", "User added successfully.", "OK");
                Console.WriteLine("User added successfully.");
            }
            catch (SQLiteException ex)
            {
                DisplayAlert("Error", $"Failed to add user: {ex.Message}", "OK");
                Console.WriteLine($"SQLite error while adding user: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
                Console.WriteLine($"Unexpected error while adding user: {ex.Message}");
            }
        }

        private void DeleteUser(User user)
        {
            if (user == null)
            {
                Console.WriteLine("DeleteUser called with null user.");
                return;
            }

            try
            {
                Console.WriteLine($"Attempting to delete user: {user.Name}");

                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = "DELETE FROM Users WHERE Username = @Username";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", user.Name);

                command.ExecuteNonQuery();

                // Remove the user from the ObservableCollection
                Users.Remove(user);

                DisplayAlert("Success", "User deleted successfully.", "OK");
                Console.WriteLine($"User {user.Name} deleted successfully.");
            }
            catch (SQLiteException ex)
            {
                DisplayAlert("Error", $"Failed to delete user: {ex.Message}", "OK");
                Console.WriteLine($"SQLite error while deleting user: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
                Console.WriteLine($"Unexpected error while deleting user: {ex.Message}");
            }
        }

        private void FilterUsersByRole(string roleName)
        {
            try
            {
                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = @"SELECT Users.Username, Users.Email, Roles.RoleName
                                FROM Users
                                INNER JOIN Roles ON Users.RoleID = Roles.RoleID
                                WHERE Roles.RoleName = @RoleName";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@RoleName", roleName);
                using var reader = command.ExecuteReader();

                Users.Clear();
                while (reader.Read())
                {
                    Users.Add(new User
                    {
                        Name = reader["Username"].ToString() ?? string.Empty,
                        Email = reader["Email"].ToString() ?? string.Empty,
                        RoleName = reader["RoleName"].ToString() ?? string.Empty
                    });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error filtering users: {ex.Message}");
            }
        }
    }

    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty; // Display RoleName
        public int RoleID { get; set; } // Store RoleID
    }
}