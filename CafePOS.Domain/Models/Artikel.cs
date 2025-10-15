using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CafePOS.Domain.Models;

public class Artikel : IDataErrorInfo, INotifyPropertyChanged
{
    private int _id;
    private string _name = string.Empty;
    private decimal _preis;
    private int _menge;

    public int Id { get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(); } } }
    public string Name { get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } } }
    public decimal Preis { get => _preis; set { if (_preis != value) { _preis = value; OnPropertyChanged(); } } }
    public int Menge { get => _menge; set { if (_menge != value) { _menge = value; OnPropertyChanged(); } } }

    public string Error => string.Empty;
    public string this[string columnName] => columnName switch
    {
        nameof(Name) => string.IsNullOrWhiteSpace(Name) ? "Name ist erforderlich." : string.Empty,
        nameof(Preis) => Preis <= 0 ? "Preis muss > 0 sein." : string.Empty,
        nameof(Menge) => Menge < 0 ? "Menge darf nicht negativ sein." : string.Empty,
        _ => string.Empty
    };

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}