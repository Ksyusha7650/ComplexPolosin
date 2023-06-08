using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Algorithm;
using Algorithm.Models;
using Database;
using Database.Models;
using Microsoft.Win32;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly List<string> _incorrectValues = new();
    
    private Calculation _calculation;

    private DrawCharts _charts;

    private DataService _dataService;
    private EmpiricCoefficientsModel[] _empiricCoefficients;
    private string[] _marks, _types;

    public Color BorderColor = new()
    {
        A = 100
    };

    public MainWindow()
    {
        InitializeComponent();
        var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        GetDataFromDataBase();
        SetUpColumns();
    }
    private bool CheckTextBox => _incorrectValues.Count == 0;

    /*private void SetRamAndTime(object sender, EventArgs e)
    {
        TimeTextBox.Text = MyCounter.NextValue().ToString();
    }*/

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void ZeroValidationTextBox(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var exist = _incorrectValues.Any(incorrectTextBox => incorrectTextBox == textBox?.Name);
        if (textBox?.Text is "0" or "")
        {
            textBox.BorderBrush = Brushes.Red;
            if (!exist)
                _incorrectValues.Add(textBox.Name);
        }
        else
        {
            textBox.BorderBrush = new SolidColorBrush(BorderColor);
            if (exist)
                _incorrectValues.Remove(textBox.Name);
        }
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        var startTime = DateTime.Now;
        var currentProcess = Process.GetCurrentProcess();
        var counter1 = currentProcess.WorkingSet64;
        if (!Calculate())
        {
            
            return;
        }
        TemperatureProductTextBox.Text = GetTemperature().ToString(CultureInfo.InvariantCulture);
        ViscosityProductTextBox.Text = Math.Round(GetViscosity(), 0).ToString(CultureInfo.InvariantCulture);
        EfficiencyTextBox.Text = GetEfficiency().ToString(CultureInfo.InvariantCulture);
        var ts = DateTime.Now.Subtract(startTime);
        var elapsedTime = $"{ts.Seconds:00}.{ts.Milliseconds:00} s";
        currentProcess = Process.GetCurrentProcess();
        var counter2 = currentProcess.WorkingSet64;
        RamTextBox.Text = $"{Math.Round(counter2 / Math.Pow(1024, 2), 2) } MB";
        TimeTextBox.Text = elapsedTime;
        ExportButton.IsEnabled = true;
        ShowResultsButton.IsEnabled = true;
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
        return _calculation.GetThroughout();
    }

    private bool Calculate()
    {
        if (!CheckTextBox)
        {
            MessageBox.Show("Fix highlighted red fields!");
            return false;
        }
        try
        {
            if (_empiricCoefficients.Length < 5)
            {
                MessageBox.Show("There are not enough empiric coefficients!");
                return false;
            }
            _calculation = new Calculation(
                new EmpiricCoefficients(
                    Convert.ToDouble(_empiricCoefficients[0].Value),
                    Convert.ToDouble(_empiricCoefficients[1].Value),
                    Convert.ToDouble(_empiricCoefficients[2].Value),
                    Convert.ToDouble(_empiricCoefficients[3].Value),
                    Convert.ToDouble(_empiricCoefficients[4].Value)),
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


            /*_calculation = new Calculation(
                new EmpiricCoefficients(29940, 20000, 190, 0.35, 425),
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
                    Convert.ToDouble(StepTextBox.Text)));*/
        }
        catch (Exception)
        {
            MessageBox.Show("Something goes wrong!");
            return false;
        }

        return true;
    }

    private void MarkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var mark = MarkComboBox.SelectedItem.ToString();
        if (mark == "--default") Clear();

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
        // if (_charts is null) return;
        // if (ChartComboBox.SelectedIndex == 0)
        //     _charts.TemperatureLength();
        // else
        //     _charts.ViscosityLength();
    }

    private void GetDataFromDataBase()
    {
        _dataService = new DataService();
        _marks = _dataService.ChannelDataBase.GetMarks();
        foreach (var mark in _marks)
            MarkComboBox.Items.Add(mark);
        _types = _dataService.MaterialDataBase.GetTypes();
        foreach (var type in _types) TypeComboBox.Items.Add(type);
    }

    private async void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var type = TypeComboBox.SelectedItem.ToString();
        var idType = _dataService.MaterialDataBase.GetIdParameterSet(type);
        EmpiricCoefficientsDataGrid.Items.Clear();
        _empiricCoefficients = _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficients(idType).Result;
        foreach (var empiricCoefficient in _empiricCoefficients)
            EmpiricCoefficientsDataGrid.Items.Add(new EmpiricCoefficientsToDataGrid(
                empiricCoefficient.IdEc,
                empiricCoefficient.Name,
                empiricCoefficient.Symbol,
                empiricCoefficient.Unit ?? " ",
                empiricCoefficient.Value));
        var (_, density, specificHeat, meltingPoint) =
            await _dataService.MaterialDataBase.GetMaterialProperties(idType);
        DensityTextBox.Text = density.ToString(CultureInfo.InvariantCulture);
        SpecificHeartTextBox.Text = specificHeat.ToString(CultureInfo.InvariantCulture);
        MeltingPointTextBox.Text = meltingPoint.ToString(CultureInfo.InvariantCulture);
    }

    private void SetUpColumns()
    {
        var column = new DataGridTextColumn
        {
            Header = "№",
            Binding = new Binding("IdEc")
        };
        EmpiricCoefficientsDataGrid.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding("Name")
        };
        EmpiricCoefficientsDataGrid.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Symbol",
            Binding = new Binding("Symbol")
        };
        EmpiricCoefficientsDataGrid.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Unit",
            Binding = new Binding("Unit")
        };
        EmpiricCoefficientsDataGrid.Columns.Add(column);
        column = new DataGridTextColumn
        {
            Header = "Value",
            Binding = new Binding("Value")
        };
        EmpiricCoefficientsDataGrid.Columns.Add(column);
    }

    private void ButtonShowResult_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckTextBox || _calculation is null)
        {
            MessageBox.Show("Make calculation first!");
            return;
        }

        var listOfChannelLength = _calculation.ListOfChannelLength();
        var listOfTemperatures = _calculation.ListOfTemperatures(listOfChannelLength);
        var listOfViscosity = _calculation.ListOfViscosity(listOfTemperatures);

        TableWindow tableWindow = new(listOfChannelLength, listOfTemperatures, listOfViscosity);
        tableWindow.Show();

        ChartsWindow chartsWindow = new(listOfChannelLength, listOfTemperatures, listOfViscosity);
        chartsWindow.Show();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        new LoginWindow().Show();
        Close();
    }

    private void AnimationButton_Click(object sender, RoutedEventArgs e) {
        AnimationWindow animationWindow = new(double.Parse(HeightTextBox.Text), double.Parse(LengthTextBox.Text), double.Parse(CoverVelocityTextBox.Text));
        animationWindow.ShowDialog();
    }

    private void ExcelButton_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckTextBox || _calculation is null)
        {
            MessageBox.Show("Make calculation first!");
            return;
        }

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.xlsx)|*.xlsx",
            FilterIndex = 2,
            RestoreDirectory = true
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            var filePath = saveFileDialog.FileName;
            try
            {
                File.Delete(filePath);
            }
            catch (IOException)
            {
                MessageBox.Show("This file is opened!", "Warning!");
                return;
            }

            WorkWithExcel excelWork = new();
            if (excelWork.Open(filePath))
            {
                excelWork.SetData("A", 1, "Channel");
                excelWork.SetData("A", 2, "Mark:");
                excelWork.SetData("B", 2, MarkComboBox.Text);

                excelWork.SetData("A", 3, "Geometric parameters:");
                excelWork.SetData("A", 4, "Height, m:");
                excelWork.SetData("B", 4, HeightTextBox.Text);
                excelWork.SetData("A", 5, "Length, m:");
                excelWork.SetData("B", 5, LengthTextBox.Text);
                excelWork.SetData("A", 6, "Width, m:");
                excelWork.SetData("B", 6, WidthTextBox.Text);

                excelWork.SetData("A", 7, "Cover:");
                excelWork.SetData("A", 8, "Velocity, m/s:");
                excelWork.SetData("B", 8, CoverVelocityTextBox.Text);
                excelWork.SetData("A", 9, "Temperature, °С");
                excelWork.SetData("B", 9, CoverTemperatureTextBox.Text);

                excelWork.SetData("A", 11, "Step, m:");
                excelWork.SetData("B", 11, StepTextBox.Text);

                excelWork.SetData("D", 1, "Material");
                excelWork.SetData("D", 2, "Type:");
                excelWork.SetData("E", 2, TypeComboBox.Text);

                excelWork.SetData("D", 3, "Density, kg/m^3:");
                excelWork.SetData("E", 3, DensityTextBox.Text);
                excelWork.SetData("D", 4, "Specific heat, J/(kg*°С):");
                excelWork.SetData("E", 4, SpecificHeartTextBox.Text);
                excelWork.SetData("D", 5, "Melting point, °С");
                excelWork.SetData("E", 5, MeltingPointTextBox.Text);

                excelWork.SetData("D", 6, "Empiric Coefficients:");
                var numberRow = 7;
                // foreach (var emp in _empiricCoefficients)
                // {
                //     excelWork.SetData("D", numberRow, emp.Name);
                //     excelWork.SetData("E", numberRow, emp.Value.ToString());
                //     numberRow++;
                // }

                var listOfChannelLength = _calculation.ListOfChannelLength();
                var listOfTemperatures = _calculation.ListOfTemperatures(listOfChannelLength);
                var listOfViscosity = _calculation.ListOfViscosity(listOfTemperatures);
                excelWork.SetData("G", 1, "Coordinates, m");
                excelWork.SetData("H", 1, "Temperature, °С");
                excelWork.SetData("I", 1, "Viscosity, Pa*s");
                for (var i = 0; i < listOfChannelLength.Count; i++)
                {
                    excelWork.SetData("G", i + 2, Math.Round(listOfChannelLength[i], 2).ToString());
                    excelWork.SetData("H", i + 2, Math.Round(listOfTemperatures[i], 2).ToString());
                    excelWork.SetData("I", i + 2, Math.Round(listOfViscosity[i], 0).ToString());
                }

                excelWork.SetData("K", 1, "Criteria indicators of the process:");
                excelWork.SetData("K", 2, "Product temperature, °С:");
                excelWork.SetData("L", 2, TemperatureProductTextBox.Text);
                excelWork.SetData("K", 3, "Product viscosity, Pa*s");
                excelWork.SetData("L", 3, ViscosityProductTextBox.Text);
                excelWork.SetData("K", 4, "Throughput, kg/h");
                excelWork.SetData("L", 4, EfficiencyTextBox.Text);
                //excelWork.DrawInExcel(witchOfAgnesi.pairs.Count);

                excelWork.Save();
            }

            MessageBox.Show("Excel was successfully saved!", "Saving!");
        }
        else
        {
            MessageBox.Show("File was not saved!", "Warning!");
        }
    }
}