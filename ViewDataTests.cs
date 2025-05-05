using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;
using SECW;

namespace SECW_UnitTest;

public class ViewDataTests
{
    [Fact]
    public async Task LoadSensorPicker_LoadsSensorsFromDatabase()
    {
        // Arrange
        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate one sensor
                  .Returns(false); // End of data
        mockReader.Setup(r => r["SensorID"]).Returns(1);
        mockReader.Setup(r => r["SensorTypeID"]).Returns(1);
        mockReader.Setup(r => r["SensorName"]).Returns("Sensor1");

        mockCommand.Setup(c => c.ExecuteReaderAsync()).ReturnsAsync(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var viewDataPage = new ViewData();
        viewDataPage.SetConnection(mockConnection.Object);

        // Act
        await viewDataPage.LoadSensorPicker();

        // Assert
        Assert.NotNull(viewDataPage.SensorPicker.ItemsSource);
        var sensors = viewDataPage.SensorPicker.ItemsSource as List<SensorItem>;
        Assert.Single(sensors);
        Assert.Equal(1, sensors[0].SensorID);
        Assert.Equal(1, sensors[0].SensorTypeID);
        Assert.Equal("Sensor1", sensors[0].SensorName);
    }

    [Fact]
    public async Task SensorPicker_SelectedIndexChanged_LoadsSensorData()
    {
        // Arrange
        var selectedSensor = new SensorItem
        {
            SensorID = 1,
            SensorTypeID = 1,
            SensorName = "Sensor1"
        };

        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate one data record
                  .Returns(false); // End of data
        mockReader.Setup(r => r["Timestamp"]).Returns("2025-05-05 12:00:00");
        mockReader.Setup(r => r["NO2"]).Returns(50);
        mockReader.Setup(r => r["SO2"]).Returns(20);

        mockCommand.Setup(c => c.ExecuteReaderAsync()).ReturnsAsync(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var viewDataPage = new ViewData();
        viewDataPage.SetConnection(mockConnection.Object);

        // Act
        await viewDataPage.SensorPicker_SelectedIndexChanged(new object(), new EventArgs());

        // Assert
        Assert.NotNull(viewDataPage.DataCollectionView.ItemsSource);
        var dataItems = viewDataPage.DataCollectionView.ItemsSource as List<SensorDataItem>;
        Assert.Single(dataItems);
        Assert.Contains("Timestamp: 2025-05-05 12:00:00", dataItems[0].DisplayText);
        Assert.Contains("NO2: 50", dataItems[0].DisplayText);
        Assert.Contains("SO2: 20", dataItems[0].DisplayText);
    }

    [Fact]
    public void FormatDisplayText_FormatsCorrectly()
    {
        // Arrange
        var values = new Dictionary<string, object>
        {
            { "Timestamp", "2025-05-05 12:00:00" },
            { "NO2", 50 },
            { "SO2", null }
        };

        var viewDataPage = new ViewData();

        // Act
        var result = viewDataPage.FormatDisplayText(values, 1);

        // Assert
        Assert.Contains("2025-05-05 12:00:00", result);
        Assert.Contains("NO2: 50", result);
        Assert.Contains("SO2: N/A", result);
    }
}

// Ensure the access modifier is public for accessibility
public Picker SensorPicker { get; set; }
public CollectionView DataCollectionView { get; set; }

private async Task SensorPicker_SelectedIndexChanged(object sender, EventArgs e)
{
    // Example implementation with await to avoid running synchronously
    await Task.Delay(1); // Replace with actual asynchronous logic
}

public string FormatDisplayText(Dictionary<string, object> values, int sensorTypeID)
{
    // Example implementation to ensure all code paths return a value
    if (values == null || values.Count == 0)
    {
        return "No data available";
    }

    var displayText = new List<string>();
    foreach (var kvp in values)
    {
        displayText.Add($"{kvp.Key}: {(kvp.Value ?? "N/A")}");
    }

    return string.Join(", ", displayText);
}
}