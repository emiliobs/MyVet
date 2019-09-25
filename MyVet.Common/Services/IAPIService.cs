using MyVet.Common.Models;
using System.Threading.Tasks;

namespace MyVet.Common.Services
{
    public interface IAPIService
    {
        Task<Response<OwnerResponse>> GetOwnerByEmail(
                 string urlBase,
                 string servicePrefix,
                 string controller,
                 string tokenType,
                 string accessToken,
                 string email);

        //  Task<Response> GetOwnerByEmailAsyn(string urlBase,string servicePrefix,string controller, string email);
        Task<Response<TokenResponse>> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest tokenRequest);

        Task<bool> CheckConnection(string url);
    }
}
