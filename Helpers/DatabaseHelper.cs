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

                        // Create the SensorType table
                        try
                        {
                            const string createSensorTypeTableQuery = @"
                            CREATE TABLE IF NOT EXISTS SensorType(
                                SensorTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
                                SensorTypeFirmware REAL,
                                SensorTypeName TEXT NOT NULL UNIQUE 
                                    CHECK (SensorTypeName IN ('Air Quality', 'Water Quality', 'Weather')),
                                Data1Min REAL,
                                Data1Max REAL,
                                Data2Min REAL,
                                Data2Max REAL,
                                Data3Min REAL,
                                Data3Max REAL,
                                Data4Min REAL,
                                Data4Max REAL
                            );";

                            using var command = new SqliteCommand(createSensorTypeTableQuery, Connection);
                            command.ExecuteNonQuery();
                            
                            Console.WriteLine("SensorType table created/verified successfully");
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating SensorType table (SQLite Error {ex.ErrorCode}): {ex.Message}");
                            // Consider rethrowing if this is a critical error for your application
                            throw;
                        }

                        string InsertSensorTypeQuery = @"
                        INSERT OR IGNORE INTO SensorType (SensorTypeID, SensorTypeFirmware, SensorTypeName, Data1Min, Data1Max, Data2Min, Data2Max, Data3Min, Data3Max, Data4Min, Data4Max) VALUES (1, 10.9, 'Air Quality', 1.0, 70.0, 0.5, 5.0, 0.5, 10.0, 0.1, 15.0);
                        INSERT OR IGNORE INTO SensorType (SensorTypeID, SensorTypeFirmware, SensorTypeName, Data1Min, Data1Max, Data2Min, Data2Max, Data3Min, Data3Max, Data4Min, Data4Max) VALUES (2, 4.8, 'Water Quality', 20.0, 30.0, 1.0, 2.0, 0.01, 0.2, Null, Null);
                        INSERT OR IGNORE INTO SensorType (SensorTypeID, SensorTypeFirmware, SensorTypeName , Data1Min, Data1Max, Data2Min, Data2Max, Data3Min, Data3Max, Data4Min, Data4Max) VALUES (3, 11.0, 'Weather', -5.0, 15.0, 75.0, 95.0, -0.1, 4.0, Null, Null);
                        ";
                try
                {
                        using var insertSensorTypeCommand = new SqliteCommand(InsertSensorTypeQuery, Connection);
                        insertSensorTypeCommand.ExecuteNonQuery();
                    }
                    catch (SqliteException ex)
                    {
                        Console.WriteLine($"Error inserting roles: {ex.Message}");
                    }

                        
                        // Create the sensor table
                        try
                        {
                            string CreateSensorTableQuery = @"Create Table If Not Exists Sensor(
                            SensorID integer Primary Key,
                            SensorTypeID integer,
                            SensorName VARCHAR(255),
                            Latitude Decimal(9,6),
                            Longitude Decimal(9,6),
                            Address VARCHAR (255),
                            Foreign Key (SensorTypeID) References SensorType(SensorTypeID)
                            );";
                            using var Command = new SqliteCommand(CreateSensorTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating sensor table: {ex.Message}");
                        }

                        string InsertSensorQuery = @"
                                    INSERT OR IGNORE INTO Sensor (SensorID, SensorTypeID, SensorName, Latitude, Longitude, Address) VALUES (1, 1, 'Air Sensor 1', 55.94476, -3.183991, 'Edinburgh Nicolson Street');
                                    INSERT OR IGNORE INTO Sensor (SensorID, SensorTypeID, SensorName, Latitude, Longitude, Address) VALUES (2, 2, 'Water Sensor 1', 55.94476, -3.183991, 'Edinburgh Nicolson Street');
                                    INSERT OR IGNORE INTO Sensor (SensorID, SensorTypeID, SensorName, Latitude, Longitude, Address) VALUES (3, 3, 'Weather Sensor 1', 55.94476, -3.183991, 'Edinburgh Nicolson Street');
                                    INSERT OR IGNORE INTO Sensor (SensorID, SensorTypeID, SensorName, Latitude, Longitude, Address) VALUES (4, 1, 'Air Sensor 2', 79.87535, -15.81265, 'Edinburgh Morningside Road');
                                    ";
                        try
                        {
                            using var insertSensorCommand = new SqliteCommand(InsertSensorQuery, Connection);
                            insertSensorCommand.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error inserting roles: {ex.Message}");
                        }


                        // Create the EnvironmentalData table
                        try
                        {
                            string CreateEnvironmentalDataTableQuery = @"Create Table If Not Exists EnvironmentalData(
                            DataID integer Primary Key,
                            SensorID Integer,
                            Timestamp DateTime,
                            NO2 Float,
                            SO2 Float,
                            PM25 Float,
                            PM10 Float,
                            Nitrate Float,
                            Nitrite Float,
                            Phosphate Float,
                            Temp Float,
                            Humidity Float,
                            WindSpeed Float,
                            WindDirection Float,
                            Foreign Key (SensorID) References Sensor(SensorID)
                            );";
                            using var Command = new SqliteCommand(CreateEnvironmentalDataTableQuery, Connection);
                            Command.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error creating EnvironmentalData table: {ex.Message}");
                        }

                        string InsertEnvironmentalDataQuery = @"
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (1, 1, '2025-03-25 07:00:00', 23.1, 2.3, 5.1, 9.1);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (2, 1, '2025-03-25 08:00:00', 42.5, 5.4, 2.7, 11.9);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (3, 1, '2025-03-25 09:00:00', 33.9, 4.3, 5.6, 10.5);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (4, 1, '2025-03-25 10:00:00', 63.2, 4.2, 5.7, 9.3);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Nitrate, Nitrite, Phosphate) VALUES (5, 2, '2025-03-25 07:00:00', 23.1, 1.3, 0.02);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Nitrate, Nitrite, Phosphate) VALUES (6, 2, '2025-03-25 08:00:00', 29.9, 1.4, 0.03);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Nitrate, Nitrite, Phosphate) VALUES (7, 2, '2025-03-25 09:00:00', 25.0, 1.3, 0.06);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Nitrate, Nitrite, Phosphate) VALUES (8, 2, '2025-03-25 10:00:00', 24.3, 1.6, 0.04);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Temp, Humidity, WindSpeed, WindDirection) VALUES (9, 3, '2025-03-25 07:00:00', 3.1, 82.3, 3.1, 53);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Temp, Humidity, WindSpeed, WindDirection) VALUES (10, 3, '2025-03-25 08:00:00', 7.4, 84.5, 1.6, 183);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Temp, Humidity, WindSpeed, WindDirection) VALUES (11, 3, '2025-03-25 09:00:00', 7.3, 83.3, 2.3, 187);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, Temp, Humidity, WindSpeed, WindDirection) VALUES (12, 3, '2025-03-25 10:00:00', 10.0, 82.7, 2.4, 273);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (13, 4, '2025-03-25 07:00:00', 77.7, 2.1, 6.4, 5.6);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (14, 4, '2025-03-25 08:00:00', 25.6, 2.1, 7.3, 6.1);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (15, 4, '2025-03-25 09:00:00', 55.2, 3.4, 6.5, 7.2);
                                    INSERT OR IGNORE INTO EnvironmentalData (DataID, SensorID, Timestamp, NO2, SO2, PM25, PM10) VALUES (16, 4, '2025-03-25 10:00:00', 48.8, 3.9, 6.9, 5.9);
                                    ";
                        try
                        {
                            using var insertEnvironmentalDataCommand = new SqliteCommand(InsertEnvironmentalDataQuery, Connection);
                            insertEnvironmentalDataCommand.ExecuteNonQuery();
                        }
                        catch (SqliteException ex)
                        {
                            Console.WriteLine($"Error inserting roles: {ex.Message}");
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