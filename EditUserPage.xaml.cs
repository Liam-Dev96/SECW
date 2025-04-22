using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace SECW
{
    public partial class EditUserPage : ContentPage
    {
        private User _user;
        private Action<User> _onSaveCallback;

        public EditUserPage(User user, Action<User> onSaveCallback)
        {
            InitializeComponent();

            _user = user;
            _onSaveCallback = onSaveCallback;

            // Populate fields with user details
            UsernameEntry.Text = user.Name;
            EmailEntry.Text = user.Email;

            // Populate RolePicker
            RolePicker.ItemsSource = new List<string> { "Admin", "Environmental Scientist", "Operations Manager" };
            RolePicker.SelectedItem = user.RoleName;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            // Update user details
            _user.Name = UsernameEntry.Text;
            _user.Email = EmailEntry.Text;
            _user.RoleName = RolePicker.SelectedItem?.ToString() ?? "No Role";

            // Call the callback to save changes
            _onSaveCallback?.Invoke(_user);

            // Close the modal
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Close the modal without saving
            await Navigation.PopModalAsync();
        }
    }
}