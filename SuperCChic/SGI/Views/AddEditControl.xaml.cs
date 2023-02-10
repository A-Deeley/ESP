using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace SGI.Views
{
    /// <summary>
    /// Interaction logic for AddEditControl.xaml
    /// </summary>
    public partial class AddEditControl : UserControl
    {
        public AddEditControl()
        {
            InitializeComponent();
            DiscountInput.IsEnabled = DiscountComboBox.SelectedIndex > 0;
        }

        private void DiscountInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox discountChoices = (ComboBox)sender;

            if (DiscountInput is null)
                return;

            DiscountInput.IsEnabled = discountChoices.SelectedIndex > 0;
        }
    }
}
