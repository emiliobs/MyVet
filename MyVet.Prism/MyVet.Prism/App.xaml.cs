﻿using Prism;
using Prism.Ioc;
using MyVet.Prism.ViewModels;
using MyVet.Prism.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyVet.Common.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyVet.Prism
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IAPIService, ApiService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<PetsPage, PetsPageViewModel>();     
            containerRegistry.RegisterForNavigation<PetPage, PetPageViewModel>();
            containerRegistry.RegisterForNavigation<HistoriesPAge, HistoriesPAgeViewModel>();
            containerRegistry.RegisterForNavigation<HistoryPage, HistoryPageViewModel>();
        }
    }
}
