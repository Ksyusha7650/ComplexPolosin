using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithm;
using Algorithm.Models;
using Database;
using Database.Interfaces;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Calculation _calculation;
    private readonly DataService _dataService;

    private readonly string[] marks;
    
    public MainWindow()
    {
        InitializeComponent();
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        _dataService = new DataService();
        marks = _dataService.GetMarks();
        MarkComboBox.Items.Add("--default");
        TypeComboBox.Items.Add("--default");
        foreach (var mark in marks) 
            MarkComboBox.Items.Add(mark);
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        Calculate();
        // TableWindow tableWindow = new();
        // tableWindow.Show();
        TemperatureProductTextBox.Text = GetTemperature().ToString();
        ViscosityProductTextBox.Text = GetViscosity().ToString();
        EfficiencyTextBox.Text = GetEfficiency().ToString();
        List<double> listOfChannelLength = _calculation.ListOfChannelLength();
        List<double> listOfTemperatures = _calculation.ListOfTemperatures(listOfChannelLength);
        List<double> listOfViscosity = _calculation.ListOfViscosity(listOfTemperatures);
        
        ChartsWindow chartsWindow = new(listOfChannelLength, listOfTemperatures, listOfViscosity);
        chartsWindow.Show();

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
        // if (MarkComboBox.SelectedItem.ToString() is null)
        // {
        //     MessageBox.Show("Mark isn't chosen");
        //     return;
        // }

        //var mark = MarkComboBox.SelectedItem.ToString();
        // var result = await _dataService.GetGeometricParameters(mark);
        // var model = new GeometricParameters(
        //     mark,
        //     result.Height,
        //     result.Width,
        //     result.Length);
        // MarkComboBox.SelectedItem = model.Mark;
        // HeightTextBox.Text = model.Height.ToString();
        // WidthTextBox.Text = model.Width.ToString();1
        // LengthTextBox.Text = model.Length.ToString();
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

        var result = _dataService.GetGeometricParameters(mark).Result;
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
}