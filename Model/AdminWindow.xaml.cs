using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Algorithm.Models;
using Database;
using Database.Models;
using Microsoft.Win32;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class AdminWindow
{
    private readonly DataService _dataService;
    private readonly List<string> _incorrectValues = new() { "HeightTextBox", "WidthTextBox", "LengthTextBox" };
    private EmpiricCoefficientsModel[] _empiricCoefficients;
    private string _TypeMaterial = "";

    private readonly Color _BorderColor = new()
    {
        A = 100
    };

    public AdminWindow()
    {
        InitializeComponent();
        var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        _dataService = new DataService();
        GetDataFromDataBase();
        SetUpColumns();
    }

    private bool CheckChannelTextBox => !_incorrectValues.Any(value => 
            value is "HeightTextBox"
                  or "LengthTextBox"
                  or "WidthTextBox");

    private bool CheckMaterialTextBox => !_incorrectValues.Any(value =>
            value is "DensityTextBox"
                  or "SpecificHeartTextBox"
                  or "MeltingPointTextBox");

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void ZeroValidationTextBox(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var exist = _incorrectValues.Any(incorrectTextBox => incorrectTextBox == textBox?.Name);
        decimal.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var number);
        if (number == 0 || textBox.Text is "")
        {
            textBox.BorderBrush = Brushes.Red;
            if (!exist)
                _incorrectValues.Add(textBox.Name);
        }
        else
        {
            textBox!.BorderBrush = new SolidColorBrush(_BorderColor);
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

    private async void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TypeComboBox.Items.Count == 0 || TypeComboBox.SelectedIndex == -1)
        {
            AddEmpiricCoefficientGrid.Visibility = Visibility.Hidden;
            EditTypeButton.IsEnabled = false;
            DeleteTypeButton.IsEnabled = false;
            return;
        }
        EditTypeButton.IsEnabled = true;
        DeleteTypeButton.IsEnabled = true;
        _TypeMaterial = TypeComboBox.SelectedItem.ToString()!;
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
        var (_, density, specificHeat, meltingPoint) =
            await _dataService.MaterialDataBase.GetMaterialProperties(idType);
        DensityTextBox.Text = density.ToString(CultureInfo.InvariantCulture);
        SpecificHeartTextBox.Text = specificHeat.ToString(CultureInfo.InvariantCulture);
        MeltingPointTextBox.Text = meltingPoint.ToString(CultureInfo.InvariantCulture);
    }

    private void GetDataFromDataBase()
    {
        Clear();
        _dataService.ChannelDataBase.GetMarks()
            .ToList()
            .ForEach(mark => MarkComboBox.Items.Add(mark));
        MarkComboBox.SelectedIndex = -1;
        _dataService.MaterialDataBase.GetTypes()
            .ToList()
            .ForEach(type => TypeComboBox.Items.Add(type));
        TypeComboBox.SelectedItem = _TypeMaterial ?? "";
        _dataService.EmpiricCoefficientsDataBase.GetNamesOfEmpiricCoefficients()
            .ToList()
            .ForEach(empiricCoefficient => NameComboBox.Items.Add(empiricCoefficient));
        NameComboBox.SelectedIndex = -1;
        _dataService.EmpiricCoefficientsDataBase.GetUnits()
            .ToList()
            .ForEach(unit => UnitComboBox.Items.Add(unit));
        UnitComboBox.SelectedIndex = -1;
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        new LoginWindow().Show();
        Close();
    }

    private void CreateMarkButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!CheckChannelTextBox)
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
        MarkTextBox.Text = "";
        MarkComboBox.SelectedItem = mark;
    }

    private void MarkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MarkComboBox.SelectedIndex == -1)
        {
            EditMarkButton.IsEnabled = false;
            DeleteMarkButton.IsEnabled = false;
            return;
        }

        EditMarkButton.IsEnabled = true;
        DeleteMarkButton.IsEnabled = true;
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
        NameComboBox.Items.Clear();
        UnitComboBox.Items.Clear();
    }

    private void SetParameters(bool isReadOnly)
    {
        SymbolTextBox.IsEnabled = !isReadOnly;
        UnitComboBox.IsEnabled = !isReadOnly;
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (NameTextBox.Text.Length > 0)
        {
            SetParameters(false);
            CreateEcButton.IsEnabled = true;
        }
        else
        {
            SetParameters(true);
            CreateEcButton.IsEnabled = false;
        }
    }

    private void CreateTypeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!CheckMaterialTextBox)
        {
            MessageBox.Show("Fix fields!");
            return;
        }

        _dataService.MaterialDataBase.AddMaterial(
            new PropertiesOfMaterialModel(
                TypeTextBox.Text,
                Convert.ToDouble(DensityTextBox.Text),
                Convert.ToDouble(SpecificHeartTextBox.Text),
                Convert.ToDouble(MeltingPointTextBox.Text)));
        GetDataFromDataBase();
        TypeTextBox.Text = "";
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox ?? new TextBox();
        Button button = new();
        switch (textBox.Name)
        {
            case "TypeTextBox":
            {
                button = CreateTypeButton;
                break;
            }
            case "MarkTextBox":
            {
                button = CreateMarkButton;
                break;
            }
            case "UnitTextBox":
            {
                button = CreateUnitButton;
                break;
            }
        }

        button.IsEnabled = textBox.Text.Length > 0;
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
            _dataService.EmpiricCoefficientsDataBase
                .GetEmpiricCoefficient(idMaterial, NameComboBox.SelectedItem.ToString()).Result;
        SymbolTextBox.Text = ec.Symbol;
        UnitComboBox.SelectedItem = ec.Unit;
        SetParameters(true);
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
        ValueTextBox.Text = "0.00";
        UnitComboBox.SelectedItem = "-";
    }

    private void CreateUnitButton_OnClick(object sender, RoutedEventArgs e)
    {
        var unit = UnitTextBox.Text;
        _dataService.AddUnit(unit); //
        UnitComboBox.Items.Add(unit);
        UnitComboBox.SelectedItem = unit;
        UnitTextBox.Text = "";
    }

    private void EditTypeButton_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckMaterialTextBox)
        {
            MessageBox.Show("Fix fields!");
            return;
        }

        _dataService.MaterialDataBase.EditMaterial(
            new PropertiesOfMaterialModel(
                TypeComboBox.SelectedItem.ToString(),
                Convert.ToDouble(DensityTextBox.Text),
                Convert.ToDouble(SpecificHeartTextBox.Text),
                Convert.ToDouble(MeltingPointTextBox.Text)));
    }

    private void DeleteTypeButton_OnClick(object sender, RoutedEventArgs e)
    {
        var type = TypeComboBox.SelectedItem.ToString();
        var result = MessageBox.Show($"{type} will be deleted!", "caption", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        switch (result)
        {
            case MessageBoxResult.Cancel:
            {
                return;
            }
            case MessageBoxResult.OK:
            {
                _dataService.MaterialDataBase.DeleteMaterial(type);
                GetDataFromDataBase();
                _TypeMaterial = null;
                TypeComboBox.SelectedIndex = -1;
                DensityTextBox.Text = "0.00";
                MeltingPointTextBox.Text = "0.00";
                SpecificHeartTextBox.Text = "0.00";
                break;
            }
        }
    }

    private void BackupButton_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Sql files (*.sql)|*.sql|All files (*.sql)|*.sql",
            FilterIndex = 2,
            RestoreDirectory = true
        };
        if (saveFileDialog.ShowDialog() != true) return;
        var filePath = saveFileDialog.FileName;
        _dataService.Backup(filePath);
        MessageBox.Show("Success!");
    }

    private void EditMarkButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!CheckChannelTextBox)
        {
            MessageBox.Show("Fix fields!");
            return;
        }
        _dataService.ChannelDataBase.EditChannel(
            new GeometricParametersModel(
                Convert.ToDouble(HeightTextBox.Text),
                Convert.ToDouble(LengthTextBox.Text),
                Convert.ToDouble(WidthTextBox.Text)),
            MarkComboBox.SelectedItem.ToString());
    }

    private void DeleteMarkButton_OnClick(object sender, RoutedEventArgs e)
    {
        var type = MarkComboBox.SelectedItem.ToString();
        var result = MessageBox.Show($"{type} will be deleted!", "caption", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        switch (result)
        {
            case MessageBoxResult.Cancel:
            {
                return;
            }
            case MessageBoxResult.OK:
            {
                _dataService.ChannelDataBase.DeleteChannel(type);
                GetDataFromDataBase();
                MarkComboBox.SelectedIndex = -1;
                HeightTextBox.Text = "0.00";
                WidthTextBox.Text = "0.00";
                LengthTextBox.Text = "0.00";
                break;
            }
        }
    }

    private void RestoreButton_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog()
        {
            Filter = "Sql files (*.sql)|*.sql|All files (*.sql)|*.sql",
            FilterIndex = 2,
            RestoreDirectory = true
        };
        if (openFileDialog.ShowDialog() != true) return;
        var filePath = openFileDialog.FileName;
        _dataService.Restore(filePath);
        GetDataFromDataBase();
        MessageBox.Show("Success!");
    }
}