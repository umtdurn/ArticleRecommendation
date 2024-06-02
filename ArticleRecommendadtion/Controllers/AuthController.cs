using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArticleRecommendadtion.Models.VMs;
using ArticleRecommendadtion.Models;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace ArticleRecommendadtion.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMongoDbService _mongoService;

        public AuthController(IMongoDbService service)
        {
            _mongoService = service;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var init = new LoginVM();
            return View("Login",init);
        }

        public async Task<IActionResult> ManageUser()
        {
            string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
            User dbuser = await _mongoService.GetUserByEmailAsync<User>("Users", loggedUserMail);

            SignUpVM init = new SignUpVM()
            {
                Email = dbuser.Email,
                FirstName = dbuser.FirstName,
                LastName = dbuser.LastName,
                Interests = dbuser.Interests
            };

            return View(init);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUser([FromBody] SignUpVM user)
        {
            string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
            User dbuser = await _mongoService.GetUserByEmailAsync<User>("Users", loggedUserMail);
            string oldMail = dbuser.Email;
            dbuser.Email = user.Email;
            dbuser.FirstName = user.FirstName;
            dbuser.LastName = user.LastName;
            dbuser.Interests = user.Interests;
            dbuser.Password = user.Password;
            await _mongoService.UpdateUserAsync("Users", dbuser, oldMail);

            return Json(new { redirectToUrl = Url.Action("Index", "Home") });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM user)
        {
            if(user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            User dbuser = await _mongoService.GetUserByEmailAsync<User>("Users", user.Email);

            if(dbuser is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!dbuser.Password.Equals(user.Password))
            {
                return RedirectToAction("Login", "Auth");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync
                (
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public IActionResult SignUp([FromBody] SignUpVM user)
        {
            _mongoService.AddDocumentAsync<SignUpVM>("Users", user);
            return Json(new { redirectToUrl = Url.Action("Login", "Auth") });
        }
    }
}

