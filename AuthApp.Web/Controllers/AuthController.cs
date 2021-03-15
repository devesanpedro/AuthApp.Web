using AuthApp.Web.Models;
using AuthApp.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AuthApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService authService;

        public AuthController()
        {
            authService = new AuthService();
        }

        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.Authenticate(model);

                if (result != null)
                {
                    if (result.IsSuccessful)
                    {
                        var loggedInUser = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(result.Data));
                        Session["username"] = loggedInUser.Username;
                        Session["fullName"] = string.Format("{0} {1}", loggedInUser.FirstName, loggedInUser.LastName);

                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        ViewBag.ErrorMessage = result.Message;
                    }
                }
            }

            return View();
        }

        public ActionResult LogOut()
        {
            Session["username"] = null;
            Session["fullName"] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            var model = new RegisterModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.Register(model);

                if (result.IsSuccessful)
                {
                    ViewBag.Message = result.Message;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = result.Message;

                    return View(model);
                }
            }

            return View(model);
        }
    }
}