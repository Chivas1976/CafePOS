using CafePOS.Wpf.ViewModels;
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

namespace CafePOS.Wpf.Views
{
    /// <summary>
    /// Interaktionslogik für OrderManagementView.xaml
    /// </summary>
    public partial class OrderManagementView : UserControl
    {
        public OrderManagementView()
        {
            InitializeComponent();
        }

        private OrderManagementViewModel? VM => DataContext as OrderManagementViewModel;

        private void Alle_Click(object sender, RoutedEventArgs e)
        {
            //if (VM != null) VM.KategorieFilter = null;
        }

        private void Knoedel_Click(object sender, RoutedEventArgs e)
        {
         //   if (VM != null) VM.KategorieFilter = "K";
        }

        private void Getraenke_Click(object sender, RoutedEventArgs e)
        {
//if (VM != null) VM.KategorieFilter = "G";
        }

        private void Speisekarte_DoubleClick(object sender, MouseButtonEventArgs e)
        {
         //   if (VM?.InWarenkorbCmd?.CanExecute(null) == true)
           //     VM.InWarenkorbCmd.Execute(null);
        }
    }
}
