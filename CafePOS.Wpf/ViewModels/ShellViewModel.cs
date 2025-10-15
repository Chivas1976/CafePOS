// CafePOS.Wpf/ViewModels/ShellViewModel.cs
using CafePOS.Wpf.Services;

namespace CafePOS.Wpf.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        public NavigationService Nav { get; }

        public ShellViewModel(NavigationService nav, INavigableVM startVm)
        {
            Nav = nav;               // <<< ista instanca koju koriste ViewModeli
            Nav.Navigate(startVm);   // prikaži početni VM
        }
    }
}