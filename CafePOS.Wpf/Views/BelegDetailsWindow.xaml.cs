using System.IO;
using System.Text;
using System.Globalization;
using Microsoft.Win32;
using CafePOS.Domain.Models;

using System.Windows;

namespace CafePOS.Wpf.Views
{
    public partial class BelegDetailsWindow : Window
    {
        public BelegDetailsWindow()
        {
            InitializeComponent();
            Loaded += BelegDetailsWindow_Loaded;
        }

        private void BelegDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Lep naslov sa datumom i sumom, ako je prosleđen Beleg kao DataContext
            if (DataContext is Beleg b)
            {
                Title = $"Beleg – {b.Datum:dd.MM.yyyy HH:mm}  |  Summe: {b.Summe:F2} €";
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}