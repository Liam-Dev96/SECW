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

        LoadSensorPicker();
    }

    private async void LoadSensorPicker()
    {
        //connect to the database and Select All the sensors and there types
        // and add them to the SensorPicker
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

    // Event handler for when the selected item in the SensorPicker changes
    // This will load the data for the selected sensor
    private async void SensorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SensorPicker.SelectedItem is SensorItem selectedSensor)
        {
            await LoadSensorData(selectedSensor.SensorID, selectedSensor.SensorTypeID);
        }
    }

    // Load the data for the selected sensor from the database
    // based on the sensor type ID, it will load different data types
    // and display them in the DataCollectionView
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
                
                // create a list to hold the data items
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
                        
                        // Add the data item to the list
                        // Format the display text
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


    // Format the display text for the data item based on the sensor type ID
    private string FormatDisplayText(Dictionary<string, object> values, int sensorTypeID)
    {
        //create the display text as a string and add the timestamp to it
        var stringbuild = new StringBuilder();
        stringbuild.AppendLine(values["Timestamp"].ToString());
        
        //for each item in Value that is not Timestamp add their feild name and value to the display text
        // if the value is null, add N/A to the display text
        foreach (var keyvalue in values.Where(x => x.Key != "DataID" && x.Key != "Timestamp"))
        {
            stringbuild.AppendLine($"{keyvalue.Key}: {keyvalue.Value ?? "N/A"}");
        }
        
        return stringbuild.ToString();
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