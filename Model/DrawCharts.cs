using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using ScottPlot;
using ScottPlot.Plottable;

namespace ModelPolosin;

public class DrawCharts
{
    private readonly List<double> _listOfChannelLength;
    private readonly List<double> _listOfTemperatures;
    private readonly List<double> _listOfViscosity;
    private MarkerPlot _highlightedPoint;
    private int _lastHighlightedIndex = -1;
    private readonly WpfPlot _plot;
    private ScatterPlot? _scatterPlot;

    public DrawCharts(
        List<double> listOfChannelLength,
        List<double> listOfTemperatures,
        List<double> listOfViscosity,
        WpfPlot plot)
    {
        _listOfChannelLength = listOfChannelLength;
        _listOfTemperatures = listOfTemperatures;
        _listOfViscosity = listOfViscosity;
        _plot = plot;
        SetupHighlightedPoint();
    }

    private void SetupHighlightedPoint()
    {
        _highlightedPoint = _plot.Plot.AddPoint(0, 0);
        _highlightedPoint.Color = Color.Red;
        _highlightedPoint.MarkerSize = 10;
        _highlightedPoint.MarkerShape = MarkerShape.openCircle;
        _highlightedPoint.IsVisible = false;
        _plot.Refresh();
    }

    public void TemperatureLength()
    {
        _plot.Plot.Clear();
        _plot.Plot.Title("Graph of material temperature distribution along the length of the channel");
        _plot.Plot.XLabel("Length of the channel, m");
        _plot.Plot.YLabel("Material temperature, C");
        _scatterPlot = _plot.Plot.AddScatter(_listOfChannelLength.ToArray(), _listOfTemperatures.ToArray());
        _plot.Refresh();
    }

    public void ViscosityLength()
    {
        _plot.Plot.Clear();
        _plot.Plot.Title("Graph of material viscosity distribution along the length of the channel");
        _plot.Plot.XLabel("Length of the channel, m");
        _plot.Plot.YLabel("Material viscosity, Pa*s");
        _scatterPlot = _plot.Plot.AddScatter(_listOfChannelLength.ToArray(), _listOfViscosity.ToArray());
        _plot.Refresh();
    }
    public void plot_MouseMove(object sender, MouseEventArgs e)
    {
        var (mouseCoordX, mouseCoordY) = _plot.GetMouseCoordinates();
        var xyRatio = _plot.Plot.XAxis.Dims.PxPerUnit / _plot.Plot.YAxis.Dims.PxPerUnit;
        if (_scatterPlot is null) return;
        var (pointX, pointY, pointIndex) = _scatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
        _highlightedPoint.X = pointX;
        _highlightedPoint.Y = pointY;
        _highlightedPoint.IsVisible = true;
        if (_lastHighlightedIndex == pointIndex) 
            return;
        _lastHighlightedIndex = pointIndex;
        _plot.Render();
        
    //    _plot.Plot.Title = $"({Math.Round(mouseCoordX, 2)}, {Math.Round(mouseCoordY, 2)})";
         _plot.Plot.PlotText("",0, 100);
         _plot.Plot.PlotText($"Point index {pointIndex} at ({pointX}, {pointY})",0,100);
    }
}