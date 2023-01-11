using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CookieAuthenticationExample.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!ModelState.IsValid) return View();

            //verify the user credential
            if(username == "admin" && password == "123")
            {
                //if the user provide correct username and pwd
                //create the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, "admin@google.com"),
                    //new Claim("Department", "HR") //cannot access private page for now
                };

                var identity = new ClaimsIdentity(claims, "MyCookie");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                //Use the SignIn method in HttpContext, for now please follow the syntax
                await HttpContext.SignInAsync("MyCookie", principal);
                return Redirect("/Home/Index");
            }
            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
           return Redirect("/Account/Login");
        }


    }
}
