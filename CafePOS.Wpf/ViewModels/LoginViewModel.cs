// CafePOS.Wpf/ViewModels/LoginViewModel.cs
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Wpf.Commands;

namespace CafePOS.Wpf.ViewModels;

/// <summary>
/// Einfaches Login-ViewModel mit In-Memory-Benutzern und Rollen.
/// </summary>
public class LoginViewModel : ObservableObject
{
    private string _loginname = "";
    public string Loginname
    {
        get => _loginname;
        set => Set(ref _loginname, value);
    }

    private string _passwort = "";
    /// <summary>Wird aus dem PasswordBox im View gesetzt.</summary>
    public string Passwort
    {
        get => _passwort;
        set => Set(ref _passwort, value);
    }

    private string _fehlermeldung = "";
    public string Fehlermeldung
    {
        get => _fehlermeldung;
        set => Set(ref _fehlermeldung, value);
    }

    public Benutzer? AngemeldeterBenutzer { get; private set; }

    // Demo-Benutzer (aus deinem alten Projekt übernommen, vereinfacht)
    private readonly List<Benutzer> _benutzer = new()
    {
        new Benutzer { Anzeigename = "Vlado Oroz", Loginname = "vlado",  Passwort = "vlado",  Rolle = Rolle.Chef },
        new Benutzer { Anzeigename = "Anna-Maria Schwast", Loginname = "anna",  Passwort = "pass123", Rolle = Rolle.Mitarbeiter },
        new Benutzer { Anzeigename = "Rafel Reich",  Loginname = "rafael", Passwort = "pass123", Rolle = Rolle.Mitarbeiter },
    };

    public ICommand AnmeldenCmd { get; }
    public ICommand AbbrechenCmd { get; }

    public LoginViewModel()
    {
        AnmeldenCmd = new RelayCommand(Anmelden);
        AbbrechenCmd = new RelayCommand(() => CloseRequested?.Invoke());
    }

    /// <summary>Event, damit das Fenster schließen kann (wird im Code-Behind gesetzt).</summary>
    public Action? CloseRequested { get; set; }

    /// <summary>Wird vom Fenster abonniert: Login erfolgreich.</summary>
    public event Action<Benutzer>? AnmeldungErfolgreich;

    private void Anmelden()
    {
        Fehlermeldung = "";

        var ben = _benutzer.FirstOrDefault(b =>
            string.Equals(b.Loginname, Loginname, StringComparison.OrdinalIgnoreCase)
            && b.Passwort == Passwort);

        if (ben is null)
        {
            Fehlermeldung = "Loginname oder Passwort ist falsch.";
            return;
        }

        AngemeldeterBenutzer = ben;
        AnmeldungErfolgreich?.Invoke(ben);
        CloseRequested?.Invoke();
    }
}