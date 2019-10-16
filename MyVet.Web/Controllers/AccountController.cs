using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using MyVet.Web.Models;

namespace MyVet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        public AccountController(IUserHelper userHelper, IConfiguration configuration, DataContext dataContext)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Login()
        {       
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index","Home");
                }

                ModelState.AddModelError(string.Empty, "User or Password not valid.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult>  Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index","Home");
        }

         [HttpPost]
         public async Task<IActionResult> CreateToken([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(loginViewModel.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, loginViewModel.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[] 
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"], 
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMonths(5), 
                            signingCredentials: credentials);

                        var results = new 
                        {
                             token = new JwtSecurityTokenHandler().WriteToken(token),
                             expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);

                    }
                }
            }

            return BadRequest();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Register(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUserAsync(view);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    return View(view);
                }

                var owner = new Owner
                {
                    Pets = new List<Pet>(),
                    User = user,
                };

                _dataContext.Owners.Add(owner);
                await _dataContext.SaveChangesAsync();

                var loginViewModel = new LoginViewModel 
                {
                   Password = view.Password,
                   RememberMe = false,
                   Username = view.Username,
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index","Home");
                }
            }

            return View(view);
        }

        private async Task<User> AddUserAsync(AddUserViewModel view)
        {
            var user = new User
            {
                Address = view.Address,
                Document = view.Document,
                Email = view.Username,
                FirstName = view.FirstName,
                LastName = view.LastName,
                PhoneNumber = view.PhoneNumber,
                UserName = view.Username,
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);

            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(view.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Customer");
            
            return newUser;
        }
    }
}