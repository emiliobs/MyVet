using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
     public interface ICombosHelpers
    {
        IEnumerable<SelectListItem> GetComboPetTypes();

        IEnumerable<SelectListItem> GetComboServiceTypes();
    }
}
