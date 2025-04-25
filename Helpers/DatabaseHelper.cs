using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;

namespace SECW.Helpers
{
    public static class DataBaseHelper
    {
        // Connection string to the SQLite database file
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";

        // Method to initialize the database and create necessary tables
        public static void initializeDatabase()
        {
            try
            {
                // Check if the database file exists, if not, create it
                if (!File.Exists(@"Helpers\SoftwareEngineering.db"))
                {
                    // Ensure the database file is created by opening a connection
                    using (var tempConnection = new SqliteConnection(connectionString))
                    {
                        tempConnection.Open();
                        tempConnection.Close();
                    }

                    // Open a connection to the database
                    using var Connection = new SqliteConnection(connectionString);
                    {
                        Connection.Open();

                        // Enable foreign key constraints
                        using var pragmaCommand = new SqliteCommand("PRAGMA foreign_keys = ON;", Connection);
                        pragmaCommand.ExecuteNonQuery();

                        // Create the Users table
                        try
                        {
                            string CreateUsersTableQuery = @"Create Table If Not Exists Users(
                            UserID integer Primary Key AUTOINCREMENT,
                            Username VARCHAR (50) Unique,
                            PasswordHash VARCHAR (256) NOT NULL,
                            Email VARCHAR (100) NOT NULL,
                            RoleID Integer NOT NULL,
                            CreatedAt DateTime NOT NULL,
                            LastLogin DateTime,
                            Foreign Key (RoleID) References Roles(RoleID)
                            );";
                            using var Command = new SqliteCommand(CreateUsersTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Users table: {ex.Message}");
                        }

                        // Create the Roles table
                        try
                        {
                            string CreateRolesTableQuery = @"Create Table If Not Exists Roles(
                            RoleID integer Primary Key,
                            RoleName VARCHAR (50) Unique Check (RoleName IN ('Environmental Scientist', 'Operations Manager', 'Admin'))
                            );";
                            using var Command = new SqliteCommand(CreateRolesTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Roles table: {ex.Message}");
                        }

                        // Create the Sensors table
                        try
                        {
                            string CreateSensorsTableQuery = @"
CREATE TABLE IF NOT EXISTS Sensors (
    SensorID INTEGER PRIMARY KEY,
    Status VARCHAR(20),
    FirmwareVersion VARCHAR(20),
    SensorType VARCHAR(50),
    Location VARCHAR(255),
    Manufacturer VARCHAR(100),
    Model VARCHAR(100),
    SerialNumber VARCHAR(100),
    CalibrationDate DATETIME,
    LastMaintenanceDate DATETIME,
    BatteryStatus VARCHAR(50),
    SignalStrength VARCHAR(50),
    DataRate VARCHAR(50),
    DataFormat VARCHAR(50),
    CommunicationProtocol VARCHAR(50),
    PowerSource VARCHAR(50),
    OperatingTemperatureRange VARCHAR(50),
    HumidityRange VARCHAR(50),
    PressureRange VARCHAR(50),
    MeasurementRange VARCHAR(50),
    MeasurementUnits VARCHAR(50),
    MeasurementAccuracy VARCHAR(50),
    MeasurementResolution VARCHAR(50),
    MeasurementInterval VARCHAR(50),
    DataStorageCapacity VARCHAR(50),
    DataTransmissionInterval VARCHAR(50),
    DataTransmissionMethod VARCHAR(50),
    DataEncryption VARCHAR(50),
    DataCompression VARCHAR(50),
    DataBackup VARCHAR(50),
    DataRecovery VARCHAR(50),
    DataVisualization VARCHAR(50),
    DataAnalysis VARCHAR(50),
    DataReporting VARCHAR(50),
    DataSharing VARCHAR(50),
    DataIntegration VARCHAR(50),
    DataStorageLocation VARCHAR(50),
    DataAccessControl VARCHAR(50),
    DataRetentionPolicy VARCHAR(50),
    DataDisposalPolicy VARCHAR(50),
    DataSecurity VARCHAR(50),
    DataPrivacy VARCHAR(50),
    DataCompliance VARCHAR(50),
    DataGovernance VARCHAR(50),
    DataQuality VARCHAR(50),
    DataIntegrity VARCHAR(50)
);";
                            using var Command = new SqliteCommand(CreateSensorsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Sensors table: {ex.Message}");
                        }

                        // Create the SensorTypes table
                        try
                        {
                            string CreateSensorTypesTableQuery = @"Create Table If Not Exists SensorTypes(
                            SensorTypeID integer Primary Key,
                            TypeName VARCHAR (20),
                            Description VARCHAR (255)
                            );";
                            using var Command = new SqliteCommand(CreateSensorTypesTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating SensorTypes table: {ex.Message}");
                        }

                        // Create the Locations table
                        try
                        {
                            string CreateLocationsTableQuery = @"Create Table If Not Exists Locations(
                            LocationID integer Primary Key,
                            Latitude Decimal(9,6),
                            Longitude Decimal(9,6),
                            Address VARCHAR (255),
                            Description VARCHAR (255)
                            );";
                            using var Command = new SqliteCommand(CreateLocationsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Locations table: {ex.Message}");
                        }

                        // Create the EnvironmentalData table
                        try
                        {
                            string CreateEnvironmentalDataTableQuery = @"Create Table If Not Exists EnvironmentalData(
                            DataID integer Primary Key,
                            SensorID Integer,
                            Timestamp DateTime,
                            DataType VARCHAR (20),
                            PM25 Float,
                            PM10 Float,
                            CO2 Float,
                            SO2 Float,
                            NO2 Float,
                            pH Float,
                            DissolvedOxygen Float,
                            Turbidity Float,
                            WaterTemp Float,
                            AirTemp Float,
                            Humidity Float,
                            WindSpeed Float,
                            Precipitation Float,
                            Foreign Key (SensorID) References Sensors(SensorID)
                            );";
                            using var Command = new SqliteCommand(CreateEnvironmentalDataTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating EnvironmentalData table: {ex.Message}");
                        }

                        // Create the AlertThresholds table
                        try
                        {
                            string CreateAlertThresholdsTableQuery = @"Create Table If Not Exists AlertThresholds(
                            ThresholdID integer Primary Key,
                            SensorTypeID Integer,
                            Parameter VARCHAR (50),
                            MinValue Float,
                            MaxValue Float,
                            Foreign Key (SensorTypeID) References SensorTypes(SensorTypeID)
                            );";
                            using var Command = new SqliteCommand(CreateAlertThresholdsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating AlertThresholds table: {ex.Message}");
                        }

                        // Create the Alerts table
                        try
                        {
                            string CreateAlertsTableQuery = @"Create Table If Not Exists Alerts(
                            AlertID integer Primary Key,
                            SensorID Integer,
                            ThresholdID Integer,
                            BreachedValue Float,
                            Timestamp DateTime,
                            Status VARCHAR (20),
                            DataID Integer,
                            Foreign Key (SensorID) References Sensors(SensorID),
                            Foreign Key (ThresholdID) References AlertThresholds(ThresholdID),
                            Foreign Key (DataID) References EnvironmentalData(DataID)
                            );";
                            using var Command = new SqliteCommand(CreateAlertsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Alerts table: {ex.Message}");
                        }

                        // Create the Maintenance table
                        try
                        {
                            string CreateMaintenanceTableQuery = @"Create Table If Not Exists Maintenance(
                            MaintenanceID integer Primary Key,
                            SensorID Integer,
                            ScheduledDate DateTime,
                            CompletedDate DateTime,
                            Technician VARCHAR (100),
                            Notes VARCHAR (255),
                            Status VARCHAR (20),
                            Foreign Key (SensorID) References Sensors(SensorID)
                            );";
                            using var Command = new SqliteCommand(CreateMaintenanceTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Maintenance table: {ex.Message}");
                        }

                        // Create the Reports table
                        try
                        {
                            string CreateReportsTableQuery = @"Create Table If Not Exists Reports(
                            ReportID integer Primary Key,
                            UserID Integer,
                            ReportType VARCHAR (50),
                            StartDate DateTime,
                            EndDate DateTime,
                            GeneratedDate DateTime,
                            Parameters VARCHAR (255),
                            FilePath VARCHAR (255),
                            Foreign Key (UserID) References Users(UserID) ON DELETE CASCADE
                            );";
                            using var Command = new SqliteCommand(CreateReportsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating Reports table: {ex.Message}");
                        }

                        // Create the AuditLogs table
                        try
                        {
                            string CreateAuditLogsTableQuery = @"Create Table If Not Exists AuditLogs(
                            LogID integer Primary Key,
                            UserID Integer,
                            Action VARCHAR (50),
                            Timestamp DateTime,
                            Details VARCHAR (255),
                            Foreign Key (UserID) References Users(UserID) ON DELETE CASCADE
                            );";
                            using var Command = new SqliteCommand(CreateAuditLogsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating AuditLogs table: {ex.Message}");
                        }

                        // Insert default Admin role
                        string InsertAdminRoleQuery = @"Insert Into Roles (RoleName)
                        Values ('Admin');";
                        using (var insertCommand = new SqliteCommand(InsertAdminRoleQuery, Connection))
                        {
                            try
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                            catch (SqliteException ex)
                            {
                                // Ignore constraint violations (e.g., duplicate entries)
                                if (ex.ErrorCode != 19) // 19 is the SQLite error code for constraint violations
                                {
                                    throw;
                                }
                            }
                        }
        string InsertRolesQuery = @"
    INSERT OR IGNORE INTO Roles (RoleID, RoleName) VALUES (1, 'Admin');
    INSERT OR IGNORE INTO Roles (RoleID, RoleName) VALUES (2, 'Operations Manager');
    INSERT OR IGNORE INTO Roles (RoleID, RoleName) VALUES (3, 'Environmental Scientist');
";
            try
            {
                        using var insertRolesCommand = new SqliteCommand(InsertRolesQuery, Connection);
                        insertRolesCommand.ExecuteNonQuery();
                    }
                    catch (SqliteException ex)
                    {
                        Console.WriteLine($"Error inserting roles: {ex.Message}");
                    }

                        // Insert default Admin user
                        string InsertAdminUserQuery = @"Insert Into Users (Username, PasswordHash, Email, RoleID, CreatedAt, LastLogin)
                        Values ('admin', '$2b$12$9IRbqDiT5Vc0A8SModlwu.o/1pYCPfXEZYifdL94TgND/2FpfMBqy', 'admin112@gmail.com', 1, datetime('now'), NULL);";
                        using (var insertCommand = new SqliteCommand(InsertAdminUserQuery, Connection))
                        {
                            try
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                            catch (SqliteException ex)
                            {
                                // Ignore constraint violations (e.g., duplicate entries)
                                if (ex.ErrorCode != 19) // 19 is the SQLite error code for constraint violations
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    // Close the database connection
                    Connection.Close();
                }
            }
            catch (SqliteException ex)
            {
                // Log SQLite exceptions
                Console.WriteLine($"SqliteException: {ex.Message}");
            }
        }
    }
    }