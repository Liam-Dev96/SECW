using System;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using SECW.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SECW;

public partial class UpdateSenser : ContentPage
{
	private static string connectionString = @"Data Source=Helpers\SoftwareEngineering.db;";
	public UpdateSenser()
	{
		InitializeComponent();
		InitializeDatabase();

	}

	private void InitializeDatabase()
    {
        try
        {
            DataBaseHelper.initializeDatabase();
            Console.WriteLine("[INFO] Database initialized successfully.");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to initialize the database: {ex.Message}", "OK");
            Console.WriteLine($"[ERROR] Database initialization failed: {ex.Message}");
        }
    }

	public async void UpdateSensorCon(int id, string Min, string Max)
	{

		try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("[INFO] Database connection opened successfully.");

                // Set busy timeout to prevent "database is locked" errors
                using (var pragmaCommand = new SqliteCommand("PRAGMA busy_timeout = 3000;", connection))
                {
                     pragmaCommand.ExecuteNonQuery();
                    Console.WriteLine("[INFO] PRAGMA busy_timeout set to 3000ms.");
                }

                string query = @"SELECT " + Min +", " + Max + " FROM SensorType WHERE SensorTypeID = @id";
				    using (var command = new SqliteCommand(query, connection))
                    {
						command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string CurrentMin = reader[Min].ToString() ?? string.Empty;
                                string CurrentMax = reader[Max].ToString() ?? string.Empty;

								string newMin = await DisplayPromptAsync("Change Min", "The current Min is " + CurrentMin + " What would you like to change it to?");
								string newMax = await DisplayPromptAsync("Change Max", "The current Max is " + CurrentMax + " What would you like to change it to?");


								if (float.TryParse(newMin, out float resultMin) && float.TryParse(newMax, out float resultMax))
								{
									string updateQuery = @"UPDATE SensorType SET " + Min + " = @newMin, " + Max + " = @newMax  WHERE SensorTypeID = @id";
									using (var UpdateCommand = new SqliteCommand(updateQuery, connection))
									{
										UpdateCommand.Parameters.AddWithValue("@id", id);
										UpdateCommand.Parameters.AddWithValue("@newMin", newMin);
										UpdateCommand.Parameters.AddWithValue("@newMax", newMax);
            							int rowsAffected = UpdateCommand.ExecuteNonQuery();
            
            							if (rowsAffected > 0)
            							{
											await DisplayAlert("Success", "Sensor Updated", "OK");
										}
										else
										{
											await DisplayAlert("Error", "No records were updated", "OK");
										}
									}
								}
								else
								{
									await DisplayAlert("Error", "Make sure that the inputs are numbers", "OK");
								}
								
                            }
                            else
                            {
                                await DisplayAlert("Error", "could not find in database", "OK");
                                Console.WriteLine("could not find in database.");
                                return;
                            }
                        }
                    }
			}
		}    
		catch (SqliteException ex)
        {
            await DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            Console.WriteLine("Error finding the database");
        }
		
	}

	public async void UpdateSensorFramework(int id)
	{
				try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("[INFO] Database connection opened successfully.");

                // Set busy timeout to prevent "database is locked" errors
                using (var pragmaCommand = new SqliteCommand("PRAGMA busy_timeout = 3000;", connection))
                {
                     pragmaCommand.ExecuteNonQuery();
                    Console.WriteLine("[INFO] PRAGMA busy_timeout set to 3000ms.");
                }

                string query = @"SELECT SensorTypeFirmware FROM SensorType WHERE SensorTypeID = @id";
				using (var command = new SqliteCommand(query, connection))
                {
					command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string SensorFirmware = reader["SensorTypeFirmware"].ToString() ?? string.Empty;

							string newFirmware = await DisplayPromptAsync("Change Firmware", "The current Sensor Firmware is version " + SensorFirmware + " What would you like to change it to?");

							if (float.TryParse(newFirmware, out float resultnewFirmware))
							{
								string updateQuery = @"UPDATE SensorType SET SensorTypeFirmware = @SensorFirmware  WHERE SensorTypeID = @id";
								using (var UpdateCommand = new SqliteCommand(updateQuery, connection))
								{
									UpdateCommand.Parameters.AddWithValue("@id", id);
									UpdateCommand.Parameters.AddWithValue("@SensorFirmware", newFirmware);
            						int rowsAffected = UpdateCommand.ExecuteNonQuery();
            
            						if (rowsAffected > 0)
            						{
										await DisplayAlert("Success", "Sensor Updated", "OK");
									}
									else
									{
										await DisplayAlert("Error", "No records were updated", "OK");
									}
								}
							}
							else
							{
								await DisplayAlert("Error", "Make sure that the inputs are numbers", "OK");
							}
								
                        }
                        else
                        {
                            await DisplayAlert("Error", "could not find in database", "OK");
                            Console.WriteLine("could not find in database.");
                            return;
                        }
                    }
                }
			}
		}    
		catch (SqliteException ex)
        {
            await DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            Console.WriteLine("Error finding the database");
        }
	}

	private void UpdateConAir1Btn_Click(object sender, EventArgs e)
	{
		int Id = 1;
		string Min = "Data1Min";
		string Max = "Data1Max";
		UpdateSensorCon(Id, Min, Max);
	}

	private void UpdateConAir2Btn_Click(object sender, EventArgs e)
	{
		int Id = 1;
		string Min = "Data2Min";
		string Max = "Data2Max";
		UpdateSensorCon(Id, Min, Max);

	}

	private void UpdateConAir3Btn_Click(object sender, EventArgs e)
	{
		int Id = 1;
		string Min = "Data3Min";
		string Max = "Data3Max";
		UpdateSensorCon(Id, Min, Max);
	}

	private void UpdateConAir4Btn_Click(object sender, EventArgs e)
	{
		int Id = 1;
		string Min = "Data4Min";
		string Max = "Data4Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateAirFirmwareBtn_Click(object sender, EventArgs e)
	{
		int Id = 1;

		UpdateSensorFramework(Id);
	}
	private void UpdateConWater1Btn_Click(object sender, EventArgs e)
	{
		int Id = 2;
		string Min = "Data1Min";
		string Max = "Data1Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateConWater2Btn_Click(object sender, EventArgs e)
	{
		int Id = 2;
		string Min = "Data2Min";
		string Max = "Data2Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateConWater3Btn_Click(object sender, EventArgs e)
	{
		int Id = 2;
		string Min = "Data3Min";
		string Max = "Data3Max";
		UpdateSensorCon(Id, Min, Max);
	}

	private void UpdateWaterFirmwareBtn_Click(object sender, EventArgs e)
	{
		int Id = 2;

		UpdateSensorFramework(Id);
	}

	private void UpdateConWeather1Btn_Click(object sender, EventArgs e)
	{
		int Id = 3;
		string Min = "Data1Min";
		string Max = "Data1Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateConWeather2Btn_Click(object sender, EventArgs e)
	{
		int Id = 3;
		string Min = "Data2Min";
		string Max = "Data2Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateConWeather3Btn_Click(object sender, EventArgs e)
	{
		int Id = 3;
		string Min = "Data3Min";
		string Max = "Data3Max";
		UpdateSensorCon(Id, Min, Max);
	}
	private void UpdateWeatherFirmwareBtn_Click(object sender, EventArgs e)
	{
		int Id = 3;

		UpdateSensorFramework(Id);
	}
	

}