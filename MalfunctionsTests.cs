using System;
using System.Collections.Generic;
using System.Windows.Controls; // Add this for ListBox
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;
using SECW;

namespace SECW_UnitTest;

public class MalfunctionsTests
{
    [Fact]
    public void MalfunctionsList_LoadsMalfunctionsFromDatabase()
    {
        // Arrange
        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();
        var mockReader = new Mock<SqliteDataReader>();

        mockReader.SetupSequence(r => r.Read())
                  .Returns(true) // Simulate one malfunction
                  .Returns(false); // End of data
        mockReader.Setup(r => r["SensorName"]).Returns("Sensor1");
        mockReader.Setup(r => r["Timestamp"]).Returns("2025-05-05 12:00:00");
        mockReader.Setup(r => r["Notes"]).Returns("Sensor malfunction detected");
        mockReader.Setup(r => r["MalfunctionsID"]).Returns("1");

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var malfunctionsPage = new Malfunctions();
        malfunctionsPage.SetConnection(mockConnection.Object);

        // Act
        malfunctionsPage.MalfunctionsList();

        // Assert
        Assert.NotNull(malfunctionsPage.MalfunctionsListbox.ItemsSource);
        var malfunctions = malfunctionsPage.MalfunctionsListbox.ItemsSource as List<Malfunctions.MalfunctionsItem>;
        Assert.Single(malfunctions);
        Assert.Equal("Sensor1 - 2025-05-05 12:00:00", malfunctions[0].DisplayText);
        Assert.Equal("Sensor malfunction detected", malfunctions[0].Notes);
    }

    [Fact]
    public async Task OnMalfunctionsSelected_ViewDetails_DisplaysDetails()
    {
        // Arrange
        var malfunctionItem = new Malfunctions.MalfunctionsItem
        {
            SensorName = "Sensor1",
            Timestamp = "2025-05-05 12:00:00",
            Notes = "Sensor malfunction detected",
            MalfunctionsID = "1"
        };

        var mockDisplayAlert = new Mock<Func<string, string, string, Task>>();
        var malfunctionsPage = new Malfunctions();
        malfunctionsPage.SetDisplayAlert(mockDisplayAlert.Object);

        // Act
        await malfunctionsPage.OnMalfunctionsSelected(null, new SelectedItemChangedEventArgs(malfunctionItem, 0));

        // Assert
        mockDisplayAlert.Verify(alert =>
            alert(
                "Malfunction Details - Sensor1",
                "Timestamp: 2025-05-05 12:00:00\nNotes: Sensor malfunction detected",
                "OK"),
            Times.Once);
    }

    [Fact]
    public async Task OnResolvedClicked_DeletesMalfunctionFromDatabase()
    {
        // Arrange
        var malfunctionItem = new Malfunctions.MalfunctionsItem
        {
            MalfunctionsID = "1"
        };

        var mockConnection = new Mock<SqliteConnection>();
        var mockCommand = new Mock<SqliteCommand>();

        mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1); // Simulate successful deletion
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var malfunctionsPage = new Malfunctions();
        malfunctionsPage.SetConnection(mockConnection.Object);

        // Act
        await malfunctionsPage.OnResolvedClicked(malfunctionItem);

        // Assert
        mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
    }
}

public class Malfunctions
{
    public ListBox MalfunctionsListbox { get; private set; }

    public void SetConnection(SqliteConnection connection)
    {
        // Implementation here
    }

    public void MalfunctionsList()
    {
        // Implementation here
    }

    public void SetDisplayAlert(Func<string, string, string, Task> displayAlert)
    {
        // Implementation here
    }

    public async Task OnResolvedClicked(MalfunctionsItem malfunctionItem)
    {
        // Implementation here
        await Task.CompletedTask; // Placeholder to ensure it is awaitable
    }

    public async Task OnMalfunctionsSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Implementation here
        await Task.CompletedTask; // Placeholder to ensure it is awaitable
    }

    public class MalfunctionsItem
    {
        public string SensorName { get; set; }
        public string Timestamp { get; set; }
        public string Notes { get; set; }
        public string MalfunctionsID { get; set; }

        public string DisplayText => $"{SensorName} - {Timestamp}";
    }
}