using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;
using SECW; // Add this to reference the UpdateSenser class



public class UpdateSenserTests
{
    [Fact]
    public async Task UpdateSensorCon_ValidInput_UpdatesDatabase()
    {
        // Arrange
        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate data exists
                  .Returns(false);
        mockReader.Setup(r => r["Data1Min"]).Returns("10");
        mockReader.Setup(r => r["Data1Max"]).Returns("20");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var updateSenser = new UpdateSenser();
        updateSenser.SetConnection(mockConnection.Object);

        // Act
        await updateSenser.UpdateSensorCon(1, "Data1Min", "Data1Max");

        // Assert
        mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
    }

    [Fact]
    public async Task UpdateSensorFramework_InvalidInput_ShowsError()
    {
        // Arrange
        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate data exists
                  .Returns(false);
        mockReader.Setup(r => r["SensorTypeFirmware"]).Returns("1.0");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var updateSenser = new UpdateSenser();
        updateSenser.SetConnection(mockConnection.Object);

        // Act
        await updateSenser.UpdateSensorFramework(1);

        // Assert
        mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Never);
    }
}

public class UpdateSenser
{
    private SqliteConnection _connection;

    public void SetConnection(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task UpdateSensorCon(int sensorId, string minField, string maxField)
    {
        // Method implementation
    }

        public async Task UpdateSensorFramework(int sensorId)
    {
        // Method implementation
    }
}