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
using Algorithm;


namespace ModelPolosin {
    /// <summary>
    /// Interaction logic for Table.xaml
    /// </summary>
    public partial class TableWindow : Window {
        public TableWindow() {
            InitializeComponent();
        }



        class dataForTable {
            public double coordinate { get; set; }
            public double temperature { get; set; }
            public double viscosity { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Calculation calc = new();
            calc.InitializingVariables();
            List<double> listOfCoordinates = calc.ListOfChannelLength();
            List<double> listOfTemperatures = calc.ListOfTemperatures(listOfCoordinates);
            List<double> listOfViscosity = calc.ListOfViscosity(listOfTemperatures);
            List<dataForTable> data = new();
            for (int i = 0; i < listOfCoordinates.Count; i++) {
                data.Add(new dataForTable { coordinate = listOfCoordinates[i], temperature = listOfTemperatures[i], viscosity = listOfViscosity[i] });
            }
            tableWithCalc.ItemsSource = data;
        }
    }
}
