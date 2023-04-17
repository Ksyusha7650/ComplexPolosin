using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace ModelPolosin {
    /// <summary>
    /// Interaction logic for ChartsWindow.xaml
    /// </summary>
    public partial class ChartsWindow : Window
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
        
        private void TemperatureLength()
        {
            WpfPlot1.Plot.Clear();
            WpfPlot1.Plot.Title("Graph of material temperature distribution along the length of the channel");
            WpfPlot1.Plot.XLabel("Length of the channel, m");
            WpfPlot1.Plot.YLabel("Material temperature, C");
            WpfPlot1.Plot.AddScatter(ConvertToArray(_listOfChannelLength), ConvertToArray(_listOfTemperatures));
            WpfPlot1.Refresh();
        }
        
        private void ViscosityLength()
        {
            WpfPlot1.Plot.Clear();
            WpfPlot1.Plot.Title("Graph of material viscosity distribution along the length of the channel");
            WpfPlot1.Plot.XLabel("Length of the channel, m");
            WpfPlot1.Plot.YLabel("Material viscosity, Pa*s");
            WpfPlot1.Plot.AddScatter(ConvertToArray(_listOfChannelLength), ConvertToArray(_listOfViscosity));
            WpfPlot1.Refresh();
        }

        private void ChartComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            if (ChartComboBox.SelectedIndex == 0)
            {
                TemperatureLength();
            }
            else
            {
                ViscosityLength();
            }
        }
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
