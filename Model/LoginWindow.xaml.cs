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
        {
            MessageBox.Show("There is no such account! :c");
        }
    }
}