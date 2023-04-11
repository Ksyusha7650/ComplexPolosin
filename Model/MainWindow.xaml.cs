using System.Windows;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        TableWindow tableWindow = new();
        tableWindow.Show();
    }
}