using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SGI
{
    /// <summary>
    /// Interaction logic for WebDisplayer.xaml
    /// </summary>
    public partial class WebDisplayer : Window
    {
        private string _url;

        public WebDisplayer(string url)
        {
            _url = $"https://{url}";
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            while (WebViewControl is null || WebViewControl.CoreWebView2 is null)
            {
                await WebViewControl.EnsureCoreWebView2Async();
            }

            WebViewControl.CoreWebView2.Navigate(_url);
        }
    }
}
