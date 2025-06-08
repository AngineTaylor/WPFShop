using System.Windows;
using System.Windows.Controls;
using Shop.Services;
using Shop.ViewModels;

namespace Shop.Views
{
    public partial class CatalogView : UserControl
    {
        public CatalogView()
        {
            InitializeComponent();
            this.Loaded += CatalogView_Loaded;
        }

        private void CatalogView_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"DataContext: {this.DataContext?.GetType().Name}");
        }
    }

}

