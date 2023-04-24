using System.Collections.Generic;
using System.Windows.Controls;

namespace ModelPolosin;

internal static class DrawTable
{
    private static List<double> _listOfChannelLength;
    private static List<double> _listOfTemperatures;
    private static List<double> _listOfViscosity;
    private static DataGrid _table;

    public static void DrawTableCalculations(
        List<double> listOfChannelLength,
        List<double> listOfTemperatures,
        List<double> listOfViscosity,
        DataGrid table)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        _table = table;
        MakeTable();
    }

    private static void MakeTable()
    {
        List<DataForTable> data = new();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            data.Add(new DataForTable
            {
                Coordinate = _listOfChannelLength[i],
                Temperature = _listOfTemperatures[i],
                Viscosity = _listOfViscosity[i]
            });
        _table.ItemsSource = data;
    }


    private record DataForTable
    {
        public double Coordinate { get; set; }
        public double Temperature { get; set; }
        public double Viscosity { get; set; }
    }
}