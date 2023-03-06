using Caisse.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Caisse.Views
{
    /// <summary>
    /// Interaction logic for ModalWindow.xaml
    /// </summary>
    public partial class ModalWindowYesNo : Window
    {
        public ModalWindowYesNo()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = (CaisseViewModel)DataContext;
            if (e.Key == Key.Enter)
            {
                viewModel.YesBtn.Execute(null);
                Close();
            }

            if (e.Key is Key.Delete or Key.Back or Key.Escape)
            {
                viewModel.NoBtn.Execute(null);
                Close();
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
