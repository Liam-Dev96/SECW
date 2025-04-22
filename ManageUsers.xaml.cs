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

            BindingContext = this;
        }

        private void LoadUsersFromDatabase()
        {
            try
            {
                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = @"SELECT Users.Username, Users.Email, Roles.RoleName
                                FROM Users
                                INNER JOIN Roles ON Users.RoleID = Roles.RoleID";
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                Users.Clear();
                while (reader.Read())
                {
                    Users.Add(new User
                    {
                        Name = reader["Username"].ToString() ?? string.Empty,
                        Email = reader["Email"].ToString() ?? string.Empty,
                        role = reader["RoleName"].ToString() ?? string.Empty
                    });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(Username.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(Password.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPassword.Text) ||
                RolePicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (Password.Text != ConfirmPassword.Text)
            {
                DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

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
                    Email = EmailEntry.Text
                });

                // Clear input fields
                Username.Text = string.Empty;
                EmailEntry.Text = string.Empty;
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                RolePicker.SelectedItem = null;

                DisplayAlert("Success", "User added successfully.", "OK");
            }
            catch (SQLiteException ex)
            {
                DisplayAlert("Error", $"Failed to add user: {ex.Message}", "OK");
            }
        }

        private void DeleteUser(User user)
        {
            if (user == null)
                return;

            try
            {
                using var connection = new SQLiteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = "DELETE FROM Users WHERE Username = @Username";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", user.Name);

                command.ExecuteNonQuery();

                // Remove the user from the ObservableCollection
                Users.Remove(user);

                DisplayAlert("Success", "User deleted successfully.", "OK");
            }
            catch (SQLiteException ex)
            {
                DisplayAlert("Error", $"Failed to delete user: {ex.Message}", "OK");
            }
        }
    }

    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }
}