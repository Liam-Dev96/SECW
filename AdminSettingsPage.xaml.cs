using System;
using Microsoft.Maui.Controls;
using Microsoft.Data.Sqlite;
using BCrypt.Net;
using SECW.Helpers;

namespace SECW
{
    /// <summary>
    /// Represents the Admin Settings Page where users can update their account details.
    /// </summary>
    public partial class AdminSettingsPage : ContentPage
    {
        // Connection string for SQLite database
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminSettingsPage"/> class.
        /// </summary>
        public AdminSettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the "Save Changes" button click.
        /// Updates the user's account details in the database.
        /// </summary>
        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Save Changes button clicked. Starting validation...");

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
                    Console.WriteLine("Validation failed: Username is empty.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    DisplayAlert("Input Error", "Old password is required.", "OK");
                    Console.WriteLine("Validation failed: Old password is empty.");
                    return;
                }

                // Validate the old password before proceeding
                if (!ValidatePasswordChange(username, oldPassword))
                {
                    Console.WriteLine("Validation failed: Old password is incorrect.");
                    return; // Validation failed, error already displayed
                }

                Console.WriteLine("Validation passed. Proceeding with database update...");

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

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
                                Console.WriteLine("Validation failed: New username is the same as the current username.");
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
                                Console.WriteLine("Validation failed: Invalid email address.");
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
                                Console.WriteLine("Validation failed: Passwords do not match.");
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
                            Console.WriteLine("Validation failed: No changes to save.");
                            return;
                        }

                        // Build the update query dynamically based on provided inputs
                        updateQuery += string.Join(", ", parameters) + " WHERE Username = @username";

                        using (var updateCommand = new SqliteCommand(updateQuery, connection, transaction))
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
                                Console.WriteLine("Database update failed: No matching user found.");
                                return;
                            }
                        }

                        // Commit the transaction if all operations succeed
                        transaction.Commit();
                        Console.WriteLine("Database transaction committed successfully.");
                    }
                }

                // Notify the user of success
                DisplayAlert("Success", "Changes saved successfully.", "OK");
                Console.WriteLine("Changes saved successfully.");
                loggedInUser = newUsername; // Update the logged-in user if username changed
                Preferences.Set("LoggedInUser", loggedInUser); // Update the stored username in preferences
                //clear input fields after successful update
                UsernameEntry.Text = string.Empty;
                EmailEntry.Text = string.Empty;
                OldPassword.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                ConfirmPasswordEntry.Text = string.Empty;
                Console.WriteLine("Input fields cleared after successful update.");
            }
            catch (SqliteException ex)
            {
                // Handle database-related errors
                DisplayAlert("Database Error", ex.Message, "OK");
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors
                DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                Console.WriteLine($"General error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates the old password for the logged-in user.
        /// </summary>
        /// <param name="username">The username of the logged-in user.</param>
        /// <param name="oldPassword">The old password to validate.</param>
        /// <returns>True if the old password is valid; otherwise, false.</returns>
        private bool ValidatePasswordChange(string username, string oldPassword)
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened for password validation.");

                    string verifyQuery = "SELECT PasswordHash FROM Users WHERE Username = @username";
                    using (var verifyCommand = new SqliteCommand(verifyQuery, connection))
                    {
                        verifyCommand.Parameters.AddWithValue("@username", username); // Use the username parameter
                        var storedHash = verifyCommand.ExecuteScalar()?.ToString();

                        if (storedHash == null)
                        {
                            DisplayAlert("Validation Error", "User not found.", "OK");
                            Console.WriteLine("Password validation failed: User not found.");
                            return false;
                        }

                        // Verify the old password against the stored hash
                        if (!BCrypt.Net.BCrypt.Verify(oldPassword, storedHash))
                        {
                            DisplayAlert("Validation Error", "Old password is incorrect.", "OK");
                            Console.WriteLine("Password validation failed: Old password is incorrect.");
                            return false;
                        }
                    }
                }

                Console.WriteLine("Password validation succeeded.");
                return true;
            }
            catch (SqliteException ex)
            {
                DisplayAlert("Database Error", ex.Message, "OK");
                Console.WriteLine($"Database error during password validation: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                Console.WriteLine($"General error during password validation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the currently logged-in user's username from preferences.
        /// </summary>
        /// <returns>The username of the logged-in user.</returns>
        private string GetLoggedInUser()
        {
            try
            {
                string loggedInUser = Preferences.Get("LoggedInUser", string.Empty); // Default to empty string if not set
                Console.WriteLine($"Retrieved logged-in user: {loggedInUser}");
                return loggedInUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving logged-in user: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
