using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public interface IEmailHelper
    {
        void SendEmail(string to, string subject, string body);
    }
}
