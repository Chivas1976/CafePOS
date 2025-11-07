// CafePOS.Wpf/ViewModels/OrderManagementViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Domain.Services;
using CafePOS.Wpf.Commands;
using CafePOS.Wpf.Services;
using CafePOS.Wpf.Views;


namespace CafePOS.Wpf.ViewModels;

public sealed class OrderManagementViewModel : ObservableObject, INavigableVM
{
    private readonly NavigationService _nav;
   // private readonly BelegDateiService _belegService;

    public Benutzer Benutzer { get; }

    // UI kolekcija
    public ObservableCollection<Beleg> Belege { get; } = new();

    private Beleg? _selected;
    public Beleg? Selected
    {
        get => _selected;
        set
        {
            if (_selected == value) return;
            _selected = value;
            OnPropertyChanged();
            ((RelayCommand)OpenCmd).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteCmd).RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    public bool CanDelete => Selected != null;

    // Komande
    public ICommand ZurueckCmd { get; }
    public ICommand RefreshCmd { get; }
    public ICommand OpenCmd { get; }
    public ICommand OpenFolderCmd { get; }
    public ICommand DeleteCmd { get; }
    public ICommand NeueBestellungCmd { get; }

    public OrderManagementViewModel(NavigationService nav, Benutzer benutzer)
    {
        _nav = nav;
        Benutzer = benutzer;
        //_belegService = new BelegDateiService();

        ZurueckCmd = new RelayCommand(() => _nav.Navigate(new MainMenuViewModel(_nav)));
        RefreshCmd = new RelayCommand(Load);
        OpenCmd = new RelayCommand(OpenDetails, () => Selected != null);
        OpenFolderCmd = new RelayCommand(OpenFolder);
        DeleteCmd = new RelayCommand(DeleteSelected, () => Selected != null);
        NeueBestellungCmd = new RelayCommand(() =>
            _nav.Navigate(new ProductMenuViewModel(_nav, benutzer, zurueckZuVerwaltung: true)));

        Load();
    }

    private void Load()
    {
        Belege.Clear();
        foreach (var b in BelegDateiService.LadenAlle())
            Belege.Add(b);
    }

    private void OpenDetails()
    {
        if (Selected == null) return;

        var dlg = new BelegDetailsWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = Selected
        };
        dlg.ShowDialog();
    }

    private void OpenFolder()
    {
        try
        {
            var folder = BelegDateiService.OrdnerPfad();
            Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ordner kann nicht geöffnet werden.\n" + ex.Message,
                "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteSelected()
    {
        if (Selected == null) return;
        if (MessageBox.Show("Beleg wirklich löschen?", "Löschen",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            // Ako tvoj BelegDateiService ima Delete(BELEG) ili DeleteByDate(DateTime)
            BelegDateiService.Loeschen(Selected);
            // Ukloni iz liste
            var toRemove = Selected;
            Selected = null;
            var hit = Belege.FirstOrDefault(b => ReferenceEquals(b, toRemove)) ??
                      Belege.FirstOrDefault(b => b.Datum == toRemove.Datum && b.Endsumme == toRemove.Endsumme);
            if (hit != null) Belege.Remove(hit);
        }
        catch (NotImplementedException)
        {
            MessageBox.Show("Löschen ist im Service noch nicht implementiert.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Beleg konnte nicht gelöscht werden.\n" + ex.Message,
                "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
