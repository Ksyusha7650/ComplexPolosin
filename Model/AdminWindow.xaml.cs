using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Algorithm;
using Algorithm.Models;
using Database.Models;
using Microsoft.Win32;
using ModelPolosin.Models;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class AdminWindow
{
    // private DataService _dataService;
    private readonly List<string> _incorrectValues = new();

    public Color borderColor = new()
    {
        A = 100
    };

    public PerformanceCounter
        myCounter = new("Processor", "% Processor Time", "_Total"); // фиг знает, что мы должны отображать

    private readonly DispatcherTimer Timer99 = new();

    public AdminWindow()
    {
        InitializeComponent();
        var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        // GetDataFromDataBase();
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
            textBox.BorderBrush = new SolidColorBrush(borderColor);
            if (exist)
                _incorrectValues.Remove(textBox.Name);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        new LoginWindow().Show();
        Close();
    }
}