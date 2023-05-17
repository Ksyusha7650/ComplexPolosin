using System.Configuration;
using System.Windows;

namespace ModelPolosin;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class LoginWindow
{
    private readonly string _login;
    private readonly string _password;
    private int _quantityTries;
    public LoginWindow()
    {
        InitializeComponent();
        _login = ConfigurationManager.AppSettings["Login"];
        _password = ConfigurationManager.AppSettings["Password"];
        _quantityTries = 0;
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
        var password = PasswordTextBox.Password;
        if (login == _login && password == _password)
        {
            Hide();
            new AdminWindow().Show();
            Close();
        }
        else
        {
            _quantityTries++;
            var leftTries = 5 - _quantityTries;
            if (leftTries > 0)
                MessageBox.Show($"There is no such account! :c\n Left {leftTries} tries!!!");
            else
            {
                MessageBox.Show($"There is no such account! :c\n Left {leftTries} tries!!!");
                EnterAsAdminButton.IsEnabled = false;
            }
        }
    }
}