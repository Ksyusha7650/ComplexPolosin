using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithm;
using Algorithm.Models;
using Database;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DataService _dataService;

    private string[] _marks;
    private List<EmpiricCoefficients> _empiricCoefficients;
    private Calculation _calculation;
    private DrawCharts _charts;

    public MainWindow()
    {
        InitializeComponent();
        var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        _dataService = new DataService();
        SetFieldsFromDataBase();
        MarkComboBox.Items.Add("--default");//вынести в другой метод 
        TypeComboBox.Items.Add("--default");//вынести в другой метод
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        Calculate();
        TemperatureProductTextBox.Text = GetTemperature().ToString();
        ViscosityProductTextBox.Text = GetViscosity().ToString();
        EfficiencyTextBox.Text = GetEfficiency().ToString();
        var listOfChannelLength = _calculation.ListOfChannelLength();
        var listOfTemperatures = _calculation.ListOfTemperatures(listOfChannelLength);
        var listOfViscosity = _calculation.ListOfViscosity(listOfTemperatures);
        _charts = new DrawCharts(
            listOfChannelLength,
            listOfTemperatures,
            listOfViscosity,
            WpfPlot1);
        _charts.TemperatureLength();
        TableWindow tableWindow = new(
            listOfChannelLength,
            listOfTemperatures,
            listOfViscosity);
        tableWindow.Show();
    }

    private double GetTemperature()
    {
        return Math.Round(_calculation.Temperature(Convert.ToDouble(LengthTextBox.Text)), 2);
    }

    private double GetViscosity()
    {
        return _calculation.Viscosity(GetTemperature());
    }

    private double GetEfficiency()
    {
        return _calculation.Effiency();
    }

    private void Calculate()
    {
        try
        {
            _calculation = new Calculation(
                new EmpiricCoefficients(
                    Convert.ToDouble(M0TextBox.Text),
                    Convert.ToDouble(EaTextBox.Text),
                    Convert.ToDouble(TrTextBox.Text),
                    Convert.ToDouble(NTextBox.Text),
                    Convert.ToDouble(AlphaUTextBox.Text)),
                new GeometricParameters(
                    MarkComboBox.SelectedItem.ToString(),
                    Convert.ToDouble(HeightTextBox.Text),
                    Convert.ToDouble(LengthTextBox.Text),
                    Convert.ToDouble(WidthTextBox.Text)),
                new PropertiesOfMaterial(
                    TypeComboBox.SelectedItem.ToString(),
                    Convert.ToDouble(DensityTextBox.Text),
                    Convert.ToDouble(SpecificHeartTextBox.Text),
                    Convert.ToDouble(MeltingPointTextBox.Text)),
                new VariableParameters(
                    Convert.ToDouble(CoverTemperatureTextBox.Text),
                    Convert.ToDouble(CoverVelocityTextBox.Text),
                    Convert.ToDouble(StepTextBox.Text)));
        }
        catch (Exception)
        {
            MessageBox.Show("Check have all input fields!");
        }
    }

    private void MarkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var mark = MarkComboBox.SelectedItem.ToString();
        if (mark == "--default")
        {
            Clear();
            return;
        }

        var result = _dataService.ChannelDataBase.GetGeometricParameters(mark).Result;
        var model = new GeometricParameters(
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
        //MarkComboBox.SelectedItem = "--default";
        // HeightTextBox.Text = "0";
        // WidthTextBox.Text = "0";
        // LengthTextBox.Text = "0";
    }

    private void ChartComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_charts is null) return;
        if (ChartComboBox.SelectedIndex == 0)
            _charts.TemperatureLength();
        else
            _charts.ViscosityLength();
    }

    private void WpfPlot1_OnMouseMove(object sender, MouseEventArgs e)
    {
        _charts.plot_MouseMove(sender, e);
    }

    private void SetFieldsFromDataBase()
    {
        _marks = _dataService.ChannelDataBase.GetMarks();
        foreach (var mark in _marks)
            MarkComboBox.Items.Add(mark);
        _empiricCoefficients = _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficients();

    }
}