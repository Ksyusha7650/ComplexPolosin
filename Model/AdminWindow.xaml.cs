using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Database;
using Database.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class AdminWindow
{
    private DataService _dataService;
    private readonly List<string> _incorrectValues = new();
    private string[] _marks, _types;

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
    
    private void GetDataFromDataBase()
    {
        _dataService = new DataService();
        _marks = _dataService.ChannelDataBase.GetMarks();
       // foreach (var mark in _marks)
       //     MarkComboBox.Items.Add(mark);
        _types = _dataService.MaterialDataBase.GetTypes();
       // foreach (var type in _types)
       // TypeComboBox.Items.Add(type);
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
    }
}