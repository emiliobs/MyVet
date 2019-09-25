﻿
using MyVet.Common.Models;
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

            var parameter = new NavigationParameters 
            {
                { "pet", this }
            };

            //await _navigationService.NavigateAsync("PetPage", parameter);
            await _navigationService.NavigateAsync("HistoriesPAge", parameter);

        }
    }
}
