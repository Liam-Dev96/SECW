using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;

namespace SECW
{
    public partial class Login : ContentPage
    {
        private static string connectionString = @"Data Source=Properties\Data\SoftwareEngineering.db;Version=3;";

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
            }
        }

        private async void Submitbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
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
                        command.Parameters.AddWithValue("@username", Username.Text);
                        command.Parameters.AddWithValue("@passwordHash", Password.Text);

                        var roleId = command.ExecuteScalar();

                        if (roleId != null)
                        {
                            int role = Convert.ToInt32(roleId);
                            switch (role)
                            {
                                case 1:
                                    await DisplayAlert("Success", "Welcome, Admin!", "OK");
                                    break;
                                case 2:
                                    await DisplayAlert("Success", "Welcome, User!", "OK");
                                    break;
                                case 3:
                                    await DisplayAlert("Success", "Welcome, Guest!", "OK");
                                    break;
                                default:
                                    await DisplayAlert("Error", "Unknown role. Please contact support.", "OK");
                                    break;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid username or password.", "OK");
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                await DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }
    }
}