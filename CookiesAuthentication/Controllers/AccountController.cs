using CookiesAuthentication.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CookiesAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountDao _accountDao;
        public AccountController(AccountDao accountDao)
        {
            _accountDao = accountDao;   
        }
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
            //if (!ModelState.IsValid) return View();

            ////verify the user credential
            //if(username == "admin" && password == "123")
            //{
            //    //if the user provide correct username and pwd
            //    //create the security context
            //    var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, username),
            //        new Claim(ClaimTypes.Email, "admin@google.com"),
            //        //new Claim("Department", "HR") //cannot access private page for now
            //    };

            //    var identity = new ClaimsIdentity(claims, "MyCookie");
            //    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            //    //Use the SignIn method in HttpContext, for now please follow the syntax
            //    await HttpContext.SignInAsync("MyCookie", principal);
            //    return Redirect("/Home/Index");
            //}
            //return View();
            if (!ModelState.IsValid) return View();
            //verify using check method
            bool isVerified = _accountDao.AuthenticationCheck(username, password);
            if(isVerified)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, password),
                };
                var identity = new ClaimsIdentity(claims, "MyCookie");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                //Use SignIn method in HttpContext. 
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
