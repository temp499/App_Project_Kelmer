using System.Security.Claims;
using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private const string AccessCookie = "access_token";
    private const string RefreshCookie = "refreshToken";

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Revise los datos del registro" });

        var (success, error) = await authService.RegisterAsync(dto);
        return success
            ? Ok(new { success = true, message = "Revise su correo para confirmar la cuenta." })
            : Conflict(new { message = error });
    }

    [HttpGet]
    public IActionResult Login() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction("Index", "Home")
        : View();

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Ingrese un correo y una contraseña válidos" });

        var (success, result, error) = await authService.LoginAsync(dto);
        if (!success || result is null)
            return Unauthorized(new { message = error ?? "Credenciales inválidas" });

        SetSession(result);
        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshCookie];
        if (!string.IsNullOrEmpty(refreshToken))
            await authService.RevokeRefreshTokenAsync(refreshToken);

        ClearSession();
        return Ok(new { success = true, message = "Sesión cerrada" });
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest("Parámetros inválidos");

        if (await authService.ConfirmEmialAsync(email, token))
            TempData["Success"] = "Correo confirmado. Ya puede iniciar sesión.";
        else
            TempData["Error"] = "El enlace de confirmación es inválido o ya se utilizó.";

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> ResendConfirmation([FromBody] ForgetPasswordRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Ingrese un correo válido" });

        await authService.ResendConfirmationEmailAsync(dto.Email);
        return Ok(new { success = true, message = "Si la cuenta está pendiente, recibirá un enlace de confirmación." });
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromForm] ForgetPasswordRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await authService.SendResetPasswordLinkAsync(dto.Email);
        TempData["Success"] = "Si el correo está registrado, recibirá un enlace para restablecer su contraseña.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token) =>
        View(new ResetPasswordRequestDto { Email = email, Token = token });

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (!await authService.ResetPasswordAsync(dto))
        {
            ModelState.AddModelError("", "El enlace es inválido o expiró");
            return View(dto);
        }

        ClearSession();
        TempData["Success"] = "Contraseña actualizada. Inicie sesión con su nueva contraseña.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword() => View();

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
            return Unauthorized();

        if (!await authService.ChangePasswordAsync(email, dto))
        {
            ModelState.AddModelError("", "La contraseña actual es incorrecta");
            return View(dto);
        }

        ClearSession();
        TempData["Success"] = "Contraseña actualizada. Inicie sesión con su nueva contraseña.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies[RefreshCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "Inicie sesión para continuar" });

        var result = await authService.RefreshAccessTokenAsync(refreshToken);
        if (result is null)
            return Unauthorized(new { message = "La sesión expiró. Vuelva a iniciar sesión." });

        SetSession(result);
        return Ok(new { success = true, message = "Sesión actualizada" });
    }

    private void SetSession(LoginResponseDto result)
    {
        Response.Cookies.Append(AccessCookie, result.AccessToken, SessionCookie(result.ExpiresAt));
        Response.Cookies.Append(RefreshCookie, result.RefreshToken, SessionCookie(result.RefreshTokenExpiresAt));
    }

    private void ClearSession()
    {
        Response.Cookies.Delete(AccessCookie, SessionCookie(null));
        Response.Cookies.Delete(RefreshCookie, SessionCookie(null));
    }

    private static CookieOptions SessionCookie(DateTime? expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = expires
    };
}
