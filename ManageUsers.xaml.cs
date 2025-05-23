using Microsoft.Maui.Controls;
using SECW.Helpers;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using System.Windows.Input;

namespace SECW
{
    public partial class ManageUsers : ContentPage
    {
        public ObservableCollection<User> Users { get; set; }
        public ICommand DeleteUserCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }

        public static class DataBaseHelper
        {
            public static string ConnectionString { get; } = @"Data Source=Helpers\SoftwareEngineering.db;";
        }

        public ManageUsers()
        {
            InitializeComponent();

            // Initialize the Users collection
            Users = new ObservableCollection<User>();

            // Initialize commands
            DeleteUserCommand = new Command<User>(DeleteUser);
            AddUserCommand = new Command(AddUser);
            EditUserCommand = new Command<User>(EditUser); // Initialize the EditUserCommand

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
                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = @"SELECT Users.Username, Users.Email, Roles.RoleName, Users.RoleID
                                FROM Users
                                LEFT JOIN Roles ON Users.RoleID = Roles.RoleID
                                 WHERE Users.UserID != 1"; // Exclude admin account
                using var command = new SqliteCommand(query, connection);
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
            catch (SqliteException ex)
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

                // Hash the password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password.Text);

                // Map RoleName to RoleID
                int roleId = RolePicker.SelectedItem.ToString() switch
                {
                    "Admin" => 1,
                    "Operational Manager" => 2,
                    "Environmental Scientist" => 3,
                    _ => throw new Exception("Invalid role selected.")
                };

                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                // Check if the username already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Username", Username.Text);

                var count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count > 0)
                {
                    DisplayAlert("Error", "Username already exists. Please choose a different username.", "OK");
                    Console.WriteLine("Validation failed: Username already exists.");
                    return;
                }

                // Insert the new user into the database
                string query = @"INSERT INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                            VALUES (@Username, @PasswordHash, @Email, @RoleID, datetime('now'))";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", Username.Text);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                command.Parameters.AddWithValue("@Email", EmailEntry.Text);
                command.Parameters.AddWithValue("@RoleID", roleId);

                command.ExecuteNonQuery();

                // Add the new user to the ObservableCollection
                Users.Add(new User
                {
                    Name = Username.Text,
                    Email = EmailEntry.Text,
                    RoleName = RolePicker.SelectedItem.ToString() ?? string.Empty,
                    RoleID = roleId
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
            catch (SqliteException ex)
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

                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = "DELETE FROM Users WHERE Username = @Username";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", user.Name);

                command.ExecuteNonQuery();

                // Remove the user from the ObservableCollection
                Users.Remove(user);

                DisplayAlert("Success", "User deleted successfully.", "OK");
                Console.WriteLine($"User {user.Name} deleted successfully.");
            }
            catch (SqliteException ex)
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

        private async void EditUser(User user)
        {
            if (user == null)
            {
                Console.WriteLine("EditUser called with null user.");
                return;
            }

            Console.WriteLine($"Editing user: {user.Name}");

            // Open the EditUserPage as a modal
            await Navigation.PushModalAsync(new EditUserPage(user, OnUserModified));
        }

        private void OnUserModified(User modifiedUser)
        {
            try
            {
                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                // Update the user's details in the database
                string query = @"UPDATE Users
                                SET Username = @Username, Email = @Email
                                WHERE Username = @OriginalUsername";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Username", modifiedUser.Name);
                command.Parameters.AddWithValue("@Email", modifiedUser.Email);
                command.Parameters.AddWithValue("@OriginalUsername", modifiedUser.Name); // Assuming username is unique

                command.ExecuteNonQuery();

                // Update the user's role using UpdateUserRole
                UpdateUserRole(modifiedUser.Name, modifiedUser.RoleName);

                // Update the user in the ObservableCollection
                var user = Users.FirstOrDefault(u => u.Name == modifiedUser.Name);
                if (user != null)
                {
                    user.Name = modifiedUser.Name;
                    user.Email = modifiedUser.Email;
                    user.RoleName = modifiedUser.RoleName;
                }

                Console.WriteLine($"User {modifiedUser.Name} updated successfully.");
                DisplayAlert("Success", "User updated successfully.", "OK");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"SQLite error while updating user: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while updating user: {ex.Message}");
            }
        }

//was intially made to allow the admin to fiter users by role, and then export a mailing list of users and usernames and so on as a .txt file using the library stream writer.
//however, this was not implemented due to it not being a requirement for the project.
//just something id like to personally add in the future as a point of interest.
        private void FilterUsersByRole(string roleName)
        {
            try
            {
                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                string query = @"SELECT Users.Username, Users.Email, Roles.RoleName
                                FROM Users
                                INNER JOIN Roles ON Users.RoleID = Roles.RoleID
                                WHERE Roles.RoleName = @RoleName";

                using var command = new SqliteCommand(query, connection);
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
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error filtering users: {ex.Message}");
            }
        }

        private void UpdateUserRole(string username, string newRoleName)
        {
            try
            {
                using var connection = new SqliteConnection(DataBaseHelper.ConnectionString);
                connection.Open();

                // Map RoleName to RoleID
                int newRoleId = newRoleName switch
                {
                    "Admin" => 1,
                    "Operational Manager" => 2,
                    "Environmental Scientist" => 3,
                    _ => throw new Exception("Invalid role selected.")
                };

                // Update the user's RoleID in the database
                string updateQuery = "UPDATE Users SET RoleID = @RoleID WHERE Username = @Username";
                using var updateCommand = new SqliteCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@RoleID", newRoleId);
                updateCommand.Parameters.AddWithValue("@Username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"[INFO] Role updated successfully for user '{username}' to '{newRoleName}'.");
                }
                else
                {
                    Console.WriteLine($"[ERROR] Failed to update role for user '{username}'.");
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"[ERROR] SQLite error while updating role: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error while updating role: {ex.Message}");
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
    }