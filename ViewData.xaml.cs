using System;
using System.Text;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using SECW.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SECW;

public partial class ViewData : ContentPage
{
    private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";
    
    public ViewData()
    {
        InitializeComponent();
        InitializeDatabase();
        LoadSensorPicker();
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

    private async void LoadSensorPicker()
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                
                var command = connection.CreateCommand();
                command.CommandText = "SELECT SensorID, SensorTypeID, SensorName FROM Sensor";
                
                var sensorItems = new List<SensorItem>();
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sensorItems.Add(new SensorItem
                        {
                            SensorID = reader.GetInt32(0),
                            SensorTypeID = reader.GetInt32(1),
                            SensorName = reader.GetString(2)
                        });
                    }
                }
                
                SensorPicker.ItemsSource = sensorItems;
                SensorPicker.SelectedIndexChanged += SensorPicker_SelectedIndexChanged;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load sensors: {ex.Message}", "OK");
            Console.WriteLine($"[ERROR] Failed to load sensors: {ex.Message}");
        }
    }

    private async void SensorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SensorPicker.SelectedItem is SensorItem selectedSensor)
        {
            await LoadSensorData(selectedSensor.SensorID, selectedSensor.SensorTypeID);
        }
    }

    private async Task LoadSensorData(int sensorId, int sensorTypeID)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                
                string query = sensorTypeID switch
                {
                    1 => "SELECT DataID, Timestamp, NO2, SO2, PM25, PM10 FROM EnvironmentalData WHERE SensorID = @sensorId",
                    2 => "SELECT DataID, Timestamp, Nitrate, Nitrite, Phosphate FROM EnvironmentalData WHERE SensorID = @sensorId",
                    3 => "SELECT DataID, Timestamp, Temp, Humidity, WindSpeed, WindDirection FROM EnvironmentalData WHERE SensorID = @sensorId",
                    _ => throw new Exception("Unknown sensor type")
                };

                command.CommandText = query;
                command.Parameters.AddWithValue("@sensorId", sensorId);
                
                var dataItems = new List<SensorDataItem>();
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var values = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            values[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        
                        dataItems.Add(new SensorDataItem
                        {
                            Values = values,
                            DisplayText = FormatDisplayText(values, sensorTypeID)
                        });
                    }
                }
                
                DataCollectionView.ItemsSource = dataItems;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load sensor data: {ex.Message}", "OK");
        }
    }

    private string FormatDisplayText(Dictionary<string, object> values, int sensorTypeID)
    {
        var sb = new StringBuilder();
        sb.AppendLine(values["Timestamp"].ToString());
        
        foreach (var kvp in values.Where(x => x.Key != "DataID" && x.Key != "Timestamp"))
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value ?? "N/A"}");
        }
        
        return sb.ToString();
    }
}

// Model classes
public class SensorItem
{
    public int SensorID { get; set; }
    public int SensorTypeID { get; set; }
    public string SensorName { get; set; }
}

public class SensorDataItem
{
    public Dictionary<string, object> Values { get; set; }
    public string DisplayText { get; set; }
}