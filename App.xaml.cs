using Microsoft.Maui.Controls;

namespace SECW
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the Login page as the main page
            MainPage = new AppShell();
        }
    }
}
