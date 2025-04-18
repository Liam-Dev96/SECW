using Microsoft.Maui.Controls;
using SECW.Helpers; // Ensure the namespace for DataBaseHelper is included

namespace SECW
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Initialize the database at app startup
            DataBaseHelper.initializeDatabase();

            // Set the Login page as the main page
            MainPage = new AppShell();
        }
    }
}
