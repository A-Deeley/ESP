using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Caisse.Views;

/// <summary>
/// Interaction logic for CaisseUserControl.xaml
/// </summary>
public partial class CaisseUserControl : UserControl
{

    private bool _addManyState;
    private bool _removeState;

    public CaisseUserControl()
    {
        InitializeComponent();
    }


    private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            ToggleAddRemove();
            AddRemoveToggle.IsChecked = _removeState;
        }

        if (e.Key == Key.F2)
        {
            ToggleSingleMany();
            SingleMultipleToggle.IsChecked = _addManyState;
        }

        if (CUP_Input.IsFocused)
            return;

        CUP_Input.Focus();
    }

    private void SingleMultipleToggle_Click(object sender, RoutedEventArgs e) => ToggleSingleMany();

    private void AddRemoveToggle_Click(object sender, RoutedEventArgs e) => ToggleAddRemove();

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (AddRemoveToggle.IsChecked is true)
        {
            _removeState = true;
            Label_Remove.FontWeight = FontWeights.Bold;
            Label_Remove.FontSize += 2;
        }
        else
        {
            _removeState = false;
            Label_Add.FontWeight = FontWeights.Bold;
            Label_Add.FontSize += 2;
        }

        if (SingleMultipleToggle.IsChecked is null or false)
        {
            _addManyState = false;
            Label_Single.FontWeight = FontWeights.Bold;
            Label_Single.FontSize += 2;
        }
        else
        {
            _addManyState = true;
            Label_Multiple.FontWeight = FontWeights.Bold;
            Label_Multiple.FontSize += 2;
        }

        CUP_Input.Focus();
    }

    private void ToggleAddRemove()
    {
        if (!_removeState)
        {
            Label_Remove.FontWeight = FontWeights.Bold;
            Label_Remove.FontSize += 2;
            Label_Add.FontWeight = FontWeights.Normal;
            Label_Add.FontSize -= 2;
        }
        else
        {
            Label_Add.FontWeight = FontWeights.Bold;
            Label_Add.FontSize += 2;
            Label_Remove.FontWeight = FontWeights.Normal;
            Label_Remove.FontSize -= 2;
        }

        _removeState = !_removeState;
    }

    private void ToggleSingleMany()
    {
        if (_addManyState)
        {
            Label_Single.FontWeight = FontWeights.Bold;
            Label_Single.FontSize += 2;
            Label_Multiple.FontWeight = FontWeights.Normal;
            Label_Multiple.FontSize -= 2;
        }
        else
        {
            Label_Multiple.FontWeight = FontWeights.Bold;
            Label_Multiple.FontSize += 2;
            Label_Single.FontWeight = FontWeights.Normal;
            Label_Single.FontSize -= 2;
        }

        _addManyState = !_addManyState;
    }
}
