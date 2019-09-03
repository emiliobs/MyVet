using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelpers _combosHelpers;

        public ConverterHelper(DataContext context, ICombosHelpers combosHelpers)
        {
            _context = context;
            _combosHelpers = combosHelpers;
        }

        public async Task<History> ToHistoryAsync(HistoryViewModel historyViewModel, bool isNew)
        {
            return new History
            {
                Date = historyViewModel.Date.ToUniversalTime(),
                Description = historyViewModel.Description,
                Id = isNew ? 0 : historyViewModel.Id,
                Pet = await _context.Pets.FindAsync(historyViewModel.PetId),
                Remarks = historyViewModel.Remarks,
                ServiceType = await _context.ServiceTypes.FindAsync(historyViewModel.ServiceTypeId),
            };
        }

        public HistoryViewModel ToHistoryViewModel(History history)
        {
            return new HistoryViewModel 
            {
                Date = history.Date,
                Description = history.Description,
                Id = history.Id,
                PetId = history.Pet.Id,
                Remarks = history.Remarks,
                ServiceTypeId = history.ServiceType.Id,
                ServiceTypes = _combosHelpers.GetComboServiceTypes()
            };
        }

        public async Task<Pet> ToPetAsync(PetViewModel petViewModel, string path, bool isNew)
        {
            var pet = new  Pet()
            {
                Agendas = petViewModel.Agendas,
                Born = petViewModel.Born,
                Histories = petViewModel.Histories,
                Id = isNew  ? 0 :  petViewModel.Id,
                ImageUrl = path,
                Name = petViewModel.Name,
                Owner = await _context.Owners.FindAsync(petViewModel.OwnerId),
                PetType = await _context.PetTypes.FindAsync(petViewModel.PetTypeId),
                Race = petViewModel.Race,
                Remarks = petViewModel.Remarks,      
            };

            

            return pet;
        }
                                                                          
        public PetViewModel ToPetViewModel(Pet pet)
        {
            return new PetViewModel
            {
                Agendas = pet.Agendas,
                Born = pet.Born,
                Histories = pet.Histories,
               
                ImageUrl = pet.ImageUrl,
                
                Name = pet.Name,
                Owner = pet.Owner,
                PetType = pet.PetType,
                Race = pet.Race,
                Remarks = pet.Remarks,
                Id = pet.Id,
                OwnerId = pet.Owner.Id,
                PetTypeId = pet.PetType.Id,
                PetTypes =  _combosHelpers.GetComboPetTypes(),
            };
        }
    }
}
