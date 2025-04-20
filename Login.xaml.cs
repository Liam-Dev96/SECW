using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
using BCrypt.Net;
using SECW.Helpers;

namespace SECW
{
    public partial class Login : ContentPage
    {
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";

        public Login()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                DataBaseHelper.initializeDatabase();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to initialize the database: {ex.Message}", "OK");
                Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
            }
        }

        private async void Submitbtn_Click(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                Console.WriteLine("[INFO] Login attempt failed: Missing username or password.");
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Query now only fetches the password hash and role for the user.
                    string query = @"SELECT PasswordHash, RoleID FROM Users WHERE Username = @username";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString() ?? string.Empty;
                                int role = Convert.ToInt32(reader["RoleID"]);

                                // Use BCrypt to verify entered password against stored hash.
                                if (!BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    await DisplayAlert("Error", "Invalid username or password.", "OK");
                                    Console.WriteLine($"[INFO] Login failed: Invalid password for username '{username}'.");
                                    return;
                                }

                                // If password is valid, proceed with role-based navigation
                                switch (role)
                                {
                                    case 1:
                                        await DisplayAlert("Success", $"Welcome, Admin {username}!", "OK");
                                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Admin.");
                                        await this.Navigation.PopAsync(); // Close login page
                                        updateLastLoginDate(username);    // Update last login time

                                        try
                                        {
                                            if (Application.Current != null)
                                            {
                                                Application.Current.MainPage = new NavigationPage(new AdminPage());
                                            }
                                            else
                                            {
                                                Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to AdminPage.");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"[ERROR] Failed to navigate to AdminPage: {ex.Message}");
                                        }

                                        break;

                                    case 2:
                                        await DisplayAlert("Success", $"Welcome, User: {username}!", "OK");
                                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as User.");
                                        await this.Navigation.PopAsync(); // Close login page
                                        updateLastLoginDate(username);    // Update last login time

                                        if (Application.Current != null)
                                        {
                                            Application.Current.MainPage = new NavigationPage(new UserPage());
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to UserPage.");
                                        }
                                        break;

                                    case 3:
                                        await DisplayAlert("Success", "Welcome, Guest!", "OK");
                                        Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Guest.");
                                        break;

                                    default:
                                        await DisplayAlert("Error", "Unknown role. Please contact support.", "OK");
                                        Console.WriteLine($"[ERROR] Login failed: Username '{username}' has an unknown role.");
                                        return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "Invalid username or password.", "OK");
                                Console.WriteLine($"[INFO] Login failed: Username '{username}' not found.");
                                return;
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
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

        // Updates the user's last login date to the current timestamp in the database.
        private static void updateLastLoginDate(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                try
                {
                    string updateQuery = @"UPDATE Users SET LastLogin = @lastLoginDate WHERE Username = @username";
                    using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@lastLoginDate", DateTime.Now);
                        updateCommand.Parameters.AddWithValue("@username", username);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"[ERROR] Failed to update last login date for username '{username}': {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Unexpected error while updating last login date for username '{username}': {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine($"[INFO] Last login date updated for username '{username}'.");
                }
            }
        }
    }
}