using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace demo;

public partial class ToAdmin : Window
{
    private string _code = "0000";
    public ToAdmin()
    {
        InitializeComponent();

    }

    private void LoginAdminButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CodeTextBox.Text == "0000")
        {
            new MainWindow(true).Show();
            this.Close();
        }
        else
        {
            new MainWindow(false).Show();
            this.Close();
        }
    }

}