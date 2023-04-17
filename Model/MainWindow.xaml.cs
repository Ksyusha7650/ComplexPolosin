using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Algorithm;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private readonly Calculation _calculation;
    
    public MainWindow()
    {
        InitializeComponent();
        _calculation = new Calculation();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        TableWindow tableWindow = new();
        tableWindow.Show();
        TemperatureProductTextBox.Text = GetTemperature().ToString();
        ViscosityProductTextBox.Text = GetViscosity().ToString();
        EfficiencyTextBox.Text = GetEfficiency().ToString();
    }

    private double GetTemperature()
    {
        return _calculation.Temperature(_calculation._L);
    }
    
    private double GetViscosity()
    {
        return _calculation.Viscosity(GetTemperature());
    }
    
    private double GetEfficiency()
    {
        return _calculation.Effiency();
    }
}