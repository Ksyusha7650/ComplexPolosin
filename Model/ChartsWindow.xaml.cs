using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for ChartsWindow.xaml
/// </summary>
public partial class ChartsWindow : Window
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;
    public SeriesCollection SeriesCollection { get; set; }


    public ChartsWindow(List<double> listOfChannelLength, List<double> listOfTemperatures, List<double> listOfViscosity)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        InitializeComponent();
        TemperatureLength();
        ViscosityLength();
    }

    private void TemperatureLength()
    {
        // PlotTemperature.AxisX.Clear();
        // PlotTemperature.AxisY.Clear();
        PlotTemperature.AxisX.Add(new Axis{Title = "Length of the channel, m", FontSize = 15});
        PlotTemperature.AxisY.Add(new Axis{Title = "Material temperature, C", FontSize = 15});
        var points = new ChartValues<ObservablePoint>();
        for (int i = 0; i < _listOfChannelLength.Count; i++)
        {
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = _listOfTemperatures[i]
            });
        }
        SeriesCollection = new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint> (points),
                PointGeometrySize = 10
            }
        };
        PlotTemperature.DataContext = this;
    }

    private void ViscosityLength()
    {
        // PlotViscosity.AxisX.Clear();
        // PlotViscosity.AxisY.Clear();
        PlotViscosity.AxisX.Add(new Axis{Title = "Length of the channel, m", FontSize = 15});
        PlotViscosity.AxisY.Add(new Axis{Title = "Material viscosity, Pa*s", FontSize = 15});
        var points = new ChartValues<ObservablePoint>();
        for (int i = 0; i < _listOfChannelLength.Count; i++)
        {
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = _listOfViscosity[i]
            });
        }
        SeriesCollection = new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint> (points),
                PointGeometrySize = 10
            }
        };
        PlotViscosity.DataContext = this;
    }
}
