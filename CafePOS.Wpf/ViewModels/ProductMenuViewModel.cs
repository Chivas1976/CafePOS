// CafePOS.Wpf/ViewModels/ProductMenuViewModel.cs
using System;
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Wpf.Commands;
using CafePOS.Wpf.Services;

namespace CafePOS.Wpf.ViewModels;

public class ProductMenuViewModel : SpeisekarteViewModel, INavigableVM
{
    private readonly NavigationService _nav;
    private readonly Benutzer _benutzer;              // << sačuvamo korisnika
    private readonly bool _zurueckZuVerwaltung;

    public ICommand ZurueckCmd { get; }

    // jednostavan filter: null = svi; "K" = knödel; "G" = getränke
    private string? _kat;
    public string? KategorieFilter
    {
        get => _kat;
        set
        {
            _kat = value;
            OnPropertyChanged(nameof(KategorieFilter));
            ArtikelView.Refresh();
        }
    }

    public ProductMenuViewModel(NavigationService nav, Benutzer benutzer, bool zurueckZuVerwaltung = false)
        : base(benutzer)
    {
        _nav = nav;
        _benutzer = benutzer;                          // << VAŽNO
        _zurueckZuVerwaltung = zurueckZuVerwaltung;    // << VAŽNO

        ZurueckCmd = new RelayCommand(Zurueck);

        // nadoveži se na postojeći filter pa dodaj kategoriju po ID-opsegu:
        var baseFilter = ArtikelView.Filter;
        ArtikelView.Filter = obj =>
        {
            if (baseFilter != null && !baseFilter(obj)) return false;
            if (obj is not Artikel a) return false;

            // Ako nema kategorija u modelu, koristimo dogovor:
            // ID < 100 = Knödel, ID >= 100 = Getränke
            return KategorieFilter switch
            {
                "K" => a.Id < 100,
                "G" => a.Id >= 100,
                _ => true
            };
        };
    }

    private void Zurueck()
    {
        if (_zurueckZuVerwaltung)
            _nav.Navigate(new OrderManagementViewModel(_nav, _benutzer)); // nazad na listu belega
        else
            _nav.Navigate(new CustomerStartViewModel(_nav));
    }
}