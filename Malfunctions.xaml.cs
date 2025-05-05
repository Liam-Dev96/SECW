using System;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using SECW.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SECW;

public partial class Malfunctions : ContentPage
{
    private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";
    private SqliteConnection _connection;
    private Func<string, string, string, Task> _displayAlert;

    public Malfunctions()
    {
        InitializeComponent();
        MalfunctionsList();
        
        // Add item selected handler
        MalfunctionsListbox.ItemSelected += OnMalfunctionsSelected;
    }

    public void SetConnection(SqliteConnection connection)
    {
        _connection = connection;
    }

    public void SetDisplayAlert(Func<string, string, string, Task> displayAlert)
    {
        _displayAlert = displayAlert;
    }

    // Load the list of malfunctions from the database and display them in the ListBox
    // The list is populated with the SensorName and Timestamp of each malfunction
    public void MalfunctionsList()
    {
        var malfunctions = new List<MalfunctionsItem>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            string query = @"SELECT 
                                M.MalfunctionsID,
                                S.SensorName, 
                                M.Timestamp,
                                M.Notes
                            FROM Malfunctions M
                            JOIN Sensor S ON S.SensorID = M.SensorID";

            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        malfunctions.Add(new MalfunctionsItem 
                        { 
                            DisplayText = $"{reader["SensorName"]} - {reader["Timestamp"]}",
                            SensorName = reader["SensorName"].ToString(),
                            Timestamp = DateTime.Parse(reader["Timestamp"].ToString()).ToString("g"),
                            Notes = reader["Notes"].ToString(),
                            MalfunctionsID = reader["MalfunctionsID"].ToString()
                        });
                    }
                }
            }
        }
        MalfunctionsListbox.ItemsSource = malfunctions;
    }

    // Event handler for when an item in the ListBox is selected
    // Displays an action sheet with options to view details or mark as resolved
    // If the user selects "View Details", a detailed view of the malfunction is shown
    // If the user selects "Mark as Resolved", a confirmation dialog is shown
    // If confirmed, the malfunction is marked as resolved in the database
    // and the list is refreshed
    public async void OnMalfunctionsSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedMalfunction = e.SelectedItem as MalfunctionsItem;
        
        // Show action sheet with options
        string action = await DisplayActionSheet(
            $"Malfunction: {selectedMalfunction.SensorName}",
            "Cancel",
            null,
            "View Details",
            "Mark as Resolved");

        if (action == "View Details")
        {
            // Use injected DisplayAlert for testing
            if (_displayAlert != null)
            {
                await _displayAlert(
                    $"Malfunction Details - {selectedMalfunction.SensorName}",
                    $"Timestamp: {selectedMalfunction.Timestamp}\nNotes: {selectedMalfunction.Notes}",
                    "OK");
            }
            else
            {
                await DisplayAlert(
                    $"Malfunction Details - {selectedMalfunction.SensorName}",
                    $"Timestamp: {selectedMalfunction.Timestamp}\nNotes: {selectedMalfunction.Notes}",
                    "OK");
            }
        }
        else if (action == "Mark as Resolved")
        {
            bool confirm = await DisplayAlert(
                "Confirm Resolution",
                "Mark this malfunction as resolved?",
                "Yes", "No");
            
            if (confirm)
            {
                OnResolvedClicked(selectedMalfunction);
            }
        }

        // Deselect the item
        MalfunctionsListbox.SelectedItem = null;
    }

    // Mark the selected malfunction as resolved in the database
    // by deleting it from the Malfunctions table
    public async Task OnResolvedClicked(MalfunctionsItem malfunction)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Malfunctions WHERE MalfunctionsID = @MalfunctionsID";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MalfunctionsID", malfunction.MalfunctionsID);
                    int rowsAffected = command.ExecuteNonQuery();
                    
                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("Success", "Malfunction marked as resolved", "OK");
                        // Refresh the list
                        MalfunctionsList();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to resolve malfunction", "OK");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to resolve malfunction: {ex.Message}", "OK");
        }
    }

    public class MalfunctionsItem
    {
        public string DisplayText { get; set; }
        public string SensorName { get; set; }
        public string Timestamp { get; set; }
        public string Notes { get; set; }
        public string MalfunctionsID { get; set; }
    }
}