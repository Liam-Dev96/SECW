using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
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
                    string query = @"SELECT RoleID FROM Users WHERE Username = @username AND PasswordHash = @passwordHash";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@passwordHash", password);

                        var roleId = command.ExecuteScalar();

                        if (roleId != null)
                        {
                            int role = Convert.ToInt32(roleId);
                            switch (role)
                            {
                                case 1:
                                    await DisplayAlert("Success", "Welcome, Admin!", "OK");
                                    Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Admin.");
                                    // Navigate to Admin page or perform admin-specific actions
                                    // Assuming the AdminPage.xaml.cs file for the admin interface exsists
                                    // and is properly set up to handle admin functionalities.
                                    await this.Navigation.PopAsync(); // Close the login page
                                    if (Application.Current != null)
                                    {
                                        Application.Current.MainPage = new NavigationPage(new AdminPage());
                                    }
                                    else
                                    {
                                        Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to AdminPage.");
                                    }

                                    break;
                                case 2:
                                    await DisplayAlert("Success", "Welcome, User!", "OK");
                                    Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as User.");
                                    break;
                                case 3:
                                    await DisplayAlert("Success", "Welcome, Guest!", "OK");
                                    Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Guest.");
                                    break;
                                default:
                                    await DisplayAlert("Error", "Unknown role. Please contact support.", "OK");
                                    Console.WriteLine($"[ERROR] Login failed: Username '{username}' has an unknown role.");
                                    connection.Close();
                                    return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid username or password.", "OK");
                            Console.WriteLine($"[INFO] Login failed: Invalid credentials for username '{username}'.");
                            connection.Close();
                            return;
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
    }
}