using System;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using SECW.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Data.Sqlite;

namespace SECW
{
    /// <summary>
    /// Represents the Login page for the application.
    /// Handles user authentication and role-based navigation.
    /// </summary>
    public partial class Login : ContentPage
    {
        // Connection string for the SQLite database
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        /// <summary>
        /// Initializes the database by invoking the helper method.
        /// Logs any errors encountered during initialization.
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                DataBaseHelper.initializeDatabase();
                Console.WriteLine("[INFO] Database initialized successfully.");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to initialize the database: {ex.Message}", "OK");
                Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the login submission event.
        /// Validates user credentials and navigates based on user roles.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void Submitbtn_Click(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            // Validate input fields
            if (username == null || password == null)
            {
                await DisplayAlert("Error", "Username or password cannot be null.", "OK");
                Console.WriteLine("[WARN] Login attempt failed: Null username or password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                Console.WriteLine("[INFO] Login attempt failed: Missing username or password.");
                return;
            }

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("[INFO] Database connection opened successfully.");

                    // Set busy timeout to prevent "database is locked" errors
                    using (var pragmaCommand = new SqliteCommand("PRAGMA busy_timeout = 3000;", connection))
                    {
                        pragmaCommand.ExecuteNonQuery();
                        Console.WriteLine("[INFO] PRAGMA busy_timeout set to 3000ms.");
                    }

                    string query = @"SELECT PasswordHash, RoleID FROM Users WHERE Username = @username";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString() ?? string.Empty;
                                int role = Convert.ToInt32(reader["RoleID"]);

                                // Verify password using BCrypt
                                if (!BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    await DisplayAlert("Error", "Invalid username or password.", "OK");
                                    Console.WriteLine($"[INFO] Login failed: Invalid password for username '{username}'.");
                                    return;
                                }

                                // Handle role-based navigation
                                HandleRoleBasedNavigation(role, username);
                            }
                            else
                            {
                                await DisplayAlert("Error", "Invalid username or password.", "OK");
                                Console.WriteLine($"[INFO] Login failed: Username '{username}' not found.");
                                return;
                            }
                        }
                    }

                    // Update the last login date
                    updateLastLoginDate(connection, username);
                }
            }
            catch (SqliteException ex)
            {
                await DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
                Console.WriteLine($"[ERROR] Database error during login attempt for username '{username}': {ex.Message}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
                Console.WriteLine($"[ERROR] Unexpected error during login attempt for username '{username}': {ex.Message}");
            }
        }

        /// <summary>
        /// Handles navigation based on the user's role.
        /// </summary>
        /// <param name="role">The role ID of the user.</param>
        /// <param name="username">The username of the logged-in user.</param>
        private async void HandleRoleBasedNavigation(int role, string username)
        {
            try
            {
                switch (role)
                {
                    case 1: // Admin
                        await DisplayAlert("Success", $"Welcome, Admin {username}!", "OK");
                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Admin.");
                        LoggedinUser(username);
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new AdminPage());
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to AdminPage.");
                        }
                        break;

                    case 2: // Operational Manager
                        await DisplayAlert("Success", $"Welcome, Operational Manager {username}!", "OK");
                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Operational Manager.");
                        LoggedinUser(username);
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new OperationalManagerPage());
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to OperationalManagerPage.");
                        }
                        break;

                    case 3: // Environmental Scientist
                        await DisplayAlert("Success", $"Welcome, Environmental Scientist {username}!", "OK");
                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Environmental Scientist.");
                        LoggedinUser(username);
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new EnvironmentalScientistPage());
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to EnvironmentalScientistPage.");
                        }
                        break;

                    default:
                        await DisplayAlert("Error", "Unknown role. Please contact support.", "OK");
                        Console.WriteLine($"[ERROR] Login failed: Username '{username}' has an unknown role.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error during role-based navigation for username '{username}': {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the user's last login date to the current timestamp in the database.
        /// </summary>
        /// <param name="connection">The SQLite connection.</param>
        /// <param name="username">The username of the logged-in user.</param>
        private static void updateLastLoginDate(SqliteConnection connection, string username)
        {
            try
            {
                string updateQuery = @"UPDATE Users SET LastLogin = @lastLoginDate WHERE Username = @username";
                using (var updateCommand = new SqliteCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@lastLoginDate", DateTime.Now);
                    updateCommand.Parameters.AddWithValue("@username", username);
                    updateCommand.ExecuteNonQuery();
                }

                Console.WriteLine($"[INFO] Last login date updated for username '{username}'.");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"[ERROR] Failed to update last login date for username '{username}': {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error while updating last login date for username '{username}': {ex.Message}");
            }
        }

        /// <summary>
        /// Logs the username of the currently logged-in user and stores it in preferences.
        /// </summary>
        /// <param name="username">The username of the logged-in user.</param>
        public void LoggedinUser(string username)
        {
            try
            {
                var loggedInUser = username;
                Console.WriteLine($"[INFO] Logged in user: {loggedInUser}");

                // Store the logged-in user in preferences for later use
                Preferences.Set("LoggedInUser", loggedInUser);
                Console.WriteLine("[INFO] Logged-in user stored in preferences.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to store logged-in user in preferences: {ex.Message}");
            }
        }
    }
}