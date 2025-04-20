using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
using SECW.Helpers;

namespace SECW
{
    public partial class AdminSettingsPage : ContentPage
    {
        // Connection string to the SQLite database. It specifies the database file and version.
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";

        public AdminSettingsPage()
        {
            InitializeComponent(); // Initializes the UI components defined in the XAML file.
        }

        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                // Retrieve user input from the UI fields.
                string username = UsernameEntry.Text;
                string email = EmailEntry.Text;
                string oldPassword = OldPassword.Text;
                string newPassword = PasswordEntry.Text;
                string confirmPassword = ConfirmPasswordEntry.Text;

                // Check if the new password matches the confirmation password.
                if (newPassword != confirmPassword)
                {
                    DisplayAlert("Error", "Passwords do not match.", "OK"); // Notify the user of the mismatch.
                    return; // Exit the method early to prevent further processing.
                }

                // Establish a connection to the SQLite database.
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open(); // Open the database connection.

                    // Query to retrieve the stored password for the given username.
                    string verifyQuery = @"SELECT Password FROM Users WHERE Username = @username";
                    using (var verifyCommand = new SQLiteCommand(verifyQuery, connection))
                    {
                        // Add the username parameter to prevent SQL injection.
                        verifyCommand.Parameters.AddWithValue("@username", username);

                        // Execute the query and retrieve the stored password.
                        var storedPassword = verifyCommand.ExecuteScalar()?.ToString();
                        if (storedPassword == null || storedPassword != oldPassword)
                        {
                            // If the username doesn't exist or the old password doesn't match, notify the user.
                            DisplayAlert("Error", "Old password is incorrect.", "OK");
                            return; // Exit the method early.
                        }
                    }

                    // Query to update the user's email and password in the database.
                    string updateQuery = @"UPDATE Users SET Email = @newEmail, Password = @newPassword WHERE Username = @username";
                    using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        // Add parameters to the query to prevent SQL injection.
                        updateCommand.Parameters.AddWithValue("@username", username);
                        updateCommand.Parameters.AddWithValue("@newEmail", email);
                        updateCommand.Parameters.AddWithValue("@newPassword", newPassword);

                        // Execute the update query.
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Notify the user of success and log the event.
                DisplayAlert("Success", "Changes saved successfully.", "OK");
                Console.WriteLine("[INFO] Changes saved successfully.");
            }
            catch (SQLiteException ex)
            {
                // Handle database-related errors and notify the user.
                DisplayAlert("Error", "Database error: " + ex.Message, "OK");
                Console.WriteLine($"[ERROR] Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general errors and notify the user.
                DisplayAlert("Error", "An error occurred while saving changes: " + ex.Message, "OK");
                Console.WriteLine($"[ERROR] Error while applying change. {ex.Message}");
            }
        }
    }
}

