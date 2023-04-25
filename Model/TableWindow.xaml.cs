using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Algorithm.Models;

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
        SetUpColumns();
        // List<DataForTable> data = new();
        // for (var i = 0; i < _listOfChannelLength.Count; i++)
        //     data.Add(new DataForTable
        //     {
        //         Coordinate = _listOfChannelLength[i], Temperature = _listOfTemperatures[i],
        //         Viscosity = _listOfViscosity[i]
        //     });
        // tableWithCalc.ItemsSource = data;
        // List<DataForTable> data = new();
        for (int i = 0; i < _listOfViscosity.Count; i++)
        {
            TableWithCalc.Items.Add(new DataForTable(
                _listOfChannelLength[i],
                Math.Round(_listOfTemperatures[i], 2),
                Math.Round(_listOfViscosity[i], 0)));
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // List<DataForTable> data = new();
        // for (var i = 0; i < _listOfChannelLength.Count; i++)
        //     data.Add(new DataForTable
        //     {
        //         Coordinate = _listOfChannelLength[i], Temperature = _listOfTemperatures[i],
        //         Viscosity = _listOfViscosity[i]
        //     });
        // tableWithCalc.ItemsSource = data;
    }

    private void SetUpColumns()
    {
        var column = new DataGridTextColumn
        {
            Header = "Coordinate, m",
            Binding = new Binding("Coordinate")
        };
        TableWithCalc.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Temperature, C",
            Binding = new Binding("Temperature")
        };
        TableWithCalc.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Viscosity, Pa*s",
            Binding = new Binding("Viscosity")
        };
        TableWithCalc.Columns.Add(column);
    }

    // private class DataForTable
    // {
    //     public double Coordinate { get; set; }
    //     public double Temperature { get; set; }
    //     public double Viscosity { get; set; }
    // }
}