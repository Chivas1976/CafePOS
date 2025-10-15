// CafePOS.Wpf/ViewModels/MainMenuViewModel.cs
using System.Windows;
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Wpf.Commands;
using CafePOS.Wpf.Services;
using CafePOS.Wpf.Views;

namespace CafePOS.Wpf.ViewModels;

public class MainMenuViewModel : ObservableObject, INavigableVM 
{
    private readonly NavigationService _nav;
    public ICommand ChefCmd { get; }
    public ICommand MitarbeiterCmd { get; }
    public ICommand KundeCmd { get; }
    public ICommand BeendenCmd { get; }

    public MainMenuViewModel(NavigationService nav)
    {
        _nav = nav;
        ChefCmd = new RelayCommand(() => LoginUndVerwaltung(Rolle.Chef));
        MitarbeiterCmd = new RelayCommand(() => LoginUndVerwaltung(Rolle.Mitarbeiter));
        KundeCmd = new RelayCommand(() => _nav.Navigate(new CustomerStartViewModel(_nav)));
        BeendenCmd = new RelayCommand(() => Application.Current.Shutdown());
    }

    private void LoginUndVerwaltung(Rolle erwarteteRolle)
    {
        var lvm = new LoginViewModel();
        var dlg = new LoginWindow { DataContext = lvm };
        Benutzer? okUser = null;
      
        
        lvm.AnmeldungErfolgreich += b =>
        {
            // Chef kann als Mitarbeiter sich anmelden
            if (b.Rolle == erwarteteRolle || erwarteteRolle == Rolle.Mitarbeiter && b.Rolle == Rolle.Chef)
                okUser = b;
        };

         dlg.ShowDialog();

        if (okUser != null)
              _nav.Navigate(new OrderManagementViewModel(_nav, okUser));
            //_nav.Navigate(new ProductMenuViewModel(_nav, okUser));
    }
}