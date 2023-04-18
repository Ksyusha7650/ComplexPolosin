using System.Collections.Generic;
using System.Windows;
using Algorithm;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for Table.xaml
/// </summary>
public partial class TableWindow : Window
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;
    public TableWindow(List<double> listOfChannelLength, List<double> listOfTemperatures, List<double> listOfViscosity)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        List<DataForTable> data = new();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            data.Add(new DataForTable
            {
                Coordinate = _listOfChannelLength[i], Temperature = _listOfTemperatures[i], Viscosity = _listOfViscosity[i]
            });
        tableWithCalc.ItemsSource = data;
    }


    private class DataForTable
    {
        public double Coordinate { get; set; }
        public double Temperature { get; set; }
        public double Viscosity { get; set; }
    }
}