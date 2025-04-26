using SECW.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SECW;

public partial class EnvironmentalScientistPage : ContentPage
{
    // ObservableCollection to hold the list of sensors
    public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();

    // Currently selected sensor
    public Sensor? SelectedSensor { get; set; }

    public EnvironmentalScientistPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Load sensors from the database on initialization
        LoadSensors();
    }

    /// <summary>
    /// Loads sensors from the database and populates the ObservableCollection.
    /// </summary>
    private void LoadSensors()
    {
        Sensors.Clear();
        Console.WriteLine("Loading sensors from the database...");
        var sensors = SensorsHelper.GetSensors();
        foreach (var sensor in sensors)
        {
            Sensors.Add(sensor);
        }
        Console.WriteLine($"Loaded {Sensors.Count} sensors.");
    }

    /// <summary>
    /// Handles the Add Sensor button click event.
    /// Validates input, checks for duplicates, and adds a new sensor.
    /// </summary>
    private void OnAddSensorClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Add Sensor button clicked.");

        if (!ValidateSensorInput(out int sensorID)) return;

        // Check if a sensor with the same ID already exists
        var existingSensor = Sensors.FirstOrDefault(s => s.SensorID == sensorID);
        if (existingSensor != null)
        {
            Console.WriteLine($"Sensor with ID {sensorID} already exists.");
            DisplayAlert("Error", $"A sensor with ID {sensorID} already exists.", "OK");
            return;
        }

        // Create a new sensor object
        var newSensor = new Sensor
        {
            SensorID = sensorID,
            Status = StatusEntry.Text,
            FirmwareVersion = FirmwareVersionEntry.Text,
            SensorType = SensorTypeEntry.Text,
            Location = LocationEntry.Text,
            Manufacturer = ManufacturerEntry.Text,
            Model = ModelEntry.Text,
            SerialNumber = SerialNumberEntry.Text,
            CalibrationDate = DateTime.TryParse(CalibrationDateEntry.Text, out var calibrationDate) ? calibrationDate : null,
            LastMaintenanceDate = DateTime.TryParse(LastMaintenanceDateEntry.Text, out var maintenanceDate) ? maintenanceDate : null,
            BatteryStatus = BatteryStatusEntry.Text,
            SignalStrength = SignalStrengthEntry.Text,
            DataRate = DataRateEntry.Text,
            DataFormat = DataFormatEntry.Text,
            CommunicationProtocol = CommunicationProtocolEntry.Text,
            PowerSource = PowerSourceEntry.Text,
            OperatingTemperatureRange = OperatingTemperatureRangeEntry.Text,
            HumidityRange = HumidityRangeEntry.Text,
            PressureRange = PressureRangeEntry.Text,
            MeasurementRange = MeasurementRangeEntry.Text,
            MeasurementUnits = MeasurementUnitsEntry.Text,
            MeasurementAccuracy = MeasurementAccuracyEntry.Text,
            MeasurementResolution = MeasurementResolutionEntry.Text,
            MeasurementInterval = MeasurementIntervalEntry.Text,
            DataStorageCapacity = DataStorageCapacityEntry.Text,
            DataTransmissionInterval = DataTransmissionIntervalEntry.Text,
            DataTransmissionMethod = DataTransmissionMethodEntry.Text,
            DataEncryption = DataEncryptionEntry.Text,
            DataCompression = DataCompressionEntry.Text,
            DataBackup = DataBackupEntry.Text,
            DataRecovery = DataRecoveryEntry.Text,
            DataVisualization = DataVisualizationEntry.Text,
            DataAnalysis = DataAnalysisEntry.Text,
            DataReporting = DataReportingEntry.Text,
            DataSharing = DataSharingEntry.Text,
            DataIntegration = DataIntegrationEntry.Text,
            DataStorageLocation = DataStorageLocationEntry.Text,
            DataAccessControl = DataAccessControlEntry.Text,
            DataRetentionPolicy = DataRetentionPolicyEntry.Text,
            DataDisposalPolicy = DataDisposalPolicyEntry.Text,
            DataSecurity = DataSecurityEntry.Text,
            DataPrivacy = DataPrivacyEntry.Text,
            DataCompliance = DataComplianceEntry.Text,
            DataGovernance = DataGovernanceEntry.Text,
            DataQuality = DataQualityEntry.Text,
            DataIntegrity = DataIntegrityEntry.Text
        };

        // Add the new sensor to the database
        SensorsHelper.AddSensor(newSensor);
        Console.WriteLine($"Sensor with ID {sensorID} added successfully.");

        // Refresh the sensor list
        LoadSensors();

        // Clear input fields
        ClearInputFields();

        // Notify the user
        DisplayAlert("Success", "Sensor added successfully.", "OK");
    }

    /// <summary>
    /// Handles the Remove Sensor button click event.
    /// Removes the selected sensor from the database.
    /// </summary>
    private void OnRemoveSensorClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Remove Sensor button clicked.");

        if (SelectedSensor == null)
        {
            Console.WriteLine("No sensor selected for removal.");
            DisplayAlert("Error", "No sensor selected for removal.", "OK");
            return;
        }

        // Remove the sensor from the database
        SensorsHelper.RemoveSensor(SelectedSensor.SensorID);
        Console.WriteLine($"Sensor with ID {SelectedSensor.SensorID} removed successfully.");

        // Refresh the sensor list
        LoadSensors();

        // Clear input fields
        ClearInputFields();

        // Notify the user
        DisplayAlert("Success", "Sensor removed successfully.", "OK");
    }

    /// <summary>
    /// Handles the Save Configuration button click event.
    /// Updates the selected sensor's configuration in the database.
    /// </summary>
    private void OnSaveConfigurationClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Save Configuration button clicked.");

        if (SelectedSensor == null)
        {
            Console.WriteLine("No sensor selected for saving configuration.");
            DisplayAlert("Error", "No sensor selected for saving configuration.", "OK");
            return;
        }

        if (!ValidateSensorInput(out _)) return;

        // Update the selected sensor's properties
        SelectedSensor.Status = StatusEntry.Text;
        SelectedSensor.FirmwareVersion = FirmwareVersionEntry.Text;
        SelectedSensor.SensorType = SensorTypeEntry.Text;
        SelectedSensor.Location = LocationEntry.Text;
        SelectedSensor.Manufacturer = ManufacturerEntry.Text;
        SelectedSensor.Model = ModelEntry.Text;
        SelectedSensor.SerialNumber = SerialNumberEntry.Text;
        SelectedSensor.CalibrationDate = DateTime.TryParse(CalibrationDateEntry.Text, out var calibrationDate) ? calibrationDate : null;
        SelectedSensor.LastMaintenanceDate = DateTime.TryParse(LastMaintenanceDateEntry.Text, out var maintenanceDate) ? maintenanceDate : null;
        SelectedSensor.BatteryStatus = BatteryStatusEntry.Text;
        SelectedSensor.SignalStrength = SignalStrengthEntry.Text;
        SelectedSensor.DataRate = DataRateEntry.Text;
        SelectedSensor.DataFormat = DataFormatEntry.Text;
        SelectedSensor.CommunicationProtocol = CommunicationProtocolEntry.Text;
        SelectedSensor.PowerSource = PowerSourceEntry.Text;
        SelectedSensor.OperatingTemperatureRange = OperatingTemperatureRangeEntry.Text;
        SelectedSensor.HumidityRange = HumidityRangeEntry.Text;
        SelectedSensor.PressureRange = PressureRangeEntry.Text;
        SelectedSensor.MeasurementRange = MeasurementRangeEntry.Text;
        SelectedSensor.MeasurementUnits = MeasurementUnitsEntry.Text;
        SelectedSensor.MeasurementAccuracy = MeasurementAccuracyEntry.Text;
        SelectedSensor.MeasurementResolution = MeasurementResolutionEntry.Text;
        SelectedSensor.MeasurementInterval = MeasurementIntervalEntry.Text;
        SelectedSensor.DataStorageCapacity = DataStorageCapacityEntry.Text;
        SelectedSensor.DataTransmissionInterval = DataTransmissionIntervalEntry.Text;
        SelectedSensor.DataTransmissionMethod = DataTransmissionMethodEntry.Text;
        SelectedSensor.DataEncryption = DataEncryptionEntry.Text;
        SelectedSensor.DataCompression = DataCompressionEntry.Text;
        SelectedSensor.DataBackup = DataBackupEntry.Text;
        SelectedSensor.DataRecovery = DataRecoveryEntry.Text;
        SelectedSensor.DataVisualization = DataVisualizationEntry.Text;
        SelectedSensor.DataAnalysis = DataAnalysisEntry.Text;
        SelectedSensor.DataReporting = DataReportingEntry.Text;
        SelectedSensor.DataSharing = DataSharingEntry.Text;
        SelectedSensor.DataIntegration = DataIntegrationEntry.Text;
        SelectedSensor.DataStorageLocation = DataStorageLocationEntry.Text;
        SelectedSensor.DataAccessControl = DataAccessControlEntry.Text;
        SelectedSensor.DataRetentionPolicy = DataRetentionPolicyEntry.Text;
        SelectedSensor.DataDisposalPolicy = DataDisposalPolicyEntry.Text;
        SelectedSensor.DataSecurity = DataSecurityEntry.Text;
        SelectedSensor.DataPrivacy = DataPrivacyEntry.Text;
        SelectedSensor.DataCompliance = DataComplianceEntry.Text;
        SelectedSensor.DataGovernance = DataGovernanceEntry.Text;
        SelectedSensor.DataQuality = DataQualityEntry.Text;
        SelectedSensor.DataIntegrity = DataIntegrityEntry.Text;

        // Save the updated sensor to the database
        SensorsHelper.UpdateSensor(SelectedSensor);
        Console.WriteLine($"Configuration for sensor with ID {SelectedSensor.SensorID} saved successfully.");

        // Refresh the sensor list
        LoadSensors();
        // Clear input fields
        ClearInputFields();
        // Clear the selected sensor
        // Notify the user
        DisplayAlert("Success", "Configuration saved successfully.", "OK");
    }

    /// <summary>
    /// Handles the selection of a sensor from the list.
    /// Populates the input fields with the selected sensor's details.
    /// </summary>
    private void OnSensorSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
        {
            Console.WriteLine("No sensor selected.");
            ClearInputFields();
            return;
        }

        if (e.SelectedItem is Sensor selectedSensor)
        {
            Console.WriteLine($"Sensor with ID {selectedSensor.SensorID} selected.");
            SelectedSensor = selectedSensor;

            // Populate input fields with the selected sensor's details
            SensorIDEntry.Text = selectedSensor.SensorID.ToString();
            StatusEntry.Text = selectedSensor.Status;
            FirmwareVersionEntry.Text = selectedSensor.FirmwareVersion;
            SensorTypeEntry.Text = selectedSensor.SensorType;
            LocationEntry.Text = selectedSensor.Location;
            ManufacturerEntry.Text = selectedSensor.Manufacturer;
            ModelEntry.Text = selectedSensor.Model;
            SerialNumberEntry.Text = selectedSensor.SerialNumber;
            CalibrationDateEntry.Text = selectedSensor.CalibrationDate?.ToString("yyyy-MM-dd");
            LastMaintenanceDateEntry.Text = selectedSensor.LastMaintenanceDate?.ToString("yyyy-MM-dd");
            BatteryStatusEntry.Text = selectedSensor.BatteryStatus;
            SignalStrengthEntry.Text = selectedSensor.SignalStrength;
            DataRateEntry.Text = selectedSensor.DataRate;
            DataFormatEntry.Text = selectedSensor.DataFormat;
            CommunicationProtocolEntry.Text = selectedSensor.CommunicationProtocol;
            PowerSourceEntry.Text = selectedSensor.PowerSource;
            OperatingTemperatureRangeEntry.Text = selectedSensor.OperatingTemperatureRange;
            HumidityRangeEntry.Text = selectedSensor.HumidityRange;
            PressureRangeEntry.Text = selectedSensor.PressureRange;
            MeasurementRangeEntry.Text = selectedSensor.MeasurementRange;
            MeasurementUnitsEntry.Text = selectedSensor.MeasurementUnits;
            MeasurementAccuracyEntry.Text = selectedSensor.MeasurementAccuracy;
            MeasurementResolutionEntry.Text = selectedSensor.MeasurementResolution;
            MeasurementIntervalEntry.Text = selectedSensor.MeasurementInterval;
            DataStorageCapacityEntry.Text = selectedSensor.DataStorageCapacity;
            DataTransmissionIntervalEntry.Text = selectedSensor.DataTransmissionInterval;
            DataTransmissionMethodEntry.Text = selectedSensor.DataTransmissionMethod;
            DataEncryptionEntry.Text = selectedSensor.DataEncryption;
            DataCompressionEntry.Text = selectedSensor.DataCompression;
            DataBackupEntry.Text = selectedSensor.DataBackup;
            DataRecoveryEntry.Text = selectedSensor.DataRecovery;
            DataVisualizationEntry.Text = selectedSensor.DataVisualization;
            DataAnalysisEntry.Text = selectedSensor.DataAnalysis;
            DataReportingEntry.Text = selectedSensor.DataReporting;
            DataSharingEntry.Text = selectedSensor.DataSharing;
            DataIntegrationEntry.Text = selectedSensor.DataIntegration;
            DataStorageLocationEntry.Text = selectedSensor.DataStorageLocation;
            DataAccessControlEntry.Text = selectedSensor.DataAccessControl;
            DataRetentionPolicyEntry.Text = selectedSensor.DataRetentionPolicy;
            DataDisposalPolicyEntry.Text = selectedSensor.DataDisposalPolicy;
            DataSecurityEntry.Text = selectedSensor.DataSecurity;
            DataPrivacyEntry.Text = selectedSensor.DataPrivacy;
            DataComplianceEntry.Text = selectedSensor.DataCompliance;
            DataGovernanceEntry.Text = selectedSensor.DataGovernance;
            DataQualityEntry.Text = selectedSensor.DataQuality;
            DataIntegrityEntry.Text = selectedSensor.DataIntegrity;
        }
    }

    /// <summary>
    /// Clears all input fields and resets the selected sensor.
    /// </summary>
    private void ClearInputFields()
    {
        Console.WriteLine("Clearing input fields.");
        SelectedSensor = null;
        SensorIDEntry.Text = string.Empty;
        StatusEntry.Text = string.Empty;
        FirmwareVersionEntry.Text = string.Empty;
        SensorTypeEntry.Text = string.Empty;
        LocationEntry.Text = string.Empty;
        ManufacturerEntry.Text = string.Empty;
        ModelEntry.Text = string.Empty;
        SerialNumberEntry.Text = string.Empty;
        CalibrationDateEntry.Text = string.Empty;
        LastMaintenanceDateEntry.Text = string.Empty;
        BatteryStatusEntry.Text = string.Empty;
        SignalStrengthEntry.Text = string.Empty;
        DataRateEntry.Text = string.Empty;
        DataFormatEntry.Text = string.Empty;
        CommunicationProtocolEntry.Text = string.Empty;
        PowerSourceEntry.Text = string.Empty;
        OperatingTemperatureRangeEntry.Text = string.Empty;
        HumidityRangeEntry.Text = string.Empty;
        PressureRangeEntry.Text = string.Empty;
        MeasurementRangeEntry.Text = string.Empty;
        MeasurementUnitsEntry.Text = string.Empty;
        MeasurementAccuracyEntry.Text = string.Empty;
        MeasurementResolutionEntry.Text = string.Empty;
        MeasurementIntervalEntry.Text = string.Empty;
        DataStorageCapacityEntry.Text = string.Empty;
        DataTransmissionIntervalEntry.Text = string.Empty;
        DataTransmissionMethodEntry.Text = string.Empty;
        DataEncryptionEntry.Text = string.Empty;
        DataCompressionEntry.Text = string.Empty;
        DataBackupEntry.Text = string.Empty;
        DataRecoveryEntry.Text = string.Empty;
        DataVisualizationEntry.Text = string.Empty;
        DataAnalysisEntry.Text = string.Empty;
        DataReportingEntry.Text = string.Empty;
        DataSharingEntry.Text = string.Empty;
        DataIntegrationEntry.Text = string.Empty;
        DataStorageLocationEntry.Text = string.Empty;
        DataAccessControlEntry.Text = string.Empty;
        DataRetentionPolicyEntry.Text = string.Empty;
        DataDisposalPolicyEntry.Text = string.Empty;
        DataSecurityEntry.Text = string.Empty;
        DataPrivacyEntry.Text = string.Empty;
        DataComplianceEntry.Text = string.Empty;
        DataGovernanceEntry.Text = string.Empty;
        DataQualityEntry.Text = string.Empty;
        DataIntegrityEntry.Text = string.Empty;
    }

    /// <summary>
    /// Validates the sensor input fields.
    /// Ensures the Sensor ID is valid and not already in use.
    /// </summary>
    /// <returns>True if validation passes, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when the Sensor ID is invalid.</exception>
    /// allows for white space and empty values to be checked for in the sensor ID entry field other fields are not checked for empty values as they are not required to be filled in at this point in time.</exception>
    private bool ValidateSensorInput(out int sensorID)
    {
        sensorID = 0;

        // Validate Sensor ID
        if (string.IsNullOrWhiteSpace(SensorIDEntry.Text) || !int.TryParse(SensorIDEntry.Text, out sensorID))
        {
            Console.WriteLine("Invalid Sensor ID: Not a number or empty.");
            DisplayAlert("Error", "Please enter a valid Sensor ID (must be a number).", "OK");
            return false;
        }

        if (sensorID <= 0)
        {
            Console.WriteLine("Invalid Sensor ID: Must be a positive integer.");
            DisplayAlert("Error", "Sensor ID must be a positive integer.", "OK");
            return false;
        }

        Console.WriteLine($"Sensor input validation passed for Sensor ID {sensorID}.");
        return true;
    }
}
