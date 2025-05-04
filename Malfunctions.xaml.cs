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

    public Malfunctions()
    {
        InitializeComponent();
        InitializeDatabase();
        MalfunctionsList();
        
        // Add item selected handler
        MalfunctionsListbox.ItemSelected += OnMalfunctionsSelected;
    }

    private void InitializeDatabase()
    {
        try
        {
            DataBaseHelper.initializeDatabase();
            Console.WriteLine("[INFO] Database initialized successfully.");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to initialize the database: {ex.Message}", "OK");
            Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
        }
    }

    private void MalfunctionsList()
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

    private async void OnMalfunctionsSelected(object sender, SelectedItemChangedEventArgs e)
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
            await DisplayAlert(
                $"Malfunction Details - {selectedMalfunction.SensorName}",
                $"Timestamp: {selectedMalfunction.Timestamp}\n" +
                $"Notes: {selectedMalfunction.Notes}",
                "OK");
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

    private async void OnResolvedClicked(MalfunctionsItem malfunction)
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