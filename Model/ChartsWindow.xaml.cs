using System;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for ChartsWindow.xaml
/// </summary>
public partial class ChartsWindow
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;

    public ChartsWindow(List<double> listOfChannelLength, List<double> listOfTemperatures, List<double> listOfViscosity)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        InitializeComponent();
        TemperatureLength();
        ViscosityLength();
    }

    private Func<ChartPoint, string>? PointLabel { get; set; }

    private void TemperatureLength()
    {
        // SeriesCollection.Clear();
        // PlotTemperature.AxisX.Clear();
        // PlotTemperature.AxisY.Clear();
        PlotTemperature.AxisX.Add(new Axis { Title = "Channel length coordinate, m", FontSize = 15 });
        PlotTemperature.AxisY.Add(new Axis { Title = "Material temperature, °С", FontSize = 15 });
        var points = new ChartValues<ObservablePoint>();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = _listOfTemperatures[i]
            });
        PointLabel = chartPoint => $"Coordinate: {chartPoint.X}, Temperature: {chartPoint.Y}";


        new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(points),
                PointGeometrySize = 10,
                Fill = Brushes.Transparent,
                LabelPoint = PointLabel,
                Title = ""
            }
        };

        PlotTemperature.DataContext = this;
        // LineSeries series = new();
        // series.Values = points;
        // series.ToolTip = "tool";
        // series.TooltipLabelFormatter = (chartPoint) => $"{YAxes[0].Name}: {chartPoint.PrimaryValue}, {XAxes[0].Name}: {chartPoint.SecondaryValue}";
        // SeriesCollectionTemp.Add(series);
        // PointLabel = (chartPoint) =>
        // {
        //     
        //     return "pointLabel";
        // };
    }

    private void ViscosityLength()
    {
        // SeriesCollection.Clear();
        // PlotViscosity.AxisX.Clear();
        // PlotViscosity.AxisY.Clear();
        PlotViscosity.AxisX.Add(new Axis { Title = "Channel length coordinate, m", FontSize = 15 });
        PlotViscosity.AxisY.Add(new Axis { Title = "Material viscosity, Pa*s", FontSize = 15 });
        var points = new ChartValues<ObservablePoint>();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = Math.Round(_listOfViscosity[i], 0)
            });
        PointLabel = chartPoint => $"Coordinate: {chartPoint.X}, Viscosity: {chartPoint.Y}";
        new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(points),
                PointGeometrySize = 10,
                Fill = Brushes.Transparent,
                LabelPoint = PointLabel,
                Title = ""
            }
        };
        PlotViscosity.DataContext = this;
    }
}