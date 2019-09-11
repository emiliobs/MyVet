using MyVet.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyVet.Common.Services
{
    public interface IAPIService
    {
       // Task<Response> GetOwnerByEmail(string urlBase,string servicePrefix,string controller,string tokenType,string email);
        Task<Response> GetTokenAsync(string urlBase,string servicePrefix,string controller, TokenRequest tokenRequest);
    }
}
