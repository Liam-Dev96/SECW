using System.Data.SQLite;
using System.IO;
//using System.Data.SQLite is a namespace that provides classes for working with SQLite databases.
//using System.IO is a namespace that provides classes for working with files and directories.
//SQLite is a lightweight, serverless, self-contained SQL database engine.



public static class DataBaseHelper
{
    private static string connectionString = @"Data Source=..\..\Files\SoftwareEngineering.db;Version=3;";
    //connection string to connect to the database
    //Data Source specifies the location of the database file.
    //Version specifies the version of the SQLite database engine to use.
    //Version 3 is the most commonly used version of SQLite.
    public static void initializeDatabase()
    //this method is used to initialize the database
    //it checks if the database file exists, and if not, it creates the database file and the tables
    //and inserts the data into the tables
    //it also enables foreign key constraints
    {
        try
        //try and catch block to handle exceptions
        //SQLiteException is a class that represents an exception that occurs during SQLite database operations.
        {
            if (!File.Exists(@"..\..\Files\SoftwareEngineering.db"))
            {
                SQLiteConnection.CreateFile(@"..\..\Files\SoftwareEngineering.db");
                using var Connection = new SQLiteConnection(connectionString);//passing the connection string to the SQLiteConnection constructor
                {
                    Connection.Open();
                    using var pragmaCommand = new SQLiteCommand("PRAGMA foreign_keys = ON;", Connection);
                    //PRAGMA foreign_keys = ON; is a command used in SQLite to enable foreign key constraints.
                    //allows me to add foreign keys.
                    pragmaCommand.ExecuteNonQuery();
                    //executes the command to enable foreign key constraints.


                    //create all the tables for the database.
                   //example of a table creation query.
                   //will continue to use varchar for all the string data types due to previously faced issues with text formatting.
                    string CreateCoachesTableQuery = @"Create Table If Not Exists Coaches(
                    Coaches_ID integer Primary Key,
                    Coaches_FirstName VARCHAR (150),
                    Coaches_LastName VARCHAR (150),
                    Coaches_Email VARCHAR (225),
                    Coaches_Address VARCHAR (225),
                    Coaches_Number Integer,
                    DOB Date,
                    Comments VARCHAR (500)
                    );";

                    //example of inset query
                    string insertIntoStaff = @"Insert Into Staff (Staff_ID, Staff_FirstName, Staff_LastName, Staff_Email, Staff_Address, Staff_Number, DOB) Values(10011589, 'John', 'Smith', 'john.smith@example.com', '123 Main St', 1234567890, '1985-03-15');";

                    // example of a table creation query
                    using (var Command = new SQLiteCommand(Connection))
                    {
                        Command.CommandText = CreateCoachesTableQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = insertIntoStaff;
                        Command.ExecuteNonQuery();
                    };
                }

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