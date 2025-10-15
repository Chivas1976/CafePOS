// CafePOS.Wpf/ViewModels/ObservableObject.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CafePOS.Wpf.ViewModels;

/// <summary>
/// Basisklasse für ViewModels (PropertyChanged-Benachrichtigung).
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>Hilfsmethode zum Setzen eines Feldes mit Change-Event.</summary>
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}