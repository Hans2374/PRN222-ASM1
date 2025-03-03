using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentCVSTS.Repositories.Models;
using PaymentCVSTS.Services;

namespace PaymentCVSTS.MVCWebApp.Controllers
{
    public class UserAccountsController : Controller
    {
        // kết nối Services
        private readonly UserAccountService _userAccountService;

        public UserAccountsController(UserAccountService userAccountService) => _userAccountService = userAccountService;

        public IActionResult Index()
        {
            return RedirectToAction("Login");
            //  return View();
        }

        public IActionResult Login()
        {
            return View("/Views/LoginAccount/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                var userAccount = await _userAccountService.Login(userName, password);

                if (userAccount != null)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userAccount.UserName),
                            new Claim(ClaimTypes.Role, userAccount.RoleId.ToString())
                        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    Response.Cookies.Append("UserName", userAccount.FullName);
                    Response.Cookies.Append("Role", userAccount.RoleId.ToString());

                    return RedirectToAction("Index", "Payments");
                }
            }
            catch (Exception ex)
            {

                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ModelState.AddModelError("", "Login failure");

            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "UserAccounts");
        }

        public async Task<IActionResult> Forbidden()
        {
            return View("/Views/LoginAccount/Forbiden.cshtml"); ;
        }
    }
}
