using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CafePOS.Domain.Models;

/// <summary>Einzelne Position im Warenkorb (Artikel + Menge).</summary>
public class Bestellposition : INotifyPropertyChanged
{
    private int _artikelId;
    private string _name = string.Empty;
    private decimal _einzelpreis;
    private int _menge = 1;

    public int ArtikelId
    {
        get => _artikelId;
        set { if (_artikelId != value) { _artikelId = value; OnPropertyChanged(); } }
    }

    public string Name
    {
        get => _name;
        set { if (_name != value) { _name = value; OnPropertyChanged(); } }
    }

    public decimal Einzelpreis
    {
        get => _einzelpreis;
        set { if (_einzelpreis != value) { _einzelpreis = value; OnPropertyChanged(); OnPropertyChanged(nameof(Gesamt)); } }
    }

    public int Menge
    {
        get => _menge;
        set { if (_menge != value) { _menge = value; OnPropertyChanged(); OnPropertyChanged(nameof(Gesamt)); } }
    }

    /// <summary>Einzelpreis * Menge (gerundet).</summary>
    public decimal Gesamt => Math.Round(Einzelpreis * Menge, 2);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}