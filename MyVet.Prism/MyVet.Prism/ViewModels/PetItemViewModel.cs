﻿
using MyVet.Common.Helpers;
using MyVet.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Prism.ViewModels
{
    public class PetItemViewModel : PetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _SelectPetCommand;

        public PetItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectPetCommand => _SelectPetCommand ?? (_SelectPetCommand = new DelegateCommand(SelectPet));

        private async void SelectPet()
        {
            //var parameter = new NavigationParameters();
            //parameter.Add("pet",this);

            //var parameter = new NavigationParameters 
            //{
            //    { "pet", this }
            //};
            Setting.Pet = JsonConvert.SerializeObject(this); 

            //await _navigationService.NavigateAsync("PetPage", parameter);
            //await _navigationService.NavigateAsync("HistoriesPAge", parameter);
            await _navigationService.NavigateAsync("PetTabbedPage");

        }
    }
}
