using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;

        public ConverterHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<Pet> ToPetAsync(PetViewModel petViewModel, string path)
        {
            return new Pet()
            {
                Agendas = petViewModel.Agendas,
                Born = petViewModel.Born,
                Histories = petViewModel.Histories,
                //Id = petViewModel.Id,
                ImageUrl = path,
                Name = petViewModel.Name,
                Owner = await _context.Owners.FindAsync(petViewModel.OwnerId),
                PetType = await _context.PetTypes.FindAsync(petViewModel.PetTypeId),
                Race = petViewModel.Race,
                Remarks = petViewModel.Remarks,



            };
        }
    }
}
