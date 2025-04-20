namespace SECW;

public partial class AdminPage : ContentPage
{
    public AdminPage()
    {
    InitializeComponent();
    }

    // Event handlers
    private void ManageUsersBtn_Click(object sender, EventArgs e)
    {
    }

    private void ViewReportsBtn_Click(object sender, EventArgs e)
    {
    }

    private void SettingsBtn_Click(object sender, EventArgs e)
    {
    }

    private void LogoutBtn_Click(object sender, EventArgs e)
    {
        // Navigate back to the login page
        // Assuming LoginPage is the main login page of your application
        //using this method to navigate back to the login page for the sole reason of it not being a part of the navigation stack meaning admin can not go back to the admin page once logged out and has to log back in
        //another option would be to use the popasync method but that would allow the admin to go back to the admin page
        if (Application.Current != null)
        {
            Application.Current.MainPage = new NavigationPage(new Login());
        }
    }
    private void AdminSettingsBtn_Click(object sender, EventArgs e)
    {
        // Navigate to AdminSettingsPage
        // Assuming AdminSettingsPage is another page in your application
        // TODO: Implement AdminSettingsPage
        Navigation.PushAsync(new AdminSettingsPage());

    }
}
