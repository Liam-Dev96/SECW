using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SECW.Helpers
{
    public static class SensorsHelper
    {
        private static readonly string connectionString = "Data Source=Helpers\\SoftwareEngineering.db;";

        /// <summary>
        /// Retrieves all sensors from the database.
        /// </summary>
        /// <returns>List of sensors.</returns>
        public static List<Sensor> GetSensors()
        {
            var sensors = new List<Sensor>();
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("Database connection opened successfully.");

                string query = "SELECT * FROM Sensors";
                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();
                Console.WriteLine("Executing query to fetch sensors.");

                while (reader.Read())
                {
                    sensors.Add(new Sensor
                    {
                        SensorID = reader.GetInt32(0),
                        Status = reader.GetString(1),
                        FirmwareVersion = reader.GetString(2),
                        SensorType = reader.GetString(3),
                        Location = reader.GetString(4),
                        Manufacturer = reader.GetString(5),
                        Model = reader.GetString(6),
                        SerialNumber = reader.GetString(7),
                        CalibrationDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        LastMaintenanceDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                        SignalStrength = reader.GetString(11),
                        DataRate = reader.GetString(12),
                        DataFormat = reader.GetString(13),
                        CommunicationProtocol = reader.GetString(14),
                        PowerSource = reader.GetString(15),
                        OperatingTemperatureRange = reader.GetString(16),
                        HumidityRange = reader.GetString(17),
                        PressureRange = reader.GetString(18),
                        MeasurementRange = reader.GetString(19),
                        MeasurementUnits = reader.GetString(20),
                        MeasurementAccuracy = reader.GetString(21),
                        MeasurementResolution = reader.GetString(22),
                        MeasurementInterval = reader.GetString(23),
                        DataStorageCapacity = reader.GetString(24),
                        DataTransmissionInterval = reader.GetString(25),
                        DataTransmissionMethod = reader.GetString(26),
                        DataEncryption = reader.GetString(27),
                        DataCompression = reader.GetString(28),
                        DataBackup = reader.GetString(29),
                        DataRecovery = reader.GetString(30),
                        DataVisualization = reader.GetString(31),
                        DataAnalysis = reader.GetString(32),
                        DataReporting = reader.GetString(33),
                        DataSharing = reader.GetString(34),
                        DataIntegration = reader.GetString(35),
                        DataStorageLocation = reader.GetString(36),
                        DataAccessControl = reader.GetString(37),
                        DataRetentionPolicy = reader.GetString(38),
                        DataDisposalPolicy = reader.GetString(39),
                        DataSecurity = reader.GetString(40),
                        DataPrivacy = reader.GetString(41),
                        DataCompliance = reader.GetString(42),
                        DataGovernance = reader.GetString(43),
                        DataQuality = reader.GetString(44),
                        DataIntegrity = reader.GetString(45)
                    });
                }

                Console.WriteLine($"Successfully retrieved {sensors.Count} sensors.");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error retrieving sensors: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return sensors;
        }

        /// <summary>
        /// Adds a new sensor to the database.
        /// </summary>
        /// <param name="sensor">Sensor object to add.</param>
        public static void AddSensor(Sensor sensor)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("Database connection opened successfully.");

                string query = @"
                INSERT INTO Sensors (
                    SensorID, Status, FirmwareVersion, SensorType, Location, Manufacturer, Model, SerialNumber,
                    CalibrationDate, LastMaintenanceDate, BatteryStatus, SignalStrength, DataRate, DataFormat,
                    CommunicationProtocol, PowerSource, OperatingTemperatureRange, HumidityRange, PressureRange,
                    MeasurementRange, MeasurementUnits, MeasurementAccuracy, MeasurementResolution, MeasurementInterval,
                    DataStorageCapacity, DataTransmissionInterval, DataTransmissionMethod, DataEncryption, DataCompression,
                    DataBackup, DataRecovery, DataVisualization, DataAnalysis, DataReporting, DataSharing, DataIntegration,
                    DataStorageLocation, DataAccessControl, DataRetentionPolicy, DataDisposalPolicy, DataSecurity, DataPrivacy,
                    DataCompliance, DataGovernance, DataQuality, DataIntegrity
                ) VALUES (
                    @SensorID, @Status, @FirmwareVersion, @SensorType, @Location, @Manufacturer, @Model, @SerialNumber,
                    @CalibrationDate, @LastMaintenanceDate, @BatteryStatus, @SignalStrength, @DataRate, @DataFormat,
                    @CommunicationProtocol, @PowerSource, @OperatingTemperatureRange, @HumidityRange, @PressureRange,
                    @MeasurementRange, @MeasurementUnits, @MeasurementAccuracy, @MeasurementResolution, @MeasurementInterval,
                    @DataStorageCapacity, @DataTransmissionInterval, @DataTransmissionMethod, @DataEncryption, @DataCompression,
                    @DataBackup, @DataRecovery, @DataVisualization, @DataAnalysis, @DataReporting, @DataSharing, @DataIntegration,
                    @DataStorageLocation, @DataAccessControl, @DataRetentionPolicy, @DataDisposalPolicy, @DataSecurity, @DataPrivacy,
                    @DataCompliance, @DataGovernance, @DataQuality, @DataIntegrity
                );";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@SensorID", sensor.SensorID);
                command.Parameters.AddWithValue("@Status", sensor.Status ?? string.Empty);
                command.Parameters.AddWithValue("@FirmwareVersion", sensor.FirmwareVersion ?? string.Empty);
                command.Parameters.AddWithValue("@SensorType", sensor.SensorType ?? string.Empty);
                command.Parameters.AddWithValue("@Location", sensor.Location ?? string.Empty);
                command.Parameters.AddWithValue("@Manufacturer", sensor.Manufacturer ?? string.Empty);
                command.Parameters.AddWithValue("@Model", sensor.Model ?? string.Empty);
                command.Parameters.AddWithValue("@SerialNumber", sensor.SerialNumber ?? string.Empty);
                command.Parameters.AddWithValue("@CalibrationDate", sensor.CalibrationDate.HasValue ? sensor.CalibrationDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@LastMaintenanceDate", sensor.LastMaintenanceDate.HasValue ? sensor.LastMaintenanceDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@BatteryStatus", sensor.BatteryStatus ?? string.Empty);
                command.Parameters.AddWithValue("@SignalStrength", sensor.SignalStrength ?? string.Empty);
                command.Parameters.AddWithValue("@DataRate", sensor.DataRate ?? string.Empty);
                command.Parameters.AddWithValue("@DataFormat", sensor.DataFormat ?? string.Empty);
                command.Parameters.AddWithValue("@CommunicationProtocol", sensor.CommunicationProtocol ?? string.Empty);
                command.Parameters.AddWithValue("@PowerSource", sensor.PowerSource ?? string.Empty);
                command.Parameters.AddWithValue("@OperatingTemperatureRange", sensor.OperatingTemperatureRange ?? string.Empty);
                command.Parameters.AddWithValue("@HumidityRange", sensor.HumidityRange ?? string.Empty);
                command.Parameters.AddWithValue("@PressureRange", sensor.PressureRange ?? string.Empty);
                command.Parameters.AddWithValue("@MeasurementRange", sensor.MeasurementRange ?? string.Empty);
                command.Parameters.AddWithValue("@MeasurementUnits", sensor.MeasurementUnits ?? string.Empty);
                command.Parameters.AddWithValue("@MeasurementAccuracy", sensor.MeasurementAccuracy ?? string.Empty);
                command.Parameters.AddWithValue("@MeasurementResolution", sensor.MeasurementResolution ?? string.Empty);
                command.Parameters.AddWithValue("@MeasurementInterval", sensor.MeasurementInterval ?? string.Empty);
                command.Parameters.AddWithValue("@DataStorageCapacity", sensor.DataStorageCapacity ?? string.Empty);
                command.Parameters.AddWithValue("@DataTransmissionInterval", sensor.DataTransmissionInterval ?? string.Empty);
                command.Parameters.AddWithValue("@DataTransmissionMethod", sensor.DataTransmissionMethod ?? string.Empty);
                command.Parameters.AddWithValue("@DataEncryption", sensor.DataEncryption ?? string.Empty);
                command.Parameters.AddWithValue("@DataCompression", sensor.DataCompression ?? string.Empty);
                command.Parameters.AddWithValue("@DataBackup", sensor.DataBackup ?? string.Empty);
                command.Parameters.AddWithValue("@DataRecovery", sensor.DataRecovery ?? string.Empty);
                command.Parameters.AddWithValue("@DataVisualization", sensor.DataVisualization ?? string.Empty);
                command.Parameters.AddWithValue("@DataAnalysis", sensor.DataAnalysis ?? string.Empty);
                command.Parameters.AddWithValue("@DataReporting", sensor.DataReporting ?? string.Empty);
                command.Parameters.AddWithValue("@DataSharing", sensor.DataSharing ?? string.Empty);
                command.Parameters.AddWithValue("@DataIntegration", sensor.DataIntegration ?? string.Empty);
                command.Parameters.AddWithValue("@DataStorageLocation", sensor.DataStorageLocation ?? string.Empty);
                command.Parameters.AddWithValue("@DataAccessControl", sensor.DataAccessControl ?? string.Empty);
                command.Parameters.AddWithValue("@DataRetentionPolicy", sensor.DataRetentionPolicy ?? string.Empty);
                command.Parameters.AddWithValue("@DataDisposalPolicy", sensor.DataDisposalPolicy ?? string.Empty);
                command.Parameters.AddWithValue("@DataSecurity", sensor.DataSecurity ?? string.Empty);
                command.Parameters.AddWithValue("@DataPrivacy", sensor.DataPrivacy ?? string.Empty);
                command.Parameters.AddWithValue("@DataCompliance", sensor.DataCompliance ?? string.Empty);
                command.Parameters.AddWithValue("@DataGovernance", sensor.DataGovernance ?? string.Empty);
                command.Parameters.AddWithValue("@DataQuality", sensor.DataQuality ?? string.Empty);
                command.Parameters.AddWithValue("@DataIntegrity", sensor.DataIntegrity ?? string.Empty);

                command.ExecuteNonQuery();
                Console.WriteLine("Sensor added successfully.");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error adding sensor: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
        /// <summary>
        /// Updates an existing sensor in the database.
        /// </summary>
        /// <param name="sensor">Sensor object with updated values.</param>
        public static void UpdateSensor(Sensor sensor)
        {
            try
            {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            Console.WriteLine("Database connection opened successfully.");

            string query = @"
            UPDATE Sensors SET
                Status = @Status,
                FirmwareVersion = @FirmwareVersion,
                SensorType = @SensorType,
                Location = @Location,
                Manufacturer = @Manufacturer,
                Model = @Model,
                SerialNumber = @SerialNumber,
                CalibrationDate = @CalibrationDate,
                LastMaintenanceDate = @LastMaintenanceDate,
                BatteryStatus = @BatteryStatus,
                SignalStrength = @SignalStrength,
                DataRate = @DataRate,
                DataFormat = @DataFormat,
                CommunicationProtocol = @CommunicationProtocol,
                PowerSource = @PowerSource,
                OperatingTemperatureRange = @OperatingTemperatureRange,
                HumidityRange = @HumidityRange,
                PressureRange = @PressureRange,
                MeasurementRange = @MeasurementRange,
                MeasurementUnits = @MeasurementUnits,
                MeasurementAccuracy = @MeasurementAccuracy,
                MeasurementResolution = @MeasurementResolution,
                MeasurementInterval = @MeasurementInterval,
                DataStorageCapacity = @DataStorageCapacity,
                DataTransmissionInterval = @DataTransmissionInterval,
                DataTransmissionMethod = @DataTransmissionMethod,
                DataEncryption = @DataEncryption,
                DataCompression = @DataCompression,
                DataBackup = @DataBackup,
                DataRecovery = @DataRecovery,
                DataVisualization = @DataVisualization,
                DataAnalysis = @DataAnalysis,
                DataReporting = @DataReporting,
                DataSharing = @DataSharing,
                DataIntegration = @DataIntegration,
                DataStorageLocation = @DataStorageLocation,
                DataAccessControl = @DataAccessControl,
                DataRetentionPolicy = @DataRetentionPolicy,
                DataDisposalPolicy = @DataDisposalPolicy,
                DataSecurity = @DataSecurity,
                DataPrivacy = @DataPrivacy,
                DataCompliance = @DataCompliance,
                DataGovernance = @DataGovernance,
                DataQuality = @DataQuality,
                DataIntegrity = @DataIntegrity
            WHERE SensorID = @SensorID;";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@SensorID", sensor.SensorID);
            command.Parameters.AddWithValue("@Status", sensor.Status ?? string.Empty);
            command.Parameters.AddWithValue("@FirmwareVersion", sensor.FirmwareVersion ?? string.Empty);
            command.Parameters.AddWithValue("@SensorType", sensor.SensorType ?? string.Empty);
            command.Parameters.AddWithValue("@Location", sensor.Location ?? string.Empty);
            command.Parameters.AddWithValue("@Manufacturer", sensor.Manufacturer ?? string.Empty);
            command.Parameters.AddWithValue("@Model", sensor.Model ?? string.Empty);
            command.Parameters.AddWithValue("@SerialNumber", sensor.SerialNumber ?? string.Empty);
            command.Parameters.AddWithValue("@CalibrationDate", sensor.CalibrationDate.HasValue ? sensor.CalibrationDate.Value : DBNull.Value);
            command.Parameters.AddWithValue("@LastMaintenanceDate", sensor.LastMaintenanceDate.HasValue ? sensor.LastMaintenanceDate.Value : DBNull.Value);
            command.Parameters.AddWithValue("@BatteryStatus", sensor.BatteryStatus ?? string.Empty);
            command.Parameters.AddWithValue("@SignalStrength", sensor.SignalStrength ?? string.Empty);
            command.Parameters.AddWithValue("@DataRate", sensor.DataRate ?? string.Empty);
            command.Parameters.AddWithValue("@DataFormat", sensor.DataFormat ?? string.Empty);
            command.Parameters.AddWithValue("@CommunicationProtocol", sensor.CommunicationProtocol ?? string.Empty);
            command.Parameters.AddWithValue("@PowerSource", sensor.PowerSource ?? string.Empty);
            command.Parameters.AddWithValue("@OperatingTemperatureRange", sensor.OperatingTemperatureRange ?? string.Empty);
            command.Parameters.AddWithValue("@HumidityRange", sensor.HumidityRange ?? string.Empty);
            command.Parameters.AddWithValue("@PressureRange", sensor.PressureRange ?? string.Empty);
            command.Parameters.AddWithValue("@MeasurementRange", sensor.MeasurementRange ?? string.Empty);
            command.Parameters.AddWithValue("@MeasurementUnits", sensor.MeasurementUnits ?? string.Empty);
            command.Parameters.AddWithValue("@MeasurementAccuracy", sensor.MeasurementAccuracy ?? string.Empty);
            command.Parameters.AddWithValue("@MeasurementResolution", sensor.MeasurementResolution ?? string.Empty);
            command.Parameters.AddWithValue("@MeasurementInterval", sensor.MeasurementInterval ?? string.Empty);
            command.Parameters.AddWithValue("@DataStorageCapacity", sensor.DataStorageCapacity ?? string.Empty);
            command.Parameters.AddWithValue("@DataTransmissionInterval", sensor.DataTransmissionInterval ?? string.Empty);
            command.Parameters.AddWithValue("@DataTransmissionMethod", sensor.DataTransmissionMethod ?? string.Empty);
            command.Parameters.AddWithValue("@DataEncryption", sensor.DataEncryption ?? string.Empty);
            command.Parameters.AddWithValue("@DataCompression", sensor.DataCompression ?? string.Empty);
            command.Parameters.AddWithValue("@DataBackup", sensor.DataBackup ?? string.Empty);
            command.Parameters.AddWithValue("@DataRecovery", sensor.DataRecovery ?? string.Empty);
            command.Parameters.AddWithValue("@DataVisualization", sensor.DataVisualization ?? string.Empty);
            command.Parameters.AddWithValue("@DataAnalysis", sensor.DataAnalysis ?? string.Empty);
            command.Parameters.AddWithValue("@DataReporting", sensor.DataReporting ?? string.Empty);
            command.Parameters.AddWithValue("@DataSharing", sensor.DataSharing ?? string.Empty);
            command.Parameters.AddWithValue("@DataIntegration", sensor.DataIntegration ?? string.Empty);
            command.Parameters.AddWithValue("@DataStorageLocation", sensor.DataStorageLocation ?? string.Empty);
            command.Parameters.AddWithValue("@DataAccessControl", sensor.DataAccessControl ?? string.Empty);
            command.Parameters.AddWithValue("@DataRetentionPolicy", sensor.DataRetentionPolicy ?? string.Empty);
            command.Parameters.AddWithValue("@DataDisposalPolicy", sensor.DataDisposalPolicy ?? string.Empty);
            command.Parameters.AddWithValue("@DataSecurity", sensor.DataSecurity ?? string.Empty);
            command.Parameters.AddWithValue("@DataPrivacy", sensor.DataPrivacy ?? string.Empty);
            command.Parameters.AddWithValue("@DataCompliance", sensor.DataCompliance ?? string.Empty);
            command.Parameters.AddWithValue("@DataGovernance", sensor.DataGovernance ?? string.Empty);
            command.Parameters.AddWithValue("@DataQuality", sensor.DataQuality ?? string.Empty);
            command.Parameters.AddWithValue("@DataIntegrity", sensor.DataIntegrity ?? string.Empty);

            command.ExecuteNonQuery();
            Console.WriteLine("Sensor updated successfully.");
            }
            catch (SqliteException ex)
            {
            Console.WriteLine($"Error updating sensor: {ex.Message}");
            }
            catch (Exception ex)
            {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a sensor from the database.
        /// </summary>
        /// <param name="sensorID">ID of the sensor to remove.</param>
        public static void RemoveSensor(int sensorID)
        {
            try
            {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            Console.WriteLine("Database connection opened successfully.");

            string query = "DELETE FROM Sensors WHERE SensorID = @SensorID;";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@SensorID", sensorID);

            command.ExecuteNonQuery();
            Console.WriteLine("Sensor removed successfully.");
            }
            catch (SqliteException ex)
            {
            Console.WriteLine($"Error removing sensor: {ex.Message}");
            }
            catch (Exception ex)
            {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    public class Sensor
    {
        public int SensorID { get; set; }
        public string? Status { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? SensorType { get; set; }
        public string? Location { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public string? BatteryStatus { get; set; }
        public string? SignalStrength { get; set; }
        public string? DataRate { get; set; }
        public string? DataFormat { get; set; }
        public string? CommunicationProtocol { get; set; }
        public string? PowerSource { get; set; }
        public string? OperatingTemperatureRange { get; set; }
        public string? HumidityRange { get; set; }
        public string? PressureRange { get; set; }
        public string? MeasurementRange { get; set; }
        public string? MeasurementUnits { get; set; }
        public string? MeasurementAccuracy { get; set; }
        public string? MeasurementResolution { get; set; }
        public string? MeasurementInterval { get; set; }
        public string? DataStorageCapacity { get; set; }
        public string? DataTransmissionInterval { get; set; }
        public string? DataTransmissionMethod { get; set; }
        public string? DataEncryption { get; set; }
        public string? DataCompression { get; set; }
        public string? DataBackup { get; set; }
        public string? DataRecovery { get; set; }
        public string? DataVisualization { get; set; }
        public string? DataAnalysis { get; set; }
        public string? DataReporting { get; set; }
        public string? DataSharing { get; set; }
        public string? DataIntegration { get; set; }
        public string? DataStorageLocation { get; set; }
        public string? DataAccessControl { get; set; }
        public string? DataRetentionPolicy { get; set; }
        public string? DataDisposalPolicy { get; set; }
        public string? DataSecurity { get; set; }
        public string? DataPrivacy { get; set; }
        public string? DataCompliance { get; set; }
        public string? DataGovernance { get; set; }
        public string? DataQuality { get; set; }
        public string? DataIntegrity { get; set; }
    }
}
