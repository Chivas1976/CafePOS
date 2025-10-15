using CafePOS.Domain.Models;
using CafePOS.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CafePOS.Wpf.Views
{
    public partial class LoginWindow : Window
    //{
    //private LoginViewModel _vm;
    //private bool _loginSuccessful = true;

    //public LoginWindow()
    //        {
    //            InitializeComponent();
    //            _vm = new LoginViewModel();
    //            DataContext = _vm;                  // nur DataContext im CTOR


    //            _vm.AnmeldungErfolgreich += benutzer =>
    //            {
    //                //  DialogResult = true;
    //                //  _loginSuccessful = true;

    //                DialogResult = true;
    //                  Close(); // Bez DialogResult direktno

    //            };

    //            _vm.CloseRequested += () =>
    //            {
    //                //DialogResult = false;
    //                //_loginSuccessful = false;
    //                       Close(); // Bez DialogResult direktno
    //;
    //            };

    //            LoginBox.Focus();

    //        }


    //        /*
    //        private void Window_Loaded(object sender, RoutedEventArgs e)
    //        {
    //            _vm.AnmeldungErfolgreich += benutzer =>
    //            {
    //                _loginSuccessful = true;
    //                Close(); // Bez DialogResult direktno
    //            };

    //            _vm.CloseRequested += () =>
    //            {
    //                _loginSuccessful = false;
    //                Close(); // Bez DialogResult direktno
    //            };

    //            LoginBox.Focus();
    //        }*/

    //        private void PwBox_PasswordChanged(object sender, RoutedEventArgs e)
    //        {
    //            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
    //                vm.Passwort = pb.Password;
    //        }
    //        public bool LoginSuccessful => _loginSuccessful;

    //        // Opcionalno: Dodaj property za dobijanje korisnika
    //        public Benutzer? AngemeldeterBenutzer => _vm.AngemeldeterBenutzer;


    //    }
    //}
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.AnmeldungErfolgreich += _ => Close();  // zatvori kad je login OK
                vm.CloseRequested += Close;               // zatvori kad korisnik odustane
            }

            LoginBox?.Focus();
        }

        private void PwBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Passwort = pb.Password;
        }
    }
}