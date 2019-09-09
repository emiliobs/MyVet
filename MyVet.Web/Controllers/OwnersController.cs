using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System;
using System.Collections.Generic;
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

            var owner = await _context.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id.Equals(id.Value));

            if (owner.Equals(null))
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = owner.User.Address,
                Document = owner.User.Document,
                FirstName = owner.User.FirstName,
                Id = owner.Id,
                LastName = owner.User.LastName,
                PhoneNumber = owner.User.PhoneNumber,
            };

            return View(model);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
        {


            if (ModelState.IsValid)
            {

                var owner = await _context.Owners
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id.Equals(editUserViewModel.Id));

                owner.User.Document = editUserViewModel.Document;
                owner.User.FirstName = editUserViewModel.FirstName;
                owner.User.LastName = editUserViewModel.LastName;
                owner.User.Address = editUserViewModel.Address;
                owner.User.PhoneNumber = editUserViewModel.PhoneNumber;

                try
                {
                    await _userHelper.UpdateUserAsync(owner.User);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return View(editUserViewModel);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.User)
                .Include(o => o.Pets)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            if (owner.Pets.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The owners cannot be removed.");
                return RedirectToAction(nameof(Index));

            }

            //elete the user:
            await _userHelper.DeleteUserAsyn(owner.User.Email);

            try
            {
                _context.Owners.Remove(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }


        }

        // POST: Owners/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var owner = await _context.Owners.FindAsync(id);
        //    _context.Owners.Remove(owner);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

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

            petViewModel.PetTypes = _combosHelpers.GetComboPetTypes();


            return View(petViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditPet(int? id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var pet = await _context.Pets.Include(p => p.Owner).Include(p => p.PetType).FirstOrDefaultAsync(p => p.Id.Equals(id));

            if (pet.Equals(null))
            {
                return NotFound();
            }

            return View(_converterHelper.ToPetViewModel(pet));
        }

        [HttpPost]
        public async Task<IActionResult> EditPet(int id, PetViewModel petViewModel)
        {
            if (id != petViewModel.Id)
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

                petViewModel.PetTypes = _combosHelpers.GetComboPetTypes();


                return this.RedirectToAction("Details", "Owners", new { id = petViewModel.OwnerId });
            }



            return View(petViewModel);
        }

        public async Task<IActionResult> DetailsPet(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .ThenInclude(o => o.User)
                .Include(p => p.Histories)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.Id.Equals(id));

            if (pet.Equals(null))
            {
                return NotFound();
            }

            return View(pet);
        }

        [HttpGet]
        public async Task<IActionResult> AddHistory(int? id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id.Value);
            if (pet.Equals(null))
            {
                return NotFound();
            }

            var historyViewModel = new HistoryViewModel
            {
                Date = DateTime.Now.ToUniversalTime(),
                PetId = pet.Id,
                ServiceTypes = _combosHelpers.GetComboServiceTypes(),
            };

            return View(historyViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddHistory(HistoryViewModel historyViewModel)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(historyViewModel, true);

                try
                {
                    _context.Histories.Add(history);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("DetailsPet", "Owners", new { id = historyViewModel.PetId });
                    //return RedirectToAction($"{nameof(DetailsPet)}/{historyViewModel.PetId}");
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }

            //here put combobox couse can loss:
            historyViewModel.ServiceTypes = _combosHelpers.GetComboServiceTypes();

            return View(historyViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditHistory(int? id)
        {
            if (id.Equals(null))
            {
                return BadRequest();
            }

            var history = await _context.Histories
                .Include(h => h.Pet)
                .Include(h => h.ServiceType)
                .FirstOrDefaultAsync(p => p.Id.Equals(id.Value));

            if (history.Equals(null))
            {
                return NotFound();
            }

            return View(_converterHelper.ToHistoryViewModel(history));
        }

        [HttpPost]
        public async Task<IActionResult> EditHistory(HistoryViewModel historyViewModel)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(historyViewModel, false);

                try
                {
                    _context.Histories.Update(history);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("DetailsPet", "Owners", new { id = historyViewModel.PetId });
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }

            historyViewModel.ServiceTypes = _combosHelpers.GetComboServiceTypes();

            return View(historyViewModel);
        }

        public async Task<IActionResult> DeleteHistory(int? Id)
        {
            if (Id.Equals(null))
            {
                return NotFound();
            }

            var history = await _context.Histories.Include(h => h.Pet).FirstOrDefaultAsync(h => h.Id.Equals(Id.Value));

            if (history.Equals(null))
            {
                return NotFound();
            }

            try
            {
                _context.Histories.Remove(history);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsPet", "Owners", new { id = history.Pet.Id });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }



        }

        public async Task<IActionResult> DeletePet(int? Id)
        {



            if (Id.Equals(null))
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.Histories)
                .FirstOrDefaultAsync(h => h.Id.Equals(Id.Value));

            if (pet.Equals(null))
            {
                return NotFound();
            }

            //aqui no permito borrar la mascota por que tiene historial:
            if (pet.Histories.Count > 0)
            {

                //ViewBag.ErrorDelete = "The pet can't be delete because it has related records.";

                ModelState.AddModelError(string.Empty, "The pet can't be delete because it has related records.");


                return RedirectToAction("Details", "Owners", new { id = pet.Owner.Id });

            }

            try
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Owners", new { id = pet.Owner.Id });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }




        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }




}
