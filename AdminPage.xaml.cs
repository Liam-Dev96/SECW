// Admin Page

// The `AdminPage` class represents the main page for administrators. It provides the following functionalities:
// - **Manage Users**: Navigate to the `ManageUsersPage` to manage user accounts.
// - **View Reports**: Placeholder for viewing reports (not yet implemented).
// - **Settings**: Placeholder for settings (not yet implemented).
// - **Logout**: Logs out the user and navigates back to the login page.
// - **Admin Settings**: Navigate to the `AdminSettingsPage` for admin-specific settings.
// - **Check Maintenance and Calibration Dates**: Placeholder for checking maintenance and calibration dates (not yet implemented).

namespace SECW;

public partial class AdminPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPage"/> class.
    /// </summary>
    public AdminPage()
    {
        InitializeComponent();
    }

    // Event handlers

    /// <summary>
    /// Handles the click event for the Manage Users button.
    /// Navigates to the ManageUsersPage.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void ManageUsersBtn_Click(object sender, EventArgs e)
    {
        try
        {
            // Log navigation attempt
            Console.WriteLine("Navigating to ManageUsersPage...");

            // Navigate to ManageUsersPage
            Navigation.PushAsync(new ManageUsers());

            // Log successful navigation
            Console.WriteLine("Successfully navigated to ManageUsersPage.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error navigating to ManageUsersPage: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event for the View Reports button.
    /// TODO: Implement functionality for viewing reports.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void ViewReportsBtn_Click(object sender, EventArgs e)
    {
        try
        {
            // Log placeholder action
            Console.WriteLine("ViewReportsBtn_Click invoked. Functionality not yet implemented.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in ViewReportsBtn_Click: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event for the Settings button.
    /// TODO: Implement functionality for settings.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void SettingsBtn_Click(object sender, EventArgs e)
    {
        try
        {
                        // Log navigation attempt
            Console.WriteLine("Navigating to UpdateSenser...");

            // Navigate to UpdateSenser
            Navigation.PushAsync(new UpdateSenser());

            // Log successful navigation
            Console.WriteLine("Successfully navigated to UpdateSencerPage.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in SettingsBtn_Click: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event for the Logout button.
    /// Navigates back to the login page and resets the navigation stack.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void LogoutBtn_Click(object sender, EventArgs e)
    {
        try
        {
            // Log logout attempt
            Console.WriteLine("Attempting to log out and navigate to LoginPage...");

            if (Application.Current != null)
            {
                // Navigate to LoginPage and reset navigation stack
                Application.Current.MainPage = new NavigationPage(new Login());

                // Log successful logout
                Console.WriteLine("Successfully logged out and navigated to LoginPage.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error during logout: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event for the Admin Settings button.
    /// Navigates to the AdminSettingsPage.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void AdminSettingsBtn_Click(object sender, EventArgs e)
    {
        try
        {
            // Log navigation attempt
            Console.WriteLine("Navigating to AdminSettingsPage...");

            // Display alert to inform the user
            DisplayAlert("Admin Settings", 
                "Navigating to Admin Settings Page\nThis is the page where the admin can change their credentials.\nIf credentials change is attempted without the old password, the changes will not take effect and an error will pop up.", 
                "Understood");

            // Navigate to AdminSettingsPage
            Navigation.PushAsync(new AdminSettingsPage());

            // Log successful navigation
            Console.WriteLine("Successfully navigated to AdminSettingsPage.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error navigating to AdminSettingsPage: {ex.Message}");
        }
    }
}
