using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;

        public LoginPageViewModel(INavigationService navigationService): base(navigationService)
        {
            Title = "Login";

            IsEnabled = true;
        }

        public string Email { get; set; }
        public string Password 
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsRunning 
        {
            get => _isRunning; 
            set => SetProperty(ref _isRunning, value); 
       }

        public bool IsEnabled 
        { 
            get => _isEnabled; 
            set => SetProperty(ref _isEnabled, value); 
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        //public DelegateCommand LoginCommand
        //{
        //    get 
        //    {
        //        if (_loginCommand == null)
        //        {
        //            _loginCommand = new DelegateCommand(Login);
        //        }

        //        return _loginCommand;
        //    } 
        //}

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You Must enter an Email", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ypu must enter a Password","Accept");
                return;
            }

            await App.Current.MainPage.DisplayAlert("OK","Fuck Yeahhhh!!","Accept");
        }
    }
}
