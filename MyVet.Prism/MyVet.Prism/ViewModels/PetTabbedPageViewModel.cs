using MyVet.Common.Helpers;
using MyVet.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class PetTabbedPageViewModel : ViewModelBase
    {
        public PetTabbedPageViewModel(INavigationService navigationService):base(navigationService)
        {

            //aqui recupero los varoes de setting en toda la aplicacion:
            var pet = JsonConvert.DeserializeObject<PetResponse>(Setting.Pet);
            Title = $"Pet: { pet.Name }";
        }
    }
}
