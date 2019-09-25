using MyVet.Common.Models;
using MyVet.Common.Services;
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
        private readonly INavigationService _navigationService;
        private readonly IAPIService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;

        public LoginPageViewModel(
            INavigationService navigationService,
            IAPIService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Login";
            IsEnabled = true;

            //TODO: Delete this lines
            Email = "jzuluaga55@hotmail.com";
            Password = "123456";
            //Email = "emilio@gmail.com";
            //Password = "123456";
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

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

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password.", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            // var url = App.Current.Resources["UrlAPI"].ToString();
            //  var connection = await _apiService.CheckConnection(url);
            //if (!connection)
            //{
            //    IsEnabled = true;
            //    IsRunning = false;
            //    await App.Current.MainPage.DisplayAlert("Error", "Check the internet connection.", "Accept");
            //    return;
            //}

            var connection = await _apiService.CheckConnection("https://myveterinary.azurewebsites.net/");


            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;

                await App.Current.MainPage.DisplayAlert("Error","Check the internet connection.", "Accept");
                return;

            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync("https://myveterinary.azurewebsites.net/", "Account", "/CreateToken", request);
           // var response = await _apiService.GetTokenAsync("https://appmyvet.azurewebsites.net/", "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Email or password incorrect.", "Accept");
                Password = string.Empty;
                return;
            }

            var token = response.Result;
            var response2 = await _apiService.GetOwnerByEmail("https://myveterinary.azurewebsites.net/", "api", "/Owners/GetOwnerByEmail", "bearer", token.Token, Email);
            //var response2 = await _apiService.GetOwnerByEmail("https://appmyvet.azurewebsites.net/", "api", "/Owners/GetOwnerByEmail", "bearer", token.Token, Email);
            if (!response2.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "This user have a big problem, call support.", "Accept");
                return;
            }

            var owner = response2.Result;
            var parameters = new NavigationParameters
            {
                { "owner", owner }
            };

            IsRunning = false;
            IsEnabled = true;

            await _navigationService.NavigateAsync("PetsPage", parameters);
            Password = string.Empty;
        }
    }
}
