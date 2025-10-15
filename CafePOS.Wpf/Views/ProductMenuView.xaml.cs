using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CafePOS.Wpf.ViewModels;

namespace CafePOS.Wpf.Views
{
    public partial class ProductMenuView : UserControl
    {
        public ProductMenuView()
        {
            InitializeComponent();
        }

        private ProductMenuViewModel? VM => DataContext as ProductMenuViewModel;

        private void Alle_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null) VM.KategorieFilter = null;
        }

        private void Knoedel_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null) VM.KategorieFilter = "K";
        }

        private void Getraenke_Click(object sender, RoutedEventArgs e)
        {
            if (VM != null) VM.KategorieFilter = "G";
        }

        private void Speisekarte_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VM?.InWarenkorbCmd?.CanExecute(null) == true)
                VM.InWarenkorbCmd.Execute(null);
        }
    }
}