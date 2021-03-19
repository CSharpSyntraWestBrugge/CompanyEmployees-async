using CompanyEmployees.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signinManager;

        public SecurityController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signinManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signinManager = signinManager;

        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register obj)
        {


            if (ModelState.IsValid)
            {
                if (!roleManager.RoleExistsAsync("Manager").Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = "Manager";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
                }

                IdentityUser user = new IdentityUser();
                user.UserName = obj.UserName;
                user.Email = obj.Email;


                IdentityResult result = userManager.CreateAsync
                (user, obj.Password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Manager").Wait();
                    return RedirectToAction("SignIn", "Security");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user details: " + result.Errors.ToString());
                }
            }
            return View(obj);
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignIn obj)
        {
          
            if (ModelState.IsValid)
            {
                var result = signinManager.PasswordSignInAsync
                (obj.UserName, obj.Password,
                    obj.RememberMe, false).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "EmployeeManager");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user details");
                }
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult SignOut()
        {
            signinManager.SignOutAsync().Wait();
            return RedirectToAction("SignIn", "Security");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
