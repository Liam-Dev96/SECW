using SECW.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SECW;

public partial class EnvironmentalScientistPage : ContentPage
{
    public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();
    public Sensor? SelectedSensor { get; set; }

    public EnvironmentalScientistPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Load sensors from the database
        LoadSensors();
    }

    private void LoadSensors()
    {
        Sensors.Clear();
        var sensors = SensorsHelper.GetSensors();
        foreach (var sensor in sensors)
        {
            Sensors.Add(sensor);
        }
    }

    private void OnAddSensorClicked(object sender, EventArgs e)
    {
        if (!ValidateSensorInput(out int sensorID)) return;
       // Check if a sensor with the same ID already exists
        var existingSensor = Sensors.FirstOrDefault(s => s.SensorID == sensorID);
    if (existingSensor != null)
    {
        DisplayAlert("Error", $"A sensor with ID {sensorID} already exists.", "OK");
        return;
    }
        // Logic to add a new sensor
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

        SensorsHelper.AddSensor(newSensor);
        LoadSensors();
        ClearInputFields();
        DisplayAlert("Success", "Sensor added successfully.", "OK");
    }


    private void OnRemoveSensorClicked(object sender, EventArgs e)
    {
        if (SelectedSensor == null)
        {
            DisplayAlert("Error", "No sensor selected for removal.", "OK");
            return;
        }

        SensorsHelper.RemoveSensor(SelectedSensor.SensorID);
        LoadSensors();
        ClearInputFields();
        DisplayAlert("Success", "Sensor removed successfully.", "OK");
    }

    private void OnSaveConfigurationClicked(object sender, EventArgs e)
    {
        if (SelectedSensor == null)
        {
            DisplayAlert("Error", "No sensor selected for saving configuration.", "OK");
            return;
        }

        if (!ValidateSensorInput(out _)) return;

        // Update the selected sensor
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

        SensorsHelper.UpdateSensor(SelectedSensor);
        LoadSensors();
        DisplayAlert("Success", "Configuration saved successfully.", "OK");
    }

    private void OnSensorSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
        {
            ClearInputFields();
            return;
        }

        if (e.SelectedItem is Sensor selectedSensor)
        {
            // Fetch the full sensor details from the database
            var sensorDetails = SensorsHelper.PopulateSensorFields(e, SensorIDEntry, selectedSensor.SensorID);
            if (sensorDetails != null)
            {
                SelectedSensor = sensorDetails;
                SensorIDEntry.Text = sensorDetails.SensorID.ToString();
                StatusEntry.Text = sensorDetails.Status;
                FirmwareVersionEntry.Text = sensorDetails.FirmwareVersion;
                SensorTypeEntry.Text = sensorDetails.SensorType;
                LocationEntry.Text = sensorDetails.Location;
                ManufacturerEntry.Text = sensorDetails.Manufacturer;
                ModelEntry.Text = sensorDetails.Model;
                SerialNumberEntry.Text = sensorDetails.SerialNumber;
                CalibrationDateEntry.Text = sensorDetails.CalibrationDate?.ToString("yyyy-MM-dd");
                LastMaintenanceDateEntry.Text = sensorDetails.LastMaintenanceDate?.ToString("yyyy-MM-dd");
                BatteryStatusEntry.Text = sensorDetails.BatteryStatus;
                SignalStrengthEntry.Text = sensorDetails.SignalStrength;
                DataRateEntry.Text = sensorDetails.DataRate;
                DataFormatEntry.Text = sensorDetails.DataFormat;
                CommunicationProtocolEntry.Text = sensorDetails.CommunicationProtocol;
                PowerSourceEntry.Text = sensorDetails.PowerSource;
                OperatingTemperatureRangeEntry.Text = sensorDetails.OperatingTemperatureRange;
                HumidityRangeEntry.Text = sensorDetails.HumidityRange;
                PressureRangeEntry.Text = sensorDetails.PressureRange;
                MeasurementRangeEntry.Text = sensorDetails.MeasurementRange;
                MeasurementUnitsEntry.Text = sensorDetails.MeasurementUnits;
                MeasurementAccuracyEntry.Text = sensorDetails.MeasurementAccuracy;
                MeasurementResolutionEntry.Text = sensorDetails.MeasurementResolution;
                MeasurementIntervalEntry.Text = sensorDetails.MeasurementInterval;
                DataStorageCapacityEntry.Text = sensorDetails.DataStorageCapacity;
                DataTransmissionIntervalEntry.Text = sensorDetails.DataTransmissionInterval;
                DataTransmissionMethodEntry.Text = sensorDetails.DataTransmissionMethod;
                DataEncryptionEntry.Text = sensorDetails.DataEncryption;
                DataCompressionEntry.Text = sensorDetails.DataCompression;
                DataBackupEntry.Text = sensorDetails.DataBackup;
                DataRecoveryEntry.Text = sensorDetails.DataRecovery;
                DataVisualizationEntry.Text = sensorDetails.DataVisualization;
                DataAnalysisEntry.Text = sensorDetails.DataAnalysis;
                DataReportingEntry.Text = sensorDetails.DataReporting;
                DataSharingEntry.Text = sensorDetails.DataSharing;
                DataIntegrationEntry.Text = sensorDetails.DataIntegration;
                DataStorageLocationEntry.Text = sensorDetails.DataStorageLocation;
                DataAccessControlEntry.Text = sensorDetails.DataAccessControl;
                DataRetentionPolicyEntry.Text = sensorDetails.DataRetentionPolicy;
                DataDisposalPolicyEntry.Text = sensorDetails.DataDisposalPolicy;
                DataSecurityEntry.Text = sensorDetails.DataSecurity;
                DataPrivacyEntry.Text = sensorDetails.DataPrivacy;
                DataComplianceEntry.Text = sensorDetails.DataCompliance;
                DataGovernanceEntry.Text = sensorDetails.DataGovernance;
                DataQualityEntry.Text = sensorDetails.DataQuality;
                DataIntegrityEntry.Text = sensorDetails.DataIntegrity;
            }
            else
            {
                ClearInputFields();
            }
        }
    }

    private void ClearInputFields()
    {
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

    private bool ValidateSensorInput(out int sensorID)
    {
        // Validate Sensor ID
        // Check if the Sensor ID is empty or not a number
        // Check if the Sensor ID is a positive integer
        // Check if the Sensor ID is already in use
        // Check if the Sensor ID is a valid number
        // allows for whitespace and empty strings for all fields except sensorID.
        sensorID = 0;

        if (string.IsNullOrWhiteSpace(SensorIDEntry.Text) || !int.TryParse(SensorIDEntry.Text, out sensorID))
        {
            DisplayAlert("Error", "Please enter a valid Sensor ID (must be a number).", "OK");
            return false;
        }

        if (sensorID <= 0)
        {
            DisplayAlert("Error", "Sensor ID must be a positive integer.", "OK");
            return false;
        }

        // Log successful validation
        Console.WriteLine("Sensor input validation passed successfully.");
        return true;
    }
}


