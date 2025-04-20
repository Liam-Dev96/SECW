using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
using BCrypt.Net;
using SECW.Helpers;

namespace SECW
{
    public partial class AdminSettingsPage : ContentPage
    {
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";

        public AdminSettingsPage()
        {
            InitializeComponent();
        }

        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                string username = UsernameEntry.Text?.Trim() ?? string.Empty;
                string email = EmailEntry.Text?.Trim() ?? string.Empty;
                string oldPassword = OldPassword.Text;
                string newPassword = PasswordEntry.Text;
                string confirmPassword = ConfirmPasswordEntry.Text;

                if (!IsInputValid(username, email, oldPassword, newPassword, confirmPassword))
                    return;

                if (!ValidatePasswordChange(username, oldPassword, newPassword, confirmPassword))
                    return;

                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = @"UPDATE Users SET Email = @newEmail, PasswordHash = @newPassword WHERE Username = @username";

                    using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@username", username);
                        updateCommand.Parameters.AddWithValue("@newEmail", email);
                        updateCommand.Parameters.AddWithValue("@newPassword", hashedNewPassword);

                        updateCommand.ExecuteNonQuery();
                    }
                }

                DisplayAlert("Success", "Changes saved successfully.", "OK");
                Console.WriteLine("[INFO] Changes saved successfully.");
            }
            catch (SQLiteException ex)
            {
                DisplayAlert("Database Error", ex.Message, "OK");
                Console.WriteLine($"[ERROR] SQLite error: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                Console.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
            }
        }

        private bool ValidatePasswordChange(string username, string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                DisplayAlert("Validation Error", "Passwords do not match.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                DisplayAlert("Validation Error", "Old password cannot be empty.", "OK");
                return false;
            }

            if (oldPassword == newPassword)
            {
                DisplayAlert("Validation Error", "New password cannot be the same as the old password.", "OK");
                return false;
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                // TODO: Grab logged in user and used it to check if the password is correct
                string loggedInUser = GetLoggedInUser(); // Assuming you have a method to get the logged-in user

                string verifyQuery = @"SELECT PasswordHash FROM Users WHERE Username = @username";

                using (var verifyCommand = new SQLiteCommand(verifyQuery, connection))
                {
                    verifyCommand.Parameters.AddWithValue("@username", loggedInUser);
                    var storedHash = verifyCommand.ExecuteScalar()?.ToString();

                    if (storedHash == null)
                    {
                        DisplayAlert("Validation Error", "User not found.", "OK");
                        return false;
                    }

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(oldPassword, storedHash);
                    if (!isPasswordValid)
                    {
                        DisplayAlert("Validation Error", "Old password is incorrect.", "OK");
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetLoggedInUser()
{
    return Preferences.Get("LoggedInUser", string.Empty); // Default to empty string if not set
}

        private bool IsInputValid(string username, string email, string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                DisplayAlert("Input Error", "All fields are required.", "OK");
                return false;
            }

            if (!email.Contains("@"))
            {
                DisplayAlert("Input Error", "Please enter a valid email address.", "OK");
                return false;
            }

            return true;
        }
    }
}
