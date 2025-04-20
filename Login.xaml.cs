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
                                    await DisplayAlert("Success", $"Welcome, Admin {username}!", "OK");
                                    Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as Admin.");
                                    // Assuming the AdminPage.xaml.cs file for the admin interface exsists
                                    // and is properly set up to handle admin functionalities.
                                    await this.Navigation.PopAsync(); // Close the login page
                                    //add the date into the users table last login date
                                    updateLastLoginDate(username);
                                    try{
                                        if (Application.Current != null)
                                    {
                                        Application.Current.MainPage = new NavigationPage(new AdminPage());
                                    }
                                    else
                                    {
                                        Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to AdminPage.");
                                    }
                                    }catch(Exception ex){
                                        Console.WriteLine($"[ERROR] Failed to navigate to AdminPage: {ex.Message}");
                                    }
                                    connection.Close();
                                    break;
                                case 2:
                                    await DisplayAlert("Success", $"Welcome, User: {username}!", "OK");
                                    Console.WriteLine($"[INFO] Login successful: Username '{username}' logged in as User.");
                                    // Assuming the UserPage.xaml.cs file for the user interface exists
                                    // and is properly set up to handle user functionalities.
                                    await this.Navigation.PopAsync(); // Close the login page
                                    //add the date into the users table last login date
                                    updateLastLoginDate(username);
                                    if (Application.Current != null)
                                    {
                                        Application.Current.MainPage = new NavigationPage(new UserPage());
                                    }
                                    else
                                    {
                                        Console.WriteLine("[ERROR] Application.Current is null. Cannot navigate to UserPage.");
                                    }
                                    connection.Close();
                                    break;
                                // Assuming the Guest role is represented by RoleID 3

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
            // Handle any exceptions that may occur during the database operation
            // and log them to the console for debugging purposes.
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
    

// This method updates the last login date for a user in the database.
    // It is called after a successful login to track when the user last logged in.
    // The method uses a SQLite connection to execute an UPDATE SQL command and is constructed to handle exceptions gracefully.
    // It also logs the success or failure of the operation to the console for debugging purposes.
    // The method is private and static, meaning it can only be called within this class and does not require an instance of the class to be invoked.
    // The method takes a string parameter 'username' which is the username of the user whose last login date is to be updated.
    private static void updateLastLoginDate(string username)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            try{
            string updateQuery = @"UPDATE Users SET LastLoginDate = @lastLoginDate WHERE Username = @username";
            using (var updateCommand = new SQLiteCommand(updateQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@lastLoginDate", DateTime.Now);
                updateCommand.Parameters.AddWithValue("@username", username);
                updateCommand.ExecuteNonQuery();
            }
            }
            // Handle any exceptions that may occur during the database operation
            // and log them to the console for debugging purposes.
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