using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
using BCrypt.Net;
using SECW.Helpers;

namespace SECW
{
    public partial class AdminSettingsPage : ContentPage
    {
        // Connection string for SQLite database
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";

        public AdminSettingsPage()
        {
            InitializeComponent();
        }

        // Event handler for the "Save Changes" button click
        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                // Retrieve the logged-in user's username
                string loggedInUser = GetLoggedInUser();
                string username = loggedInUser; // Ensure the logged-in user is being updated
                string newUsername = UsernameEntry.Text?.Trim() ?? string.Empty; // New username field
                string email = EmailEntry.Text?.Trim() ?? string.Empty; // Email field
                string oldPassword = OldPassword.Text; // Old password field
                string newPassword = PasswordEntry.Text; // New password field
                string confirmPassword = ConfirmPasswordEntry.Text; // Confirm password field

                // Validate required fields
                if (string.IsNullOrWhiteSpace(username))
                {
                    DisplayAlert("Input Error", "Username is required.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    DisplayAlert("Input Error", "Old password is required.", "OK");
                    return;
                }

                // Validate the old password before proceeding
                if (!ValidatePasswordChange(username, oldPassword))
                {
                    return; // Validation failed, error already displayed
                }

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        string updateQuery = "UPDATE Users SET ";
                        var parameters = new List<string>();

                        // Check if new username is provided and valid
                        if (!string.IsNullOrWhiteSpace(newUsername))
                        {
                            if (newUsername == username)
                            {
                                DisplayAlert("Input Error", "New username cannot be the same as the current username.", "OK");
                                return;
                            }
                            parameters.Add("Username = @newUsername");
                        }

                        // Check if email is provided and valid
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            if (!email.Contains("@"))
                            {
                                DisplayAlert("Input Error", "Please enter a valid email address.", "OK");
                                return;
                            }
                            parameters.Add("Email = @newEmail");
                        }

                        // Check if new password is provided and valid
                        if (!string.IsNullOrWhiteSpace(newPassword))
                        {
                            if (newPassword != confirmPassword)
                            {
                                DisplayAlert("Validation Error", "Passwords do not match.", "OK");
                                return;
                            }

                            // Hash the new password before saving
                            string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                            parameters.Add("PasswordHash = @newPassword");
                        }

                        // If no parameters were added, display an error
                        if (parameters.Count == 0)
                        {
                            DisplayAlert("Input Error", "No changes to save. Please modify at least one field.", "OK");
                            return;
                        }

                        // Build the update query dynamically based on provided inputs
                        updateQuery += string.Join(", ", parameters) + " WHERE Username = @username";

                        using (var updateCommand = new SQLiteCommand(updateQuery, connection, transaction))
                        {
                            // Add parameters to the query
                            updateCommand.Parameters.AddWithValue("@username", username);

                            if (!string.IsNullOrWhiteSpace(newUsername))
                                updateCommand.Parameters.AddWithValue("@newUsername", newUsername);

                            if (!string.IsNullOrWhiteSpace(email))
                                updateCommand.Parameters.AddWithValue("@newEmail", email);

                            if (!string.IsNullOrWhiteSpace(newPassword))
                                updateCommand.Parameters.AddWithValue("@newPassword", BCrypt.Net.BCrypt.HashPassword(newPassword));

                            // Execute the update query
                            int rowsAffected = updateCommand.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                DisplayAlert("Error", "No matching user found to update. No changes made.", "OK");
                                return;
                            }
                        }

                        // Commit the transaction if all operations succeed
                        transaction.Commit();
                    }
                }

                // Notify the user of success
                DisplayAlert("Success", "Changes saved successfully.", "OK");
            }
            catch (SQLiteException ex)
            {
                // Handle database-related errors
                DisplayAlert("Database Error", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                // Handle general errors
                DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // Validates the old password for the logged-in user
        private bool ValidatePasswordChange(string username, string oldPassword)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string verifyQuery = "SELECT PasswordHash FROM Users WHERE Username = @username";
                using (var verifyCommand = new SQLiteCommand(verifyQuery, connection))
                {
                    verifyCommand.Parameters.AddWithValue("@username", username); // Use the username parameter
                    var storedHash = verifyCommand.ExecuteScalar()?.ToString();

                    if (storedHash == null)
                    {
                        DisplayAlert("Validation Error", "User not found.", "OK");
                        return false;
                    }

                    // Verify the old password against the stored hash
                    if (!BCrypt.Net.BCrypt.Verify(oldPassword, storedHash))
                    {
                        DisplayAlert("Validation Error", "Old password is incorrect.", "OK");
                        return false;
                    }
                }
            }

            return true;
        }

        // Retrieves the currently logged-in user's username from preferences
        private string GetLoggedInUser()
        {
            return Preferences.Get("LoggedInUser", string.Empty); // Default to empty string if not set
        }

        // Validates input fields for general correctness
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
