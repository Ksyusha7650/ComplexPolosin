using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Linq;
using Brushes = System.Windows.Media.Brushes;


namespace ModelPolosin;

/// <summary>
///     Interaction logic for ChartsWindow.xaml
/// </summary>
public partial class ChartsWindow : Window
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;
    public SeriesCollection SeriesCollectionTemp { get; set; }
    public SeriesCollection SeriesCollectionVisc { get; set; }
    
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
        // SeriesCollection.Clear();
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
        SeriesCollectionTemp = new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint> (points),
                PointGeometrySize = 10,
                Fill = Brushes.Transparent
            }
        };
        PlotTemperature.DataContext = this;
        // LineSeries serie = new();
        // serie.Values = points;
        // serie.ToolTip = "ldkf";
        // serie.TooltipLabelFormatter = (chartPoint) => $"{YAxes[0].Name}: {chartPoint.PrimaryValue}, {XAxes[0].Name}: {chartPoint.SecondaryValue}";
        // SeriesCollectionTemp.Add(serie);
        //PointLabel = (chartPoint) => $"{YAxes[0].Name}: {chartPoint.X}, {XAxes[0].Name}: {chartPoint.Y}";
    }

    private void ViscosityLength()
    {
        // SeriesCollection.Clear();
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
        SeriesCollectionVisc = new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint> (points),
                PointGeometrySize = 10,
                Fill = Brushes.Transparent
            }
        };
        PlotViscosity.DataContext = this;
    }
}
