using MyVet.Common.Models;
using System.Threading.Tasks;

namespace MyVet.Common.Services
{
    public interface IAPIService
    {
        Task<Response> GetOwnerByEmail(
                 string urlBase,
                 string servicePrefix,
                 string controller,
                 string tokenType,
                 string accessToken,
                 string email);

        //  Task<Response> GetOwnerByEmailAsyn(string urlBase,string servicePrefix,string controller, string email);
        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest tokenRequest);
    }
}
