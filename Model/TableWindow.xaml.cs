using System.Collections.Generic;
using System.Windows;
using Algorithm;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for Table.xaml
/// </summary>
public partial class TableWindow : Window
{
    public TableWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Calculation calc = new();
        calc.InitializingVariables();
        var listOfCoordinates = calc.ListOfChannelLength();
        var listOfTemperatures = calc.ListOfTemperatures(listOfCoordinates);
        var listOfViscosity = calc.ListOfViscosity(listOfTemperatures);
        List<dataForTable> data = new();
        for (var i = 0; i < listOfCoordinates.Count; i++)
            data.Add(new dataForTable
            {
                coordinate = listOfCoordinates[i], temperature = listOfTemperatures[i], viscosity = listOfViscosity[i]
            });
        tableWithCalc.ItemsSource = data;
    }


    private class dataForTable
    {
        public double coordinate { get; set; }
        public double temperature { get; set; }
        public double viscosity { get; set; }
    }
}