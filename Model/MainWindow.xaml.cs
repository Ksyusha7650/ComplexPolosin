using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithm;
using Database;
using Database.Interfaces;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Calculation _calculation;
    private readonly DataService _dataService;

    private readonly string[] marks;

    public MainWindow()
    {
        InitializeComponent();
        _dataService = new DataService();
        _calculation = new Calculation();
        marks = _dataService.GetMarks();
        MarkComboBox.Items.Add("--default");
        foreach (var mark in marks) MarkComboBox.Items.Add(mark);
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
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

    private async Task Calculate()
    {
        if (MarkComboBox.SelectedItem.ToString() is null)
        {
            MessageBox.Show("Mark isn't chosen");
            return;
        }

        var mark = MarkComboBox.SelectedItem.ToString();
        var result = await _dataService.GetGeometricParameters(mark);
        var model = new ChannelModel(
            mark,
            result.Height,
            result.Width,
            result.Length);
        MarkComboBox.SelectedItem = model.Mark;
        HeightTextBox.Text = model.Height.ToString();
        WidthTextBox.Text = model.Width.ToString();
        LengthTextBox.Text = model.Length.ToString();
    }

    private void MarkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var mark = MarkComboBox.SelectedItem.ToString();
        if (mark == "--default")
        {
            Clear();
            return;
        }

        var result = _dataService.GetGeometricParameters(mark).Result;
        var model = new ChannelModel(
            mark,
            result.Height,
            result.Width,
            result.Length);
        MarkComboBox.SelectedItem = model.Mark;
        HeightTextBox.Text = model.Height.ToString();
        WidthTextBox.Text = model.Width.ToString();
        LengthTextBox.Text = model.Length.ToString();
    }

    private void Clear()
    {
        MarkComboBox.SelectedItem = "--default";
        HeightTextBox.Text = "0";
        WidthTextBox.Text = "0";
        LengthTextBox.Text = "0";
    }
}