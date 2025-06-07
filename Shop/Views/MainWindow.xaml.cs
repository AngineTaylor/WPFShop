using System.Windows;
using Shop.ViewModels;

namespace Shop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

    }
}
