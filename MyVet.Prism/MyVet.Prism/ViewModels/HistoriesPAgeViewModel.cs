using MyVet.Common.Helpers;
using MyVet.Common.Models;
using Newtonsoft.Json;
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
        private readonly INavigationService _navigationService;
        private PetResponse _pet;
        private ObservableCollection<HistoryItemViewModel> _histories;

        public HistoriesPAgeViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Histories";
           _navigationService = navigationService;

            Pet = JsonConvert.DeserializeObject<PetResponse>(Setting.Pet);
            LoadHistories();
        }


        public PetResponse Pet
        {
            get { return _pet; }
            set { _pet = value; }
        }

        public ObservableCollection<HistoryItemViewModel> Histories 
        {
            get => _histories;
            set
            {
                SetProperty(ref _histories, value);
            }
        }

      

        private void LoadHistories()
        {
            Histories = new ObservableCollection<HistoryItemViewModel>(Pet.Histories.Select(h => new HistoryItemViewModel(_navigationService)
            {

                Date = h.Date,
                Description = h.Description,
                Id = h.Id,
                Remarks = h.Remarks,
                ServiceType = h.ServiceType

            }).ToList());
        }
    }
}
