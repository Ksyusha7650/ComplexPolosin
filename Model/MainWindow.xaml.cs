using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
    }
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        TableWindow tableWindow = new();
        tableWindow.Show();
    }
}