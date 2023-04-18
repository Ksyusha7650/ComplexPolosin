 using System.Collections.Generic;
 using System.Windows.Controls;
 using ScottPlot;

 public class DrawCharts
    {
        private readonly List<double> _listOfChannelLength;
        private readonly List<double> _listOfTemperatures;
        private readonly List<double> _listOfViscosity;
        private readonly WpfPlot _plot;
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
        }

        private static double[] ConvertToArray(IReadOnlyList<double> list)
        {
            var res = new double[list.Count];
            var i = 0;
            for (; i < list.Count; i++)
            {
                res[i] = list[i];
            }
            return res;
        }
        
        public void TemperatureLength()
        {
            _plot.Plot.Clear();
            _plot.Plot.Title("Graph of material temperature distribution along the length of the channel");
            _plot.Plot.XLabel("Length of the channel, m");
            _plot.Plot.YLabel("Material temperature, C");
            _plot.Plot.AddScatter(ConvertToArray(_listOfChannelLength), ConvertToArray(_listOfTemperatures));
            _plot.Refresh();
        }
        
        public void ViscosityLength()
        {
            _plot.Plot.Clear();
            _plot.Plot.Title("Graph of material viscosity distribution along the length of the channel");
            _plot.Plot.XLabel("Length of the channel, m");
            _plot.Plot.YLabel("Material viscosity, Pa*s");
            _plot.Plot.AddScatter(ConvertToArray(_listOfChannelLength), ConvertToArray(_listOfViscosity));
            _plot.Refresh();
        }

      
    }

    //public partial class PointShapeLineExample : UserControl {
    //    public PointShapeLineExample() {
    //        //InitializeComponent();

    //        SeriesCollection = new SeriesCollection
    //        {
    //            new LineSeries
    //            {
    //                Title = "Series 1",
    //                Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
    //            },
    //            new LineSeries
    //            {
    //                Title = "Series 2",
    //                Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
    //                PointGeometry = null
    //            },
    //            new LineSeries
    //            {
    //                Title = "Series 3",
    //                Values = new ChartValues<double> { 4,2,7,2,7 },
    //                PointGeometry = DefaultGeometries.Square,
    //                PointGeometrySize = 15
    //            }
    //        };

    //        Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
    //        YFormatter = value => value.ToString("C");

    //        //modifying the series collection will animate and update the chart
    //        SeriesCollection.Add(new LineSeries {
    //            Title = "Series 4",
    //            Values = new ChartValues<double> { 5, 3, 2, 4 },
    //            LineSmoothness = 0, //0: straight lines, 1: really smooth lines
    //            PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
    //            PointGeometrySize = 50,
    //            PointForeground = Brushes.Gray
    //        });

    //        //modifying any series values will also animate and update the chart
    //        SeriesCollection[3].Values.Add(5d);

    //        DataContext = this;
    //    }

    //    public SeriesCollection SeriesCollection { get; set; }
    //    public string[] Labels { get; set; }
    //    public Func<double, string> YFormatter { get; set; }
