using Microsoft.Maui.Controls;
using SECW.Helpers; // Ensure the namespace for DataBaseHelper is included

namespace SECW
{
    public partial class App : Application
    {
        public App(Login loginPage)
        {
            InitializeComponent();

            Login = loginPage;
        }
    }
}
