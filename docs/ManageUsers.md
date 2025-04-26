"# Manage Users Page"

The **Manage Users Page** allows administrators to view, add, edit, and delete user accounts within the system. Below is an overview of the key functionalities and their implementation details.

## Features

1. **View Users**: Displays a list of all registered users in a table format.
2. **Add User**: Provides a form to input details for creating a new user.
3. **Edit User**: Allows modification of existing user details.
4. **Delete User**: Enables the removal of user accounts.

## Implementation Details

### XAML Structure

The `ManageUsers.xaml` file defines the UI layout using XAML. Key components include:
- A `DataGrid` for displaying the list of users.
- Buttons for "Add", "Edit", and "Delete" actions.
- A `Popup` or `Dialog` for user input when adding or editing users.

### Code-Behind Logic

The `ManageUsers.xaml.cs` file contains the event handlers and logic for user interactions:
- **Loading Users**: Fetches user data from the database and binds it to the `DataGrid`.
- **Add User**: Opens a dialog for input, validates the data, and saves it to the database.
- **Edit User**: Pre-fills the dialog with selected user data, validates changes, and updates the database.
- **Delete User**: Confirms the action and removes the user from the database.

### Example Code Snippets

#### XAML
```xml
<DataGrid x:Name="UsersDataGrid" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Username" Binding="{Binding Username}" />
        <DataGridTextColumn Header="Email" Binding="{Binding Email}" />
        <DataGridTextColumn Header="Role" Binding="{Binding Role}" />
    </DataGrid.Columns>
</DataGrid>
<Button Content="Add User" Click="AddUser_Click" />
<Button Content="Edit User" Click="EditUser_Click" />
<Button Content="Delete User" Click="DeleteUser_Click" />
```

#### Code-Behind
```csharp
private void AddUser_Click(object sender, RoutedEventArgs e)
{
    var addUserDialog = new AddUserDialog();
    if (addUserDialog.ShowDialog() == true)
    {
        // Save new user to database
        LoadUsers();
    }
}

private void EditUser_Click(object sender, RoutedEventArgs e)
{
    if (UsersDataGrid.SelectedItem is User selectedUser)
    {
        var editUserDialog = new EditUserDialog(selectedUser);
        if (editUserDialog.ShowDialog() == true)
        {
            // Update user in database
            LoadUsers();
        }
    }
}

private void DeleteUser_Click(object sender, RoutedEventArgs e)
{
    if (UsersDataGrid.SelectedItem is User selectedUser)
    {
        // Confirm and delete user
        LoadUsers();
    }
}
```

## Conclusion

The **Manage Users Page** provides a user-friendly interface for managing user accounts. The combination of XAML for UI and C# for logic ensures a seamless experience for administrators.
