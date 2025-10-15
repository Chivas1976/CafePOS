using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CafePOS.Wpf/Services/NavigationService.cs
using CafePOS.Wpf.ViewModels;

namespace CafePOS.Wpf.Services
{
    public interface INavigableVM { }

    public class NavigationService : ObservableObject
    {
        private INavigableVM? _current;
        public INavigableVM? Current
        {
            get => _current;
            set
            {
                Set(ref _current, value);
    
            }
        }

        public void Navigate(INavigableVM vm) => Current = vm;
    }
}