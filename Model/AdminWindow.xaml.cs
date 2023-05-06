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
    private string[] _marks, _types, _empiricCoefficientsNames, _units;
    private EmpiricCoefficientsModel[] _empiricCoefficients;
    private string _TypeMaterial = "";

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
        if (TypeComboBox.Items.Count == 0 || TypeComboBox.SelectedIndex == -1)
        {
            AddEmpiricCoefficientGrid.Visibility = Visibility.Hidden;
            return;
        }
            
        _TypeMaterial = TypeComboBox.SelectedItem.ToString();
        var idType = _dataService.MaterialDataBase.GetIdParameterSet(_TypeMaterial);
        EmpiricCoefficientsDataGrid.Items.Clear();
        _empiricCoefficients = _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficients(idType).Result;
        foreach (var empiricCoefficient in _empiricCoefficients)
            EmpiricCoefficientsDataGrid.Items.Add(new EmpiricCoefficientsToDataGrid(
                empiricCoefficient.IdEc,
                empiricCoefficient.Name,
                empiricCoefficient.Symbol,
                empiricCoefficient.Unit ?? " ",
                empiricCoefficient.Value));
        AddEmpiricCoefficientGrid.Visibility = Visibility.Visible;
        
        //TODO: добавить из бд 3 свойства материала
    }

    private void GetDataFromDataBase()
    {
        Clear();
        _dataService = new DataService();
        _marks = _dataService.ChannelDataBase.GetMarks();
        foreach (var mark in _marks)
            MarkComboBox.Items.Add(mark);
        MarkComboBox.SelectedIndex = -1;
        _types = _dataService.MaterialDataBase.GetTypes();
        foreach (var type in _types)
            TypeComboBox.Items.Add(type);
        TypeComboBox.SelectedItem = (_TypeMaterial is null) ? "" : _TypeMaterial;
        _empiricCoefficientsNames = _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficients();
        foreach (var ec in _empiricCoefficientsNames)
            NameComboBox.Items.Add(ec);
        NameComboBox.SelectedIndex = -1;
        _units = _dataService.EmpiricCoefficientsDataBase.GetUnits();
        foreach (var unit in _units)
        {
            UnitComboBox.Items.Add(unit);
        }
        UnitComboBox.SelectedIndex = -1;
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
        if (MarkComboBox.SelectedIndex == -1) 
            return;
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

    private void SetParameters (bool isReadOnly)
    {
        SymbolTextBox.IsEnabled = !isReadOnly;
        UnitComboBox.IsEnabled = !isReadOnly;
        ValueTextBox.IsEnabled = !isReadOnly;

    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (NameTextBox.Text.Length > 0)
        {
            SetParameters(isReadOnly: false);
            CreateEcButton.IsEnabled = true;
        }
        else
        {
            SetParameters(isReadOnly: true);
            CreateEcButton.IsEnabled = false;
        }
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

    private void TypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (TypeTextBox.Text.Length > 0)
            CreateTypeButton.IsEnabled = true;
        else CreateTypeButton.IsEnabled = false;
    }

    private void NameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NameComboBox.SelectedIndex is -1 || TypeComboBox.SelectedIndex == -1)
        {
            AddEcButton.IsEnabled = false;
            return;
        } 
        var idMaterial = _dataService.MaterialDataBase.GetIdParameterSet(TypeComboBox.SelectedItem.ToString());
        var ec =
            _dataService.EmpiricCoefficientsDataBase.GetEmpiricCoefficient(idMaterial, NameComboBox.SelectedItem.ToString()).Result;
        SymbolTextBox.Text = ec.Symbol;
        UnitComboBox.SelectedItem = ec.Unit;
        ValueTextBox.Text = ec.Value.ToString(CultureInfo.InvariantCulture);
        SetParameters(isReadOnly: true);
        AddEcButton.IsEnabled = true;

    }

    private void CreateECButton_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        var unit = UnitTextBox.Text is "" ? UnitComboBox.SelectedItem.ToString() : UnitTextBox.Text;
        var id = _dataService.MaterialDataBase.GetIdParameterSet(TypeComboBox.SelectedItem.ToString()); 
        _dataService.EmpiricCoefficientsDataBase.AddEmpiricCoefficients(
             id,
             new EmpiricCoefficientsModel(
                 id,
                 _empiricCoefficients.Length,
                 name,
                 unit,
                 Convert.ToDouble(ValueTextBox.Text),
                 SymbolTextBox.Text
                 )
        );
        GetDataFromDataBase();
    }


    private async void AddEcButton_Click(object sender, RoutedEventArgs e)
    {
        var name = NameComboBox.SelectedItem.ToString();
        var unit = UnitComboBox.SelectedItem.ToString();
        var idUnit = await _dataService.GetIdUnit(unit);
        var idParameterSet = _dataService.MaterialDataBase.GetIdParameterSet(_TypeMaterial);
        var idParameter = await _dataService.GetIdParameter(name);
        _dataService.AddParameterInParameterSet(
            idParameterSet,
            idParameter,
            idUnit,
            Convert.ToDouble(ValueTextBox.Text)
         );
        GetDataFromDataBase();
    }

}