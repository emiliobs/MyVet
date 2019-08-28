using System.Threading.Tasks;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;

namespace MyVet.Web.Helpers
{
    public interface IConverterHelper
    {
        PetViewModel ToPetViewModel(Pet pet);
        Task<Pet> ToPetAsync(PetViewModel petViewModel, string path);
    }
}