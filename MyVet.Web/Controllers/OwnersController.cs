using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OwnersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelpers _combosHelpers;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public OwnersController(DataContext context, IUserHelper userHelper, ICombosHelpers combosHelpers, 
            IConverterHelper converterHelper, IImageHelper imageHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelpers = combosHelpers;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Owners
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Owners.Include(o => o.User).Include(o => o.Pets));
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.Include(o => o.User)
                                             .Include(o => o.Pets)
                                             .ThenInclude(p => p.PetType)
                                             .Include(o => o.Pets)
                                             .ThenInclude(p => p.Histories)
                                             .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (owner.Equals(null))
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Address = model.Address,
                    Document = model.Document,
                    Email = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username,
                };

                var response = await _userHelper.AddUserAsync(user, model.Password);

                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDB, "Customer");

                    var owner = new Owner
                    {
                        Agendas = new List<Agenda>(),
                        Pets = new List<Pet>(),
                        User = userInDB,
                    };

                    _context.Owners.Add(owner);

                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError(string.Empty, ex.Message.ToString());
                        return View(model);

                    }

                }

                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }

            return View(model);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddPet(int? id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id.Value);

            if (owner.Equals(null))
            {
                return NotFound();
            }

            var model = new PetViewModel() 
            {
               Born = DateTime.Now.ToUniversalTime(),
               OwnerId = owner.Id,
               PetTypes = _combosHelpers.GetComboPetTypes(),
            };

            return View(model);
        }

       [HttpPost]
        public async Task<IActionResult> AddPet(PetViewModel petViewModel)
        {

            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (petViewModel.ImageFile != null)
                {

                    //het get the path form image:
                    path = await _imageHelper.UploadImageAsync(petViewModel.ImageFile); 
                   
                }

                var pet = await _converterHelper.ToPetAsync(petViewModel, path, true);              
                _context.Pets.Add(pet);                     

                try
                {

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message.ToString());
                    return View(petViewModel);
                }

                return this.RedirectToAction("Details", "Owners", new { id = petViewModel.OwnerId });
            }

            return View(petViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditPet(int? id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var pet = await _context.Pets.Include(p => p.Owner).Include(p => p.PetType).FirstOrDefaultAsync(p => p.Id .Equals(id));

            if (pet.Equals(null))
            {
                return NotFound();
            }
      
            return View(_converterHelper.ToPetViewModel(pet));
        }

        [HttpPost]
        public async Task<IActionResult> EditPet(int id,PetViewModel petViewModel)
        {
            if (id !=  petViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var path = petViewModel.ImageUrl;

                if (petViewModel.ImageFile != null)
                {

                    //het get the path form image:
                    path = await _imageHelper.UploadImageAsync(petViewModel.ImageFile);

                }

                var pet = await _converterHelper.ToPetAsync(petViewModel, path, false);

                try
                {
                   _context.Pets.Update(pet);

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message.ToString());
                    return View(petViewModel);
                }

               
                return this.RedirectToAction("Details", "Owners", new { id = petViewModel.OwnerId });
            }


            //petViewModel.PetType = _combosHelpers.GetComboPetTypes();

            return View(petViewModel);
        }




        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
                     


   
}
