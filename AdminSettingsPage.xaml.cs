using System;
using Microsoft.Maui.Controls;
using System.Data.SQLite;
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
            // TODO: Implement the logic to save changes made in the admin settings
        }
    }
}
