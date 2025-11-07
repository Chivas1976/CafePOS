// CafePOS.Wpf/App.xaml.cs
using System.Windows;
using CafePOS.Wpf.Services;
using CafePOS.Wpf.ViewModels;
using CafePOS.Wpf.Views;

namespace CafePOS.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var nav = new NavigationService();                      //jedna zajednicka instanca
            var mainMenu = new MainMenuViewModel(nav);              // koristi je MainMenu
            var shellVm = new ShellViewModel(nav, mainMenu);        // i Shell

            var shell = new ShellWindow { DataContext = shellVm };
            shell.Show();
        }
    }
}