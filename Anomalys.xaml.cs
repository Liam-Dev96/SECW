using System;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using SECW.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SECW;

public partial class Anomalys : ContentPage
{
    private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";
    private SqliteConnection _connection;
    private Func<string, string, string, Task> _displayAlert;

    public Anomalys()
    {
        InitializeComponent();
        AnomalysList();
        
        // Add item selected handler
        AnomalysListbox.ItemSelected += OnAnomalySelected;
    }

    public void SetConnection(SqliteConnection connection)
    {
        _connection = connection;
    }

    public void SetDisplayAlert(Func<string, string, string, Task> displayAlert)
    {
        _displayAlert = displayAlert;
    }

    // Load the list of anomalies from the database and display them in the ListBox
    // The list is populated with the SensorName and Timestamp of each anomaly
    // Create the reason and details of the anomaly from the data in the database
    public void AnomalysList()
    {
        var anomalies = new List<AnomalyItem>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            string query = @"SELECT 
                                S.SensorName, 
                                ED.Timestamp,
                                ST.SensorTypeName,
                                CASE
                                    WHEN S.SensorTypeID = 1 AND (ED.NO2 < ST.Data1Min OR ED.NO2 > ST.Data1Max) THEN 'NO2 out of range'
                                    WHEN S.SensorTypeID = 1 AND (ED.SO2 < ST.Data2Min OR ED.SO2 > ST.Data2Max) THEN 'SO2 out of range'
                                    WHEN S.SensorTypeID = 1 AND (ED.PM25 < ST.Data3Min OR ED.PM25 > ST.Data3Max) THEN 'PM2.5 out of range'
                                    WHEN S.SensorTypeID = 1 AND (ED.PM10 < ST.Data4Min OR ED.PM10 > ST.Data4Max) THEN 'PM10 out of range'
                                    WHEN S.SensorTypeID = 2 AND (ED.Nitrate < ST.Data1Min OR ED.Nitrate > ST.Data1Max) THEN 'Nitrate out of range'
                                    WHEN S.SensorTypeID = 2 AND (ED.Nitrite < ST.Data2Min OR ED.Nitrite > ST.Data2Max) THEN 'Nitrite out of range'
                                    WHEN S.SensorTypeID = 2 AND (ED.Phosphate < ST.Data3Min OR ED.Phosphate > ST.Data3Max) THEN 'Phosphate out of range'
                                    WHEN S.SensorTypeID = 3 AND (ED.Temp < ST.Data1Min OR ED.Temp > ST.Data1Max) THEN 'Temperature out of range'
                                    WHEN S.SensorTypeID = 3 AND (ED.Humidity < ST.Data2Min OR ED.Humidity > ST.Data2Max) THEN 'Humidity out of range'
                                    WHEN S.SensorTypeID = 3 AND (ED.WindSpeed < ST.Data3Min OR ED.WindSpeed > ST.Data3Max) THEN 'Wind speed out of range'
                                END AS AnomalyReason,
                                CASE
                                    WHEN S.SensorTypeID = 1 THEN 
                                        CASE
                                            WHEN ED.NO2 < ST.Data1Min OR ED.NO2 > ST.Data1Max THEN 'NO2: ' || ED.NO2 || ' (Range: ' || ST.Data1Min || '-' || ST.Data1Max || ')'
                                            WHEN ED.SO2 < ST.Data2Min OR ED.SO2 > ST.Data2Max THEN 'SO2: ' || ED.SO2 || ' (Range: ' || ST.Data2Min || '-' || ST.Data2Max || ')'
                                            WHEN ED.PM25 < ST.Data3Min OR ED.PM25 > ST.Data3Max THEN 'PM2.5: ' || ED.PM25 || ' (Range: ' || ST.Data3Min || '-' || ST.Data3Max || ')'
                                            WHEN ED.PM10 < ST.Data4Min OR ED.PM10 > ST.Data4Max THEN 'PM10: ' || ED.PM10 || ' (Range: ' || ST.Data4Min || '-' || ST.Data4Max || ')'
                                        END
                                    WHEN S.SensorTypeID = 2 THEN 
                                        CASE
                                            WHEN ED.Nitrate < ST.Data1Min OR ED.Nitrate > ST.Data1Max THEN 'Nitrate: ' || ED.Nitrate || ' (Range: ' || ST.Data1Min || '-' || ST.Data1Max || ')'
                                            WHEN ED.Nitrite < ST.Data2Min OR ED.Nitrite > ST.Data2Max THEN 'Nitrite: ' || ED.Nitrite || ' (Range: ' || ST.Data2Min || '-' || ST.Data2Max || ')'
                                            WHEN ED.Phosphate < ST.Data3Min OR ED.Phosphate > ST.Data3Max THEN 'Phosphate: ' || ED.Phosphate || ' (Range: ' || ST.Data3Min || '-' || ST.Data3Max || ')'
                                        END
                                    WHEN S.SensorTypeID = 3 THEN 
                                        CASE
                                            WHEN ED.Temp < ST.Data1Min OR ED.Temp > ST.Data1Max THEN 'Temperature: ' || ED.Temp || ' (Range: ' || ST.Data1Min || '-' || ST.Data1Max || ')'
                                            WHEN ED.Humidity < ST.Data2Min OR ED.Humidity > ST.Data2Max THEN 'Humidity: ' || ED.Humidity || ' (Range: ' || ST.Data2Min || '-' || ST.Data2Max || ')'
                                            WHEN ED.WindSpeed < ST.Data3Min OR ED.WindSpeed > ST.Data3Max THEN 'Wind Speed: ' || ED.WindSpeed || ' (Range: ' || ST.Data3Min || '-' || ST.Data3Max || ')'
                                        END
                                END AS AnomalyDetails
                            FROM Sensor S
                            JOIN EnvironmentalData ED ON S.SensorID = ED.SensorID
                            JOIN SensorType ST ON S.SensorTypeID = ST.SensorTypeID
                            WHERE 
                                (S.SensorTypeID = 1 AND (ED.NO2 < ST.Data1Min OR ED.NO2 > ST.Data1Max)) OR 
                                (S.SensorTypeID = 1 AND (ED.SO2 < ST.Data2Min OR ED.SO2 > ST.Data2Max)) OR
                                (S.SensorTypeID = 1 AND (ED.PM25 < ST.Data3Min OR ED.PM25 > ST.Data3Max)) OR
                                (S.SensorTypeID = 1 AND (ED.PM10 < ST.Data4Min OR ED.PM10 > ST.Data4Max)) OR
                                (S.SensorTypeID = 2 AND (ED.Nitrate < ST.Data1Min OR ED.Nitrate > ST.Data1Max)) OR
                                (S.SensorTypeID = 2 AND (ED.Nitrite < ST.Data2Min OR ED.Nitrite > ST.Data2Max)) OR
                                (S.SensorTypeID = 2 AND (ED.Phosphate < ST.Data3Min OR ED.Phosphate > ST.Data3Max)) OR
                                (S.SensorTypeID = 3 AND (ED.Temp < ST.Data1Min OR ED.Temp > ST.Data1Max)) OR
                                (S.SensorTypeID = 3 AND (ED.Humidity < ST.Data2Min OR ED.Humidity > ST.Data2Max)) OR
                                (S.SensorTypeID = 3 AND (ED.WindSpeed < ST.Data3Min OR ED.WindSpeed > ST.Data3Max))";

            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        anomalies.Add(new AnomalyItem 
                        { 
                            DisplayText = $"{reader["SensorName"]} - {reader["Timestamp"]}",
                            SensorName = reader["SensorName"].ToString(),
                            Timestamp = reader["Timestamp"].ToString(),
                            SensorType = reader["SensorTypeName"].ToString(),
                            AnomalyReason = reader["AnomalyReason"].ToString(),
                            AnomalyDetails = reader["AnomalyDetails"].ToString()
                        });
                    }
                }
            }
        }
        
        AnomalysListbox.ItemsSource = anomalies;
    }

    // Event handler for when an item in the ListBox is selected
    // This will show detailed information about the selected anomaly
    // The details include the sensor name, timestamp, reason, and specific details of the anomaly
    // After displaying the details, it will deselect the item in the ListBox
    public void OnAnomalySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedAnomaly = e.SelectedItem as AnomalyItem;
        
        // Use injected DisplayAlert for testing
        if (_displayAlert != null)
        {
            _displayAlert(
                $"Anomaly Details - {selectedAnomaly.SensorName}",
                $"Sensor Type: {selectedAnomaly.SensorType}\n" +
                $"Timestamp: {selectedAnomaly.Timestamp}\n" +
                $"Reason: {selectedAnomaly.AnomalyReason}\n" +
                $"Details: {selectedAnomaly.AnomalyDetails}",
                "OK").ConfigureAwait(false);
        }
        else
        {
            DisplayAlert(
                $"Anomaly Details - {selectedAnomaly.SensorName}",
                $"Sensor Type: {selectedAnomaly.SensorType}\n" +
                $"Timestamp: {selectedAnomaly.Timestamp}\n" +
                $"Reason: {selectedAnomaly.AnomalyReason}\n" +
                $"Details: {selectedAnomaly.AnomalyDetails}",
                "OK").ConfigureAwait(false);
        }

        // Deselect the item
        AnomalysListbox.SelectedItem = null;
    }

    public class AnomalyItem
    {
        public string DisplayText { get; set; }
        public string SensorName { get; set; }
        public string Timestamp { get; set; }
        public string SensorType { get; set; }
        public string AnomalyReason { get; set; }
        public string AnomalyDetails { get; set; }
    }
}