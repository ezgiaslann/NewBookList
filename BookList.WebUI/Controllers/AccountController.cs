using BookList.WebUI.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookList.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookList.Business.Abstract;

namespace BookList.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ICartService _cartService;

        public AccountController(ICartService cartService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _cartService = cartService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                //Generate Token
                //Send Mail
                return RedirectToAction("login", "account");
            }


            ModelState.AddModelError("", "Bilinmeyen hata oluştu lütfen tekrar deneyiniz.");
            return View(model);
        }


        //Login
        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                ModelState.AddModelError("", "Bu E-mail ile daha önce hesap oluşturulmamış.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if(result.Succeeded)
            {
                //Create Cart
                _cartService.InitializeCart(user.Id);

                return Redirect(model.ReturnUrl ?? "~/");
            }

            ModelState.AddModelError("", "E-mail veya parola yanlış.");
            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //TempData.Put("message", new ResultMessage()
            //{
            //    Title = "Oturum Kapatıldı.",
            //    Message = "Hesabınız güvenli bir şekilde sonlandırıldı.",
            //    Css = "warning"
            //});
            return Redirect("~/");
        }

        //public async Task<IActionResult> ConfirmEmail(string Id)
        //{
        //    return View()
        //}

        public IActionResult Accessdenied()
        {
            return View();
        }
    }
}
