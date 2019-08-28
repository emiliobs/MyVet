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
