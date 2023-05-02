using CrearLoginBasico.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrearLoginBasico.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Login(string? url = null)
        {
            ViewBag.Url = url;
            return View(new UsuarioModel.Login());
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioModel.Login model, string? url = null)
        {
            Validar(model);
            if (!ModelState.IsValid)
            {
                ViewBag.Url = url;
                return View(model);
            }

            if (!model.Correo!.Equals("prueba@mail.com") || !model.Clave!.Equals("123"))
            {
                ModelState.AddModelError("_", "El correo y/o la clave son incorrectos.");
                ViewBag.Url = url;
                return View(model);
            }

            Claim[] datos = new Claim[] { new Claim(ClaimTypes.Name, model.Correo!) };
            ClaimsIdentity identidad = new(datos, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties propiedades = new()
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = !model.MantenerActivo ? DateTimeOffset.UtcNow.AddMinutes(5) : DateTimeOffset.UtcNow.AddDays(1),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad), propiedades);

            return Redirect(url ?? "/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        #region Metodos

        [NonAction]
        private void Validar(UsuarioModel.Login entidad)
        {
            if (string.IsNullOrEmpty(entidad.Correo)) ModelState.AddModelError("Correo", "Es necesario ingresar un correo.");
            if (string.IsNullOrEmpty(entidad.Clave)) ModelState.AddModelError("Clave", "Es necesario ingresar una clave.");
        }

        #endregion
    }
}
