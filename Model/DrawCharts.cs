using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ModelPolosin;

public class DrawCharts
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;
    private readonly CartesianChart _plot;

    public DrawCharts(List<double> listOfChannelLength, List<double> listOfTemperatures, List<double> listOfViscosity,
        CartesianChart plot)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        _plot = plot;
    }

    public void TemperatureLength()
    {
        _plot.AxisX.Clear();
        _plot.AxisY.Clear();
        _plot.AxisX.Add(new Axis { Title = "Length of the channel, m", FontSize = 15 });
        _plot.AxisY.Add(new Axis { Title = "Material temperature, C", FontSize = 15 });
        var points = new ChartValues<ObservablePoint>();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = _listOfTemperatures[i]
            });
        new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(points),
                PointGeometrySize = 10
            }
        };
        _plot.DataContext = this;
    }

    public void ViscosityLength()
    {
        _plot.AxisX.Clear();
        _plot.AxisY.Clear();
        _plot.AxisX.Add(new Axis { Title = "Length of the channel, m", FontSize = 15 });
        _plot.AxisY.Add(new Axis { Title = "Material viscosity, Pa*s", FontSize = 15 });
        var points = new ChartValues<ObservablePoint>();
        for (var i = 0; i < _listOfChannelLength.Count; i++)
            points.Add(new ObservablePoint
            {
                X = _listOfChannelLength[i],
                Y = _listOfViscosity[i]
            });
        new SeriesCollection
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(points),
                PointGeometrySize = 10
            }
        };
        _plot.DataContext = this;
    }
}