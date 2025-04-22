using System.Data.SQLite;
using System.IO;

namespace SECW.Helpers
{
    public static class DataBaseHelper
    {
        // Connection string to the SQLite database file
        private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";

        // Method to initialize the database and create necessary tables
        public static void initializeDatabase()
        {
            try
            {
                // Check if the database file exists, if not, create it
                if (!File.Exists(@"Helpers\SoftwareEngineering.db"))
                {
                    SQLiteConnection.CreateFile(@"Helpers\SoftwareEngineering.db");

                    // Open a connection to the database
                    using var Connection = new SQLiteConnection(connectionString);
                    {
                        Connection.Open();

                        // Enable foreign key constraints
                        using var pragmaCommand = new SQLiteCommand("PRAGMA foreign_keys = ON;", Connection);
                        pragmaCommand.ExecuteNonQuery();

                        // Create the Users table
                        try
                        {
                            string CreateUsersTableQuery = @"Create Table If Not Exists Users(
                            UserID integer Primary Key AUTOINCREMENT,
                            Username VARCHAR (50) Unique,
                            PasswordHash VARCHAR (256),
                            Email VARCHAR (100),
                            RoleID Integer,
                            CreatedAt DateTime,
                            LastLogin DateTime,
                            Foreign Key (RoleID) References Roles(RoleID)
                            );";
                            using var Command = new SQLiteCommand(CreateUsersTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine($"Error creating Users table: {ex.Message}");
                        }

                        // Create the Roles table
                        try
                        {
                            string CreateRolesTableQuery = @"Create Table If Not Exists Roles(
                            RoleID integer Primary Key AUTOINCREMENT,
                            RoleName VARCHAR (50) Unique Check (RoleName IN ('Environmental Scientist', 'Operations Manager', 'Admin'))
                            );";
                            using var Command = new SQLiteCommand(CreateRolesTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine($"Error creating Roles table: {ex.Message}");
                        }

                        // Create the Sensors table
                        try
                        {
                            string CreateSensorsTableQuery = @"Create Table If Not Exists Sensors(
                            SensorID integer Primary Key,
                            SensorTypeID Integer,
                            LocationID Integer,
                            Status VARCHAR (20),
                            InstallationDate DateTime,
                            FirmwareVersion VARCHAR (20),
                            Foreign Key (SensorTypeID) References SensorTypes(SensorTypeID),
                            Foreign Key (LocationID) References Locations(LocationID)
                            );";
                            using var Command = new SQLiteCommand(CreateSensorsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateSensorTypesTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateLocationsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateEnvironmentalDataTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateAlertThresholdsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateAlertsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateMaintenanceTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateReportsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
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
                            using var Command = new SQLiteCommand(CreateAuditLogsTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine($"Error creating AuditLogs table: {ex.Message}");
                        }

                        // Insert default Admin role
                        string InsertAdminRoleQuery = @"Insert Into Roles (RoleName)
                        Values ('Admin');";
                        using (var insertCommand = new SQLiteCommand(InsertAdminRoleQuery, Connection))
                        {
                            try
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                // Ignore constraint violations (e.g., duplicate entries)
                                if (ex.ErrorCode != (int)SQLiteErrorCode.Constraint)
                                {
                                    throw;
                                }
                            }
                        }

                        // Insert default Admin user
                        string InsertAdminUserQuery = @"Insert Into Users (Username, PasswordHash, Email, RoleID, CreatedAt, LastLogin)
                        Values ('admin', '$2b$12$9IRbqDiT5Vc0A8SModlwu.o/1pYCPfXEZYifdL94TgND/2FpfMBqy', 'admin112@gmail.com', 1, datetime('now'), NULL);";
                        using (var insertCommand = new SQLiteCommand(InsertAdminUserQuery, Connection))
                        {
                            try
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                // Ignore constraint violations (e.g., duplicate entries)
                                if (ex.ErrorCode != (int)SQLiteErrorCode.Constraint)
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
            catch (SQLiteException ex)
            {
                // Log SQLite exceptions
                Console.WriteLine($"SQLiteException: {ex.Message}");
            }
        }
    }
}