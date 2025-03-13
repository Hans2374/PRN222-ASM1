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
            // Redirect unauthenticated users to login
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Payments");
        }

        public IActionResult Login()
        {
            // If user is already authenticated, redirect to payments
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Payments");
            }

            return View("/Views/LoginAccount/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                // Clear existing authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var userAccount = await _userAccountService.Login(userName, password);

                if (userAccount != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userAccount.UserName),
                        new Claim(ClaimTypes.Role, userAccount.RoleId.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, userAccount.UserAccountId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false, // Don't use "remember me" functionality
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                        });

                    // Use cookies for non-auth-related user information only
                    Response.Cookies.Append("UserName", userAccount.FullName, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.Now.AddMinutes(30)
                    });

                    return RedirectToAction("Index", "Payments");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }
            catch (Exception ex)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ModelState.AddModelError("", "Login failure: " + ex.Message);
            }

            return View("/Views/LoginAccount/Login.cshtml");
        }

        public async Task<IActionResult> Logout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear other cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Login", "UserAccounts");
        }

        public IActionResult Forbidden()
        {
            return View("/Views/LoginAccount/Forbidden.cshtml");
        }
    }
}