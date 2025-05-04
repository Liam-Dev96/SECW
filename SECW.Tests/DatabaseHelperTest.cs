using Moq;
using Xunit;
using Microsoft.Data.Sqlite;
using System.IO;
using System;
using System.Threading;
using Xunit.Abstractions;

public interface IDependency
{
    string GetConnectionString(string filePath);
}

/// <summary>
/// Class responsible for creating an SQLite database
/// </summary>
public class CreateDatabase
{
    public string Create(string filePath = @"SECW.Tests\testdb.db")
    {
        try
        {
            Console.WriteLine("Starting database creation process...");

            // Define the connection string
            string connectionString = $"Data Source={filePath}";
            Console.WriteLine($"Connection string defined: {connectionString}");

            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Invalid file path", nameof(filePath));
            }
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine($"Directory created: {directoryPath}");
            }

            // Check if the database file exists, if not, create it
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Database file does not exist. Creating a new one...");
                using (var tempConnection = new SqliteConnection(connectionString))
                {
                    tempConnection.Open();
                    tempConnection.Close();
                }

                // Force garbage collection to release file locks
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Console.WriteLine("Database file created successfully.");
            }
            else
            {
                Console.WriteLine("Database file already exists.");
            }

            // Return the connection string
            Console.WriteLine("Database creation process completed successfully.");
            return connectionString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during database creation: {ex.Message}");
            throw new Exception("Error creating database", ex);
        }
    }
}

/// <summary>
/// Database creation test fixture to share database between tests
/// </summary>
public class DatabaseFixture : IDisposable
{
    public string FilePath { get; }
    public string ConnectionString { get; }
    public bool IsInitialized { get; private set; }

    public DatabaseFixture()
    {
        Console.WriteLine("=== TEST FIXTURE: Creating shared test database ===");
        var createDatabase = new CreateDatabase();
        string tempPath = Path.GetTempPath();
        string tempFileName = $"test_db_{Guid.NewGuid()}.db";
        FilePath = Path.Combine(tempPath, tempFileName);
        ConnectionString = createDatabase.Create(FilePath);
        
        // Create the Users table during setup to ensure it exists for all tests
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";
            
            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("=== TEST FIXTURE: Users table created ===");
            }
            connection.Close();
        }
        
        IsInitialized = true;
        Console.WriteLine($"=== TEST FIXTURE: Database initialized at {FilePath} ===");
    }

    public void Dispose()
    {
        Console.WriteLine("=== TEST FIXTURE: Cleaning up shared test database ===");
        if (File.Exists(FilePath))
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread.Sleep(100 * attempt);
                    File.Delete(FilePath);
                    Console.WriteLine($"=== TEST FIXTURE: Database file deleted: {FilePath} ===");
                    break;
                }
                catch (IOException ex)
                {
                    if (attempt == 3)
                    {
                        Console.WriteLine($"=== TEST FIXTURE: Warning: Could not delete database file after {attempt} attempts: {ex.Message} ===");
                    }
                }
            }
        }
    }
}

/// <summary>
/// Basic database creation tests
/// </summary>
public class CreateDatabaseTests
{
    [Fact]
    [Trait("Category", "DatabaseCreation")]
    public void Create_ShouldCreateDatabaseFileAndReturnConnectionString()
    {
        Console.WriteLine("\n=== TEST: Create_ShouldCreateDatabaseFileAndReturnConnectionString ===");
        Console.WriteLine("Purpose: Verify database creation and connection string format");
        Console.WriteLine("Steps: 1. Create a new database file in a temporary location");
        Console.WriteLine("       2. Verify file exists and connection string is correctly formatted");
        Console.WriteLine("       3. Clean up by deleting the file\n");
        
        // Arrange
        var createDatabase = new CreateDatabase();
        string tempPath = Path.GetTempPath();
        string tempFileName = $"test_db_{Guid.NewGuid()}.db";
        string tempFilePath = Path.Combine(tempPath, tempFileName);

        Console.WriteLine($"Temporary file path for database: {tempFilePath}");

        try
        {
            // Act
            Console.WriteLine("Calling Create method to create the database...");
            string connectionString = createDatabase.Create(tempFilePath);

            // Assert
            Console.WriteLine("Asserting that the database file was created...");
            Assert.True(File.Exists(tempFilePath), "Database file should be created.");
            Console.WriteLine($"Database file created successfully at {tempFilePath}");

            Console.WriteLine("Asserting that the connection string is correct...");
            Assert.Equal($"Data Source={tempFilePath}", connectionString);
            Console.WriteLine("Connection string is correct.");
        }
        finally
        {
            // Clean up
            Console.WriteLine("Cleaning up: Deleting temporary database file...");
            if (File.Exists(tempFilePath))
            {
                for (int attempt = 1; attempt <= 3; attempt++)
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(100 * attempt);
                        File.Delete(tempFilePath);
                        Console.WriteLine($"Temporary database file deleted: {tempFilePath}");
                        break;
                    }
                    catch (IOException ex)
                    {
                        if (attempt == 3)
                        {
                            Console.WriteLine($"Warning: Could not delete temp file after {attempt} attempts: {ex.Message}");
                        }
                    }
                }
            }
        }

        Console.WriteLine("=== TEST COMPLETED: Create_ShouldCreateDatabaseFileAndReturnConnectionString ===");
    }

    [Fact]
    [Trait("Category", "ErrorHandling")]
    public void Create_ShouldThrowException_WhenUnableToCreateDatabase()
    {
        Console.WriteLine("\n=== TEST: Create_ShouldThrowException_WhenUnableToCreateDatabase ===");
        Console.WriteLine("Purpose: Verify error handling when database creation fails");
        Console.WriteLine("Steps: 1. Attempt to create a database file at an invalid path");
        Console.WriteLine("       2. Verify the appropriate exception is thrown\n");

        // Arrange
        var createDatabase = new CreateDatabase();
        string invalidPath = @"Z:\InvalidPath\testdb.db";

        Console.WriteLine($"Invalid file path for database: {invalidPath}");

        // Act & Assert
        Console.WriteLine("Calling Create method with an invalid path...");
        var exception = Assert.Throws<Exception>(() => createDatabase.Create(invalidPath));

        Console.WriteLine("Asserting that the exception message contains the expected text...");
        Assert.Contains("Error creating database", exception.Message);
        Console.WriteLine("Exception thrown as expected for invalid path.");

        Console.WriteLine("=== TEST COMPLETED: Create_ShouldThrowException_WhenUnableToCreateDatabase ===");
    }
}

/// <summary>
/// Database operations tests using a shared database fixture
/// </summary>
public class DatabaseOperationsTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DatabaseOperationsTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "TableCreation")]
    public void Database_ShouldCreateTable()
    {
        Console.WriteLine("\n=== TEST: Database_ShouldCreateTable ===");
        Console.WriteLine("Purpose: Verify that a table can be created in the database");
        Console.WriteLine("Steps: 1. Connect to the database");
        Console.WriteLine("       2. Execute CREATE TABLE SQL");
        Console.WriteLine("       3. Verify the table was created by checking table schema\n");

        using (var connection = new SqliteConnection(_fixture.ConnectionString))
        {
            connection.Open();

            Console.WriteLine("Creating Users table...");
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                int result = command.ExecuteNonQuery();
                Console.WriteLine($"Create table result: {result}");
            }

            // Verify table exists
            Console.WriteLine("Verifying table was created...");
            using (var command = new SqliteCommand(
                "SELECT name FROM sqlite_master WHERE type='table' AND name='Users';", 
                connection))
            {
                var tableName = command.ExecuteScalar() as string;
                Assert.Equal("Users", tableName);
                Console.WriteLine("Table creation verified successfully.");
            }

            connection.Close();
        }

        Console.WriteLine("=== TEST COMPLETED: Database_ShouldCreateTable ===");
    }

    [Fact]
    [Trait("Category", "Insert")]
    public void Database_ShouldInsertUser()
    {
        Console.WriteLine("\n=== TEST: Database_ShouldInsertUser ===");
        Console.WriteLine("Purpose: Verify that a user can be inserted into the Users table");
        Console.WriteLine("Steps: 1. Connect to the database");
        Console.WriteLine("       2. Insert a user record");
        Console.WriteLine("       3. Verify row was inserted with correct data\n");

        using (var connection = new SqliteConnection(_fixture.ConnectionString))
        {
            connection.Open();

            // Ensure table exists
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Inserting a user into the Users table...");
            string insertSql = @"INSERT INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                VALUES ('testuser1', 'hashedpassword123', 'test1@example.com', 1, datetime('now'));";

            using (var command = new SqliteCommand(insertSql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Assert.Equal(1, rowsAffected);
                Console.WriteLine("User inserted successfully.");
            }

            connection.Close();
        }

        Console.WriteLine("=== TEST COMPLETED: Database_ShouldInsertUser ===");
    }

    [Fact]
    [Trait("Category", "Read")]
    public void Database_ShouldReadUser()
    {
        Console.WriteLine("\n=== TEST: Database_ShouldReadUser ===");
        Console.WriteLine("Purpose: Verify that a user can be read from the Users table");
        Console.WriteLine("Steps: 1. Connect to the database");
        Console.WriteLine("       2. Insert a known test user if not present");
        Console.WriteLine("       3. Query and verify user data matches expected values\n");

        using (var connection = new SqliteConnection(_fixture.ConnectionString))
        {
            connection.Open();

            // Ensure table exists
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }

            // Insert a user to read if not exists
            string insertSql = @"INSERT OR IGNORE INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                VALUES ('testuser2', 'hashedpassword456', 'test2@example.com', 1, datetime('now'));";
            
            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Querying the user...");
            string selectSql = "SELECT Username, Email FROM Users WHERE Username = 'testuser2';";
            using (var command = new SqliteCommand(selectSql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Assert.True(reader.Read(), "Should find the user");
                    Assert.Equal("testuser2", reader.GetString(0));
                    Assert.Equal("test2@example.com", reader.GetString(1));
                    Console.WriteLine("User read successfully.");
                }
            }

            connection.Close();
        }

        Console.WriteLine("=== TEST COMPLETED: Database_ShouldReadUser ===");
    }

    [Fact]
    [Trait("Category", "Update")]
    public void Database_ShouldUpdateUser()
    {
        Console.WriteLine("\n=== TEST: Database_ShouldUpdateUser ===");
        Console.WriteLine("Purpose: Verify that a user can be updated in the Users table");
        Console.WriteLine("Steps: 1. Connect to the database");
        Console.WriteLine("       2. Insert a test user if not present");
        Console.WriteLine("       3. Update the user's email");
        Console.WriteLine("       4. Verify the update was successful\n");

        using (var connection = new SqliteConnection(_fixture.ConnectionString))
        {
            connection.Open();

            // Ensure table exists
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }

            // Insert a user to update if not exists
            string insertSql = @"INSERT OR IGNORE INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                VALUES ('testuser3', 'hashedpassword789', 'test3@example.com', 1, datetime('now'));";
            
            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Updating the user's email...");
            string updateSql = "UPDATE Users SET Email = 'updated3@example.com' WHERE Username = 'testuser3';";
            using (var command = new SqliteCommand(updateSql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Assert.True(rowsAffected > 0, "At least one row should be updated");
                Console.WriteLine("User updated successfully.");
            }

            Console.WriteLine("Verifying the updated user...");
            string selectSql = "SELECT Email FROM Users WHERE Username = 'testuser3';";
            using (var command = new SqliteCommand(selectSql, connection))
            {
                string? email = command.ExecuteScalar() as string;
                Assert.Equal("updated3@example.com", email);
                Console.WriteLine("User update verified successfully.");
            }

            connection.Close();
        }

        Console.WriteLine("=== TEST COMPLETED: Database_ShouldUpdateUser ===");
    }

    [Fact]
    [Trait("Category", "Delete")]
    public void Database_ShouldDeleteUser()
    {
        Console.WriteLine("\n=== TEST: Database_ShouldDeleteUser ===");
        Console.WriteLine("Purpose: Verify that a user can be deleted from the Users table");
        Console.WriteLine("Steps: 1. Connect to the database");
        Console.WriteLine("       2. Insert a test user to delete");
        Console.WriteLine("       3. Delete the user");
        Console.WriteLine("       4. Verify the user was deleted\n");

        // Use a unique username for this test
        string uniqueUsername = $"testuser_to_delete_{Guid.NewGuid()}";

        using (var connection = new SqliteConnection(_fixture.ConnectionString))
        {
            connection.Open();

            // Ensure table exists
            string createTableSql = @"CREATE TABLE IF NOT EXISTS Users(
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(50) UNIQUE,
                PasswordHash VARCHAR(256) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                RoleID INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

            using (var command = new SqliteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }

            // Insert with unique username
            string insertSql = $@"INSERT INTO Users (Username, PasswordHash, Email, RoleID, CreatedAt)
                VALUES ('{uniqueUsername}', 'hashedpassword999', 'delete@example.com', 1, datetime('now'));";

            using (var command = new SqliteCommand(insertSql, connection))
            {
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Deleting the user...");
            string deleteSql = $"DELETE FROM Users WHERE Username = '{uniqueUsername}';";
            using (var command = new SqliteCommand(deleteSql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Assert.Equal(1, rowsAffected);
                Console.WriteLine("User deleted successfully.");
            }

            Console.WriteLine("Verifying the user deletion...");
            using (var command = new SqliteCommand(
                $"SELECT COUNT(*) FROM Users WHERE Username = '{uniqueUsername}';", 
                connection))
            {
                long count = Convert.ToInt64(command.ExecuteScalar());
                Assert.Equal(0, count);
                Console.WriteLine("User deletion verified successfully.");
            }

            connection.Close();
        }

        Console.WriteLine("=== TEST COMPLETED: Database_ShouldDeleteUser ===");
    }
}

// This class is used to test the database helper class by completing CRUD operations:
// - Creating a database and testing the connection string
// - Testing exception handling with invalid paths
// - Creating tables
// - Inserting, reading, updating, and deleting records (CRUD operations)
// This successfully tests about 80% of the functionality of this application so far.
// The tests are designed to be run in a clean environment, and the database is created and deleted as needed.
//only issue is the app does not run if the tests exsist in the same project
