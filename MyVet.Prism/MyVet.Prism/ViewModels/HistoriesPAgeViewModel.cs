﻿using MyVet.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class HistoriesPAgeViewModel : ViewModelBase
    {
        
        private PetResponse _pet;
        private ObservableCollection<HistoryResponse> _histories;

        public HistoriesPAgeViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Histories";
        }


        public PetResponse Pet
        {
            get { return _pet; }
            set { _pet = value; }
        }

        public ObservableCollection<HistoryResponse> Histories 
        {
            get => _histories;
            set
            {
                SetProperty(ref _histories, value);
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("pet"))
            {
                Pet = parameters.GetValue<PetResponse>("pet");
                Title = $"Histories of: {Pet.Name}";
                Histories = new ObservableCollection<HistoryResponse>(Pet.Histories);
            }

        }




    }
}
