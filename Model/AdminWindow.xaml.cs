using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Algorithm.Models;
using Database;
using Database.Models;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class AdminWindow
{
    private DataService _dataService;
    private readonly List<string> _incorrectValues = new();
    private string[] _marks, _types;
    private EmpiricCoefficientsModel[] _empiricCoefficients;

    public Color BorderColor = new()
    {
        A = 100
    };
    
    public AdminWindow()
    {
        InitializeComponent();
        var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        GetDataFromDataBase();
        SetUpColumns();
    }

    // чуть сократила 👉👈
    private bool CheckTextBox => _incorrectValues.Count == 0;

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void ZeroValidationTextBox(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var exist = false;
        foreach (var incorrectTextBox in _incorrectValues)
            if (incorrectTextBox == textBox?.Name)
            {
                exist = true;
                break;
            }

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

    private void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TypeComboBox.Items.Count == 0)
            return;
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
        // добавить из бд 3 свойства материала
    }

    private void GetDataFromDataBase()
    {
        Clear();
        _dataService = new DataService();
        _marks = _dataService.ChannelDataBase.GetMarks();
        foreach (var mark in _marks)
            MarkComboBox.Items.Add(mark);
        _types = _dataService.MaterialDataBase.GetTypes();
        foreach (var type in _types)
            TypeComboBox.Items.Add(type);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (CheckTextBox)
        {
            
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        new LoginWindow().Show();
        Close();
    }

    private void CreateMarkButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!CheckTextBox)
        {
            MessageBox.Show("Fix fields!");
            return;
        }
        var height = Convert.ToDouble(HeightTextBox.Text);
        var width = Convert.ToDouble(WidthTextBox.Text);
        var length = Convert.ToDouble(LengthTextBox.Text);
        var mark = MarkTextBox.Text;
        _dataService.ChannelDataBase.SetGeometricParameters(
            mark,
            new GeometricParametersModel(
                height,
                length,
                width)
            );
        GetDataFromDataBase();
    }
    
    private void MarkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MarkComboBox.Items.Count == 0) return;
        var mark = MarkComboBox.SelectedItem.ToString();
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
        MarkComboBox.Items.Clear();
        TypeComboBox.Items.Clear();
    }

    private void CreateTypeButton_OnClick(object sender, RoutedEventArgs e)
    {
        _dataService.MaterialDataBase.AddMaterial(
            new PropertiesOfMaterialModel(
                TypeTextBox.Text,
                Convert.ToDouble(DensityTextBox.Text),
                Convert.ToDouble(SpecificHeartTextBox.Text),
                Convert.ToDouble(MeltingPointTextBox.Text)));
        GetDataFromDataBase();
    }

    private void NameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private async void CreateECButton_OnClick(object sender, RoutedEventArgs e)
    {
        var id = _dataService.MaterialDataBase.GetIdParameterSet(TypeComboBox.Text); 
        _dataService.EmpiricCoefficientsDataBase.AddEmpiricCoefficients(
             id,
             new EmpiricCoefficientsModel(
                 id,
                 _empiricCoefficients.Length,
                 NameTextBox.Text,
                 UnitTextBox.Text,
                 Convert.ToDouble(ValueTextBox.Text),
                 SymbolTextBox.Text
                 )
        );
        GetDataFromDataBase();
    }
}