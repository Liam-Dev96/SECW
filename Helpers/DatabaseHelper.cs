using System.Data.SQLite;
using System.IO;
//using System.Data.SQLite is a namespace that provides classes for working with SQLite databases.
//using System.IO is a namespace that provides classes for working with files and directories.
//SQLite is a lightweight, serverless, self-contained SQL database engine.

namespace SECW.Helpers{
//namespace SECW.Helpers is a namespace that contains helper classes and methods for the SECW application.
//namespace SECW is the namespace for the SECW application.

public static class DataBaseHelper
{
    private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;Version=3;";
    //connection string to connect to the database
    //Data Source specifies the location of the database file.
    //Version specifies the version of the SQLite database engine to use.
    //Version 3 is the most commonly used version of SQLite.
    public static void initializeDatabase()
    //ensure that its referenced in the project atleast once to avoid errors and create the database.
    //this method is used to initialize the database
    //it checks if the database file exists, and if not, it creates the database file and the tables
    //and inserts the data into the tables
    //it also enables foreign key constraints
    {
        try
        //try and catch block to handle exceptions
        //SQLiteException is a class that represents an exception that occurs during SQLite database operations.
        {
            if (!File.Exists(@"Helpers\SoftwareEngineering.db"))
            {
                SQLiteConnection.CreateFile(@"Helpers\SoftwareEngineering.db");
                using var Connection = new SQLiteConnection(connectionString);//passing the connection string to the SQLiteConnection constructor
                {
                    Connection.Open();
                    using var pragmaCommand = new SQLiteCommand("PRAGMA foreign_keys = ON;", Connection);
                    //PRAGMA foreign_keys = ON; is a command used in SQLite to enable foreign key constraints.
                    //allows me to add foreign keys.
                    pragmaCommand.ExecuteNonQuery();
                    //executes the command to enable foreign key constraints.
                    //create all the tables for the database.
                    //will continue to use varchar for all the string data types due to previously faced issues with text formatting.
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

                    string CreateRolesTableQuery = @"Create Table If Not Exists Roles(
                    RoleID integer Primary Key AUTOINCREMENT,
                    RoleName VARCHAR (50)
                    );";

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

                    string CreateSensorTypesTableQuery = @"Create Table If Not Exists SensorTypes(
                    SensorTypeID integer Primary Key,
                    TypeName VARCHAR (20),
                    Description VARCHAR (255)
                    );";

                    string CreateLocationsTableQuery = @"Create Table If Not Exists Locations(
                    LocationID integer Primary Key,
                    Latitude Decimal(9,6),
                    Longitude Decimal(9,6),
                    Address VARCHAR (255),
                    Description VARCHAR (255)
                    );";

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

                    string CreateAlertThresholdsTableQuery = @"Create Table If Not Exists AlertThresholds(
                    ThresholdID integer Primary Key,
                    SensorTypeID Integer,
                    Parameter VARCHAR (50),
                    MinValue Float,
                    MaxValue Float,
                    Foreign Key (SensorTypeID) References SensorTypes(SensorTypeID)
                    );";

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

                    string CreateMaintenanceTableQuery = @"Create Table If Not Exists Maintenance(
                    MaintenanceID integer Primary Key,
                    SensorID Integer,
                    ScheduledDate DateTime,
                    CompletedDate DateTime,
                    Technician VARCHAR (100),
                    Notes VARCHAR (MAX),
                    Foreign Key (SensorID) References Sensors(SensorID)
                    );";

                    string CreateReportsTableQuery = @"Create Table If Not Exists Reports(
                    ReportID integer Primary Key,
                    UserID Integer,
                    ReportType VARCHAR (50),
                    StartDate DateTime,
                    EndDate DateTime,
                    GeneratedDate DateTime,
                    Parameters VARCHAR (MAX),
                    FilePath VARCHAR (255),
                    Foreign Key (UserID) References Users(UserID) ON DELETE CASCADE
                    );";

                    string CreateAuditLogsTableQuery = @"Create Table If Not Exists AuditLogs(
                    LogID integer Primary Key,
                    UserID Integer,
                    Action VARCHAR (50),
                    Timestamp DateTime,
                    Details VARCHAR (MAX),
                    Foreign Key (UserID) References Users(UserID) ON DELETE CASCADE
                    );";

                    //insert query to create the default admin account which will be used to login to the application and populate the databse with other roles.
                    //first up is creating the account for the admin user
                    string InsertAdminUserQuery = @"Insert Into Users (Username, PasswordHash, Email, RoleID, CreatedAt, LastLogin)
                    Values ('admin', 'Rxvstbvy@p72', 'admin112@gmail.com', 1, datetime('now'), NULL);";
                    //granting the user role of admin to help ease page navigation and access to the application later on
                    string InsertAdminRoleQuery = @"Insert Into Roles (RoleName)
                    Values ('Admin');";

                    using (var Command = new SQLiteCommand(Connection))
                    {
                        // Create tables first
                        Command.CommandText = CreateUsersTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateRolesTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateSensorsTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateSensorTypesTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateLocationsTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateEnvironmentalDataTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateAlertThresholdsTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateAlertsTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateMaintenanceTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateReportsTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateAuditLogsTableQuery;
                        Command.ExecuteNonQuery();
                    }

                    // Insert default data after tables are created
                    using (var insertCommand = new SQLiteCommand(InsertAdminRoleQuery, Connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }

                    using (var insertCommand = new SQLiteCommand(InsertAdminUserQuery, Connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
                    //table creation query final stage
                    //using the using statement to ensure that the connection is closed and disposed of properly
                    //using statement is a syntactic sugar for try-finally block
                    //it ensures that the connection is closed and disposed of properly, even if an exception occurs
                    //using statement is a good practice to follow when working with database connections

                Connection.Close();
                //closes the connection to the database
                //this is important to do after creating the database and tables
                //to free up resources and avoid locking the database file. 
            }


        }
        catch (SQLiteException ex)
        {
            Console.WriteLine($"SQLiteException: {ex.Message}");
            //logging the exception message to the console
            //this is important to do to understand what went wrong, and to debug the code
            //SQLiteException is a class that represents an exception that occurs during SQLite database operations.
        }
        // used a try and catch without a finally block...
        //finally block is used to execute code after the try and catch blocks, regardless of whether an exception was thrown or not.
        //in this case, I don't need to execute any code after the try and catch blocks, so I didn't use a finally block.

    }
}
}