using System.Windows;
using System.Windows.Controls;

namespace SGI.Views;

/// <summary>
/// Interaction logic for SGIControl.xaml
/// </summary>
public partial class SGIControl : UserControl
{
    public SGIControl()
    {
        InitializeComponent();
    }

    private void Quit_Button_Click(object sender, RoutedEventArgs e)
    {
        App.Current.Shutdown();
    }
}
