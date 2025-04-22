using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SECW
{
    public partial class ManageUsers : ContentPage
    {
        public ObservableCollection<User> Users { get; set; }
        public ICommand DeleteUserCommand { get; }

        public ManageUsers()
        {
            InitializeComponent();

            // Initialize the Users collection
            Users = new ObservableCollection<User>
            {
                new User { Name = "John Doe", Email = "john.doe@example.com" },
                new User { Name = "Jane Smith", Email = "jane.smith@example.com" }
            };

            // Initialize the DeleteUserCommand
            DeleteUserCommand = new Command<User>(DeleteUser);

            BindingContext = this;
        }

        private void DeleteUser(User user)
        {
            if (user != null && Users.Contains(user))
            {
                Users.Remove(user);
            }
        }
    }

    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}