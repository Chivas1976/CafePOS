// CafePOS.Wpf/ViewModels/CustomerStartViewModel.cs
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Wpf.Commands;
using CafePOS.Wpf.Services;

namespace CafePOS.Wpf.ViewModels;

public class CustomerStartViewModel : ObservableObject, INavigableVM
{
    private readonly NavigationService _nav;
    public ICommand BestellungAufgebenCmd { get; }
    public ICommand ZurueckCmd { get; }

    public CustomerStartViewModel(NavigationService nav)
    {
        _nav = nav;

        BestellungAufgebenCmd = new RelayCommand(() =>
        {
           
            var pseudo = new Benutzer { Anzeigename = "Kunde", Rolle = Rolle.Mitarbeiter };
            _nav.Navigate(new ProductMenuViewModel(_nav, pseudo));
        });

        ZurueckCmd = new RelayCommand(() => _nav.Navigate(new MainMenuViewModel(_nav)));
    }
}