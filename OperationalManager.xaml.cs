using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SECW.Helpers;

namespace SECW;

/// <summary>
/// Represents the Operational Manager Page, which provides functionalities 
/// for managing sensors, checking battery status, and handling maintenance/calibration.
/// </summary>
public partial class OperationalManagerPage : ContentPage
{
    // Connection string for the SQLite database
    private static readonly string connectionString = "Data Source=Helpers\\SoftwareEngineering.db;";

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationalManagerPage"/> class.
    /// </summary>
    public OperationalManagerPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the click event for checking battery status of sensors.
    /// Queries the database for sensors with low battery and displays alerts accordingly.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnCheckBatteryStatusClicked(object sender, EventArgs e)
    {
        Console.WriteLine("OnCheckBatteryStatusClicked invoked.");

        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            Console.WriteLine("Database connection opened successfully.");

            // Ensure the Sensors table exists
            var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Sensors (
                    SensorID INTEGER PRIMARY KEY,
                    BatteryStatus VARCHAR(20),
                    Location VARCHAR(100),
                    Status VARCHAR(20),
                    FirmwareVersion VARCHAR(20),
                    SensorType VARCHAR(50)
                );";
            using var createTableCommand = new SqliteCommand(createTableQuery, connection);
            createTableCommand.ExecuteNonQuery();
            Console.WriteLine("Ensured Sensors table exists.");

            // Query sensors with battery status <= 40%
            var query = "SELECT SensorID, BatteryStatus, Location, Status, SensorType FROM Sensors WHERE CAST(BatteryStatus AS INTEGER) <= 40";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            bool lowBatteryFound = false;
            string lowBatterySensors = "";

            while (reader.Read())
            {
                var sensorId = reader["SensorID"].ToString();
                var batteryPercentage = int.TryParse(reader["BatteryStatus"].ToString(), out var percentage) ? percentage : -1;
                var location = reader["Location"].ToString();
                var status = reader["Status"].ToString();
                var sensorType = reader["SensorType"].ToString();

                if (batteryPercentage != -1)
                {
                    Console.WriteLine($"Sensor ID: {sensorId}, Battery Percentage: {batteryPercentage}%, Location: {location}, Status: {status}, Sensor Type: {sensorType}");

                    if (batteryPercentage <= 40)
                    {
                        lowBatteryFound = true;
                        lowBatterySensors += $"Sensor {sensorId}: {batteryPercentage}% at {location}, Status: {status}, Type: {sensorType}\n";
                    }
                }
            }

            // Display appropriate alert based on battery status
            if (lowBatteryFound)
            {
                Console.WriteLine("Low battery sensors found. Displaying alert.");
                DisplayAlert("Low Battery Alert", $"The following sensors have low battery:\n{lowBatterySensors}", "OK");
            }
            else
            {
                Console.WriteLine("All sensors have sufficient battery levels.");
                DisplayAlert("Battery Status", "All sensors have sufficient battery levels.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during battery status check: {ex.Message}");
            DisplayAlert("Error", "Failed to check battery status. Please try again.", "OK");
        }
    }

    /// <summary>
    /// Handles the click event for the Logout button.
    /// Navigates back to the login page and resets the navigation stack.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void LogoutBtn_Click(object sender, EventArgs e)
    {
        Console.WriteLine("LogoutBtn_Click invoked.");

        try
        {
            Console.WriteLine("Attempting to log out and navigate to LoginPage...");

            if (Application.Current != null)
            {
                // Navigate to LoginPage and reset navigation stack
                Application.Current.MainPage = new NavigationPage(new Login());
                Console.WriteLine("Successfully logged out and navigated to LoginPage.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the click event for checking maintenance and calibration dates.
    /// Retrieves sensors from the database and checks if maintenance or calibration is overdue.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void CheckMaintenanceAndCalibrationDates(object sender, EventArgs e)
    {
        Console.WriteLine("CheckMaintenanceAndCalibrationDates invoked.");
        DisplayAlert("Processing", "Checking maintenance and calibration dates. Please wait...", "OK");

        try
        {
            // Retrieve sensors from the database
            var sensors = SensorsHelper.GetSensors();
            Console.WriteLine($"{sensors.Count} sensors retrieved from the database.");
            DisplayAlert("Info", $"{sensors.Count} sensors retrieved from the database.", "OK");

            foreach (var sensor in sensors)
            {
                string alertMessage = string.Empty;

                // Check if the last maintenance date is older than 3 months
                if (sensor.LastMaintenanceDate.HasValue && sensor.LastMaintenanceDate.Value < DateTime.Now.AddMonths(-3))
                {
                    alertMessage += $"Maintenance required. Last maintenance date: {sensor.LastMaintenanceDate.Value.ToShortDateString()}\n";
                }

                // Check if the last calibration date is older than 3 months
                if (sensor.CalibrationDate.HasValue && sensor.CalibrationDate.Value < DateTime.Now.AddMonths(-3))
                {
                    alertMessage += $"Calibration required. Last calibration date: {sensor.CalibrationDate.Value.ToShortDateString()}\n";
                }

                // Display alert if any issues are found
                if (!string.IsNullOrEmpty(alertMessage))
                {
                    Console.WriteLine($"Sensor {sensor.SensorID} requires attention. {alertMessage}");
                    DisplayAlert(
                        "Sensor Alert",
                        $"Sensor {sensor.SensorID} ({sensor.SensorType}, located at {sensor.Location}):\n{alertMessage}",
                        "OK"
                    );
                }
            }

            Console.WriteLine("Maintenance and calibration check completed successfully.");
            DisplayAlert("Completed", "Maintenance and calibration check completed successfully.", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during maintenance and calibration check: {ex.Message}");
            DisplayAlert("Error", "An error occurred while checking maintenance and calibration dates. Please try again.", "OK");
        }
    }


//on click to move to the anomalys page
    private void OnCheckAnomalysClicked(object sender, EventArgs e)
    {
        try
        {
            // Log navigation attempt
            Console.WriteLine("Navigating to Anomalys...");

            // Navigate to Anomalys
            Navigation.PushAsync(new Anomalys());

            // Log successful navigation
            Console.WriteLine("Successfully navigated to AnomalysPage.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in OnCheckAnomalysClicked: {ex.Message}");
        }
    }

    private void OnCheckMalfunctionsClicked(object sender, EventArgs e)
    {
        try
        {
            // Log navigation attempt
            Console.WriteLine("Navigating to Malfunctions...");

            // Navigate to Malfunctions
            Navigation.PushAsync(new Malfunctions());

            // Log successful navigation
            Console.WriteLine("Successfully navigated to MalfunctionsPage.");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in OnCheckMalfunctionsClicked: {ex.Message}");
        }
    }
}