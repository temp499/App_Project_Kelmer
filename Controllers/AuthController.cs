using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;

namespace App_Proyecto.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService;
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Hubo un error al Registrar el Usuario");
            var (success, error) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return Conflict(new
                {
                    message = error
                });
            }

            return Ok(new { success = true });


        }
        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["Access_token"];
            if (!string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Datos Invalidos" });

            var (success, result, error) = await _authService.LoginAsync(dto);

            if (!success || result is null)
                return Unauthorized(new { success = false, errorMessage = error ?? "Credenciales Invalidas" });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };

            Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);
            Response.Cookies.Append("RefreshToken", result.RefreshToken, cookieOptions);
            return Ok(new
            {
                success = true,
                message = "Login Exitoso"
            });

        }
        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("Refresh_Token");
            return Json(new { success = true, message = "Session cerrada Exitosamente" });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Parametros Invalidos");

            var confirmado = await _authService.ConfirmEmialAsync(email, token);
            if (confirmado)
            {

                TempData["Success"] = "Correo confirmado ya puedes iniciar session";
                return RedirectToAction("Login");
            }
            TempData["error"] = "El token es Invalido o Expiro";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequestDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { success = false, errorMessage = "El correo es requerido" });
            }
            var sent = await _authService.SendResetPasswordLinkAsync(model.Email);
            if (sent)
            {
                return Ok(new { success = true });
            }
            else
            {
                return NotFound(new { success = false, ErrorMessage = "Correo no encontrado" });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordRequestDto { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

                var success = await _authService.ResetPasswordAsync(dto);

                if (success)
                    return Ok(new { success = true });
                ModelState.AddModelError("", "TokenInvalido o Expirado");
                return View(dto);
           
             
        }
        [HttpGet]
        [HttpPost]

        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userName = User.Identity?.Name;
            var success = await _authService.ChangePasswordAsync(userName, dto);

            if (success)
            {
                TempData["Success"] = "Contraseña actualizada";
                return RedirectToAction("Logout");
            }

            ModelState.AddModelError("", "Contraseña actual incorrecta");
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var newToken = await _authService.RefreshAccessTokenAsync(refreshToken);
            if (newToken == null)
                return Unauthorized();

            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.ExpiresAt
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("access_token", newToken.AccessToken, accessOptions);
            Response.Cookies.Append("refreshToken", newToken.RefreshToken, refreshOptions);

            return Ok(new { success = true });
        }





    }
}


