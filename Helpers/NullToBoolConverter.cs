using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SECW.Helpers
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null; // Return true if value is not null, otherwise false
        }
        //this file is responsible for converting null values to boolean values
        //and vice versa. It implements the IValueConverter interface, which is used in data binding scenarios.
        //The Convert method checks if the value is null and returns true if it is not, otherwise false.
        //The ConvertBack method is not implemented, as it is not needed in this case.
        //the reason this was created is to handle the case where a null value is passed to a boolean property in the UI.
        //This is useful for scenarios where you want to show or hide UI elements based on the presence of a value.
        // this is helpful with the binding of the selected sensor in the EnvironmentalScientistPage.xaml file.
        //which allows the UI to react to changes in the selected sensor and update the UI accordingly, more work is required to ensure that editing changes accordingly includes filling in the fields of the selected sensor to allow the user to reconfigure it without having to go to the database again as a reference point of what the sensors settings are.


        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}