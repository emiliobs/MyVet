﻿using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Helpers
{
    public class CombosHelpers  : ICombosHelpers
    {
        private readonly DataContext _context;

        public CombosHelpers(DataContext context)
        {
            _context = context;
        }

        IEnumerable<SelectListItem> ICombosHelpers.GetComboPetTypes()
        {
            //please not make this way.!!! not recursive:
            //var list = new List<SelectListItem>();
            //foreach (var petType in _context.PetTypes)
            //{
            //    list.Add(new SelectListItem() 
            //    {
            //       Text = petType.Name,
            //       Value = $"{petType.Id}",
            //    });
            //}

            var list = _context.PetTypes.Select(pt => new SelectListItem()
            {
                Value = $"{pt.Id}",
                Text = pt.Name,
            }).OrderBy(pt => pt.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Pet Type.....]",
                Value = "0",
            });


            return list;
        }
    }
}
