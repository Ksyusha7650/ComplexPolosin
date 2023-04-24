﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithm;
using Algorithm.Models;
using Database;
using Database.Models;
using Microsoft.Win32;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Calculation _calculation;
    private DrawCharts _charts;
    // private DataService _dataService;
    private EmpiricCoefficientsModel[] _empiricCoefficients;

    private string[] _marks, _types;

    public MainWindow()
    {
        InitializeComponent();
        // var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        // customCulture.NumberFormat.NumberDecimalSeparator = ".";
        // Thread.CurrentThread.CurrentCulture = customCulture;
        // GetDataFromDataBase();
        MarkComboBox.Items.Add("--default");
        TypeComboBox.Items.Add("--default");
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
        if (ChartComboBox.SelectedIndex == 0)
        {
            _charts = new DrawCharts(
                listOfChannelLength,
                listOfTemperatures,
                listOfViscosity,
                Plot);
            _charts.TemperatureLength();
        }

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

        // var result = _dataService.ChannelDataBase.GetGeometricParameters(mark).Result;
        // var model = new GeometricParameters(
        //     mark,
        //     result.Height,
        //     result.Width,
        //     result.Length);
        // MarkComboBox.SelectedItem = model.Mark;
        // HeightTextBox.Text = model.Height.ToString();
        // WidthTextBox.Text = model.Width.ToString();
        // LengthTextBox.Text = model.Length.ToString();
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

    private void Plot_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void GetDataFromDataBase()
    {
        // _dataService = new DataService();
        // _marks = _dataService.ChannelDataBase.GetMarks();
        // foreach (var mark in _marks)
        //     MarkComboBox.Items.Add(mark);
        // _types = _dataService.MaterialDataBase.GetTypes();
        // foreach (var type in _types)
        // {
        //     TypeComboBox.Items.Add(type);
        // }
    }

    private void ExcelButton_Click(object sender, RoutedEventArgs e) {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.xlsx)|*.xlsx";
        saveFileDialog.FilterIndex = 2;
        saveFileDialog.RestoreDirectory = true;
        string filePath;
        if (saveFileDialog.ShowDialog() == true) {
            filePath = saveFileDialog.FileName;
            try {
                File.Delete(filePath);
            } catch (IOException) {
                MessageBox.Show("This file is opened!", "Warning!");
                return;
            }
            WorkWithExcel excelWork = new();
            if (excelWork.Open(filePath)) {
                excelWork.SetData("A", 1, "Channel");
                excelWork.SetData("A", 2, "Mark:");
                excelWork.SetData("B", 2, MarkComboBox.Text);
                
                excelWork.SetData("A", 3, "Geometric parameters:");
                excelWork.SetData("A", 4, "Height");
                excelWork.SetData("B", 4, HeightTextBox.Text);
                excelWork.SetData("A", 5, "Length");
                excelWork.SetData("B", 5, LengthTextBox.Text);
                excelWork.SetData("A", 6, "Width");
                excelWork.SetData("B", 6, WidthTextBox.Text);
                
                excelWork.SetData("A", 7, "Cover:");
                excelWork.SetData("A", 8, "Velocity");
                excelWork.SetData("B", 8, CoverVelocityTextBox.Text);
                excelWork.SetData("A", 9, "Temperature");
                excelWork.SetData("B", 9, CoverTemperatureTextBox.Text);
                
                excelWork.SetData("D", 1, "Material");
                excelWork.SetData("D", 2, "Mark:");
                excelWork.SetData("E", 2, MarkComboBox.Text);
                
                excelWork.SetData("D", 3, "Density");
                excelWork.SetData("E", 3, DensityTextBox.Text);
                excelWork.SetData("D", 4, "Specific heat");
                excelWork.SetData("E", 4, SpecificHeartTextBox.Text);
                excelWork.SetData("D", 5, "Melting point");
                excelWork.SetData("E", 5, MeltingPointTextBox.Text);
                
                excelWork.SetData("D", 6, "Empiric Coefficients:");

                
                var listOfChannelLength = _calculation.ListOfChannelLength();
                var listOfTemperatures = _calculation.ListOfTemperatures(listOfChannelLength);
                var listOfViscosity = _calculation.ListOfViscosity(listOfTemperatures);
                excelWork.SetData("G", 1, "Coordinates");
                excelWork.SetData("H", 1, "Temperature");
                excelWork.SetData("I", 1, "Viscosity");
                for (int i = 0; i < listOfChannelLength.Count; i++)
                {
                    excelWork.SetData("G", i + 1, Math.Round(listOfChannelLength[i]).ToString());
                    excelWork.SetData("H", i + 1, Math.Round(listOfTemperatures[i]).ToString());
                    excelWork.SetData("I", i + 1, Math.Round(listOfViscosity[i]).ToString());
                    
                }
                //excelWork.DrawInExcel(witchOfAgnesi.pairs.Count);

                excelWork.Save();
            }

            MessageBox.Show("Excel was successfully saved!", "Saving!");
        } else {
            MessageBox.Show("File was not saved!", "Warning!");
        }
    }

    // private void WpfPlot1_OnMouseMove(object sender, MouseEventArgs e)
    // {
    //     _charts.plot_MouseMove(sender, e);
    // }
    private void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // var type = TypeComboBox.SelectedItem.ToString();
        // var idType = _dataService.MaterialDataBase.GetIdMaterial(type);
        // _empiricCoefficients = _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficients(idType).Result;
        // EmpiricCoefficientsDataGrid.ItemsSource = _empiricCoefficients;
    }
}