using System;
using System.Collections.Generic;
using System.Configuration;
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
public partial class LoginWindow
{
    string _login, _password;
    public LoginWindow()
    {
        InitializeComponent();
        _login = ConfigurationManager.AppSettings["Login"];
        _password = ConfigurationManager.AppSettings["Password"];
    }

    private void EnterAsExplorerButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        new MainWindow().Show();
        Close();
    }

    private void EnterAsAdminButton_Click(object sender, RoutedEventArgs e)
    {
        var login = LoginTextBox.Text;
        var password = PasswordTextBox.Text;
        if (login == _login && password == _password)
        {
           Hide();
           new AdminWindow().Show();
           Close();
        }
        else 
            MessageBox.Show("There is no such account! :c");
    }
}