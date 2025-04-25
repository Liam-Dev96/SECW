using SECW.Helpers;
using System.Collections.ObjectModel;

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

        // Logic to add a new sensor
        var newSensor = new Sensor
        {
            SensorID = sensorID,
            Status = StatusEntry.Text,
            FirmwareVersion = FirmwareVersionEntry.Text
        };

        SensorsHelper.AddSensor(newSensor);
        LoadSensors();
        ClearInputFields();
        DisplayAlert("Success", "Sensor added successfully.", "OK");
    }

    private void OnEditSensorClicked(object sender, EventArgs e)
    {
        if (SelectedSensor == null)
        {
            DisplayAlert("Error", "No sensor selected for editing.", "OK");
            return;
        }

        if (!ValidateSensorInput(out int sensorID)) return;

        // Update the selected sensor
        SelectedSensor.SensorID = sensorID;
        SelectedSensor.Status = StatusEntry.Text;
        SelectedSensor.FirmwareVersion = FirmwareVersionEntry.Text;

        SensorsHelper.UpdateSensor(SelectedSensor);
        LoadSensors();
        ClearInputFields();
        DisplayAlert("Success", "Sensor updated successfully.", "OK");
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

        SensorsHelper.UpdateSensor(SelectedSensor);
        LoadSensors();
        DisplayAlert("Success", "Configuration saved successfully.", "OK");
    }

    private void OnSensorSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Sensor selectedSensor)
        {
            SelectedSensor = selectedSensor;
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
        else
        {
            ClearInputFields();
        }
    }

    private void ClearInputFields()
    {
        SelectedSensor = null;
        SensorIDEntry.Text = string.Empty;
        StatusEntry.Text = string.Empty;
        FirmwareVersionEntry.Text = string.Empty;
    }

    private bool ValidateSensorInput(out int sensorID)
    {
        sensorID = 0;

        if (string.IsNullOrWhiteSpace(SensorIDEntry.Text) || !int.TryParse(SensorIDEntry.Text, out sensorID))
        {
            DisplayAlert("Error", "Please enter a valid Sensor ID.", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(StatusEntry.Text) || string.IsNullOrWhiteSpace(FirmwareVersionEntry.Text))
        {
            DisplayAlert("Error", "Please fill in all fields.", "OK");
            return false;
        }

        return true;
    }
}


