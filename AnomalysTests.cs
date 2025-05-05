using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;
using SECW;

namespace SECW_UnitTest;

public class AnomalysTests
{
    [Fact]
    public void AnomalysList_LoadsAnomaliesFromDatabase()
    {
        // Arrange
        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate one anomaly
                  .Returns(false); // End of data
        mockReader.Setup(r => r["SensorName"]).Returns("Sensor1");
        mockReader.Setup(r => r["Timestamp"]).Returns("2025-05-05 12:00:00");
        mockReader.Setup(r => r["SensorTypeName"]).Returns("Air Quality");
        mockReader.Setup(r => r["AnomalyReason"]).Returns("NO2 out of range");
        mockReader.Setup(r => r["AnomalyDetails"]).Returns("NO2: 50 (Range: 10-40)");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var anomalysPage = new Anomalys();
        anomalysPage.SetConnection(mockConnection.Object);

        // Act
        anomalysPage.AnomalysList();

        // Assert
        Assert.NotNull(anomalysPage.AnomalysListbox.ItemsSource);
        var anomalies = anomalysPage.AnomalysListbox.ItemsSource as List<Anomalys.AnomalyItem>;
        Assert.Single(anomalies);
        Assert.Equal("Sensor1 - 2025-05-05 12:00:00", anomalies[0].DisplayText);
        Assert.Equal("NO2 out of range", anomalies[0].AnomalyReason);
    }

    [Fact]
    public async Task OnAnomalySelected_DisplaysAnomalyDetails()
    {
        // Arrange
        var anomalyItem = new Anomalys.AnomalyItem
        {
            SensorName = "Sensor1",
            Timestamp = "2025-05-05 12:00:00",
            SensorType = "Air Quality",
            AnomalyReason = "NO2 out of range",
            AnomalyDetails = "NO2: 50 (Range: 10-40)"
        };

        var mockDisplayAlert = new Mock<Func<string, string, string, Task>>();
        var anomalysPage = new Anomalys();
        anomalysPage.SetDisplayAlert(mockDisplayAlert.Object);

        // Act
        await anomalysPage.OnAnomalySelected(new object(), new SelectedItemChangedEventArgs(anomalyItem, 0));

        // Assert
        mockDisplayAlert.Verify(alert =>
            alert(
                "Anomaly Details - Sensor1",
                "Sensor Type: Air Quality\nTimestamp: 2025-05-05 12:00:00\nReason: NO2 out of range\nDetails: NO2: 50 (Range: 10-40)",
                "OK"),
            Times.Once);
    }

    public void AnomalysList()
    {
        // Method implementation
    }
    
    public async Task OnAnomalySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Anomalys.AnomalyItem anomalyItem)
        {
            await DisplayAlert(
                $"Anomaly Details - {anomalyItem.SensorName}",
                $"Sensor Type: {anomalyItem.SensorType}\nTimestamp: {anomalyItem.Timestamp}\nReason: {anomalyItem.AnomalyReason}\nDetails: {anomalyItem.AnomalyDetails}",
                "OK");
        }
    }

    private Task DisplayAlert(string title, string message, string cancel)
    {
        // Simulate a DisplayAlert method for testing purposes
        return Task.CompletedTask;
    }

    // Change the access modifier to public
    public ListBox AnomalysListbox { get; set; }
}