﻿using MyVet.Common.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Prism.ViewModels
{
    public class HistoryItemViewModel : HistoryResponse
    {
        private readonly INavigationService _navigationService;

        private DelegateCommand _selectHistoryCommand;

        public HistoryItemViewModel(INavigationService navigationService) 
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectHistoryCommand => _selectHistoryCommand ?? (_selectHistoryCommand = new DelegateCommand(SelectHistory));

        private async void SelectHistory()
        {

            var parameter = new NavigationParameters() 
            {
                { "History", this }
            };
           
            await _navigationService.NavigateAsync("HistoryPage", parameter);
            
        }
    }
}
