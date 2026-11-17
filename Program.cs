using System.Security.Claims;
using System.Text;
using App_Proyecto.Authorization;
using App_Proyecto.Data;
using App_Proyecto.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using App_Proyecto.Mapping;
using App_Proyecto.Models.Settings;
using App_Proyecto.Repositories;
using App_Proyecto.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra los servicios de la aplicación.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = Encoding.UTF8.GetByteCount(jwt.Key) >= 32
            ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
            : null,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = async context =>
        {
            var request = context.Request;
            context.Token = request.Cookies[AuthSessionCookies.Access];
            var refreshToken = request.Cookies[AuthSessionCookies.Refresh];
            if (!string.IsNullOrEmpty(context.Token) || string.IsNullOrEmpty(refreshToken) ||
                request.Headers.ContainsKey("Authorization") || !HttpMethods.IsGet(request.Method) ||
                !AcceptsHtml(request) ||
                context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>() is null)
                return;

            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
            var session = await authService.RefreshAccessTokenAsync(refreshToken);
            if (session is null)
                return;

            AuthSessionCookies.Set(context.Response, session);
            context.Token = session.AccessToken;
        },
        OnTokenValidated = async context =>
        {
            if (!int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                context.Fail("Usuario inválido");
                return;
            }

            var repository = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();
            var user = await repository.GetUsuarioByIdWithPermissionAsync(id);
            if (user is null || !bool.TryParse(user.EmailConfirmed, out var confirmed) || !confirmed)
            {
                context.Fail("Usuario no disponible");
                return;
            }

            var identity = (ClaimsIdentity)context.Principal!.Identity!;
            foreach (var claim in identity.FindAll(ClaimsPrincipalExtension.PermissionClaimType)
                         .Concat(identity.FindAll(ClaimTypes.Role)).ToList())
                identity.RemoveClaim(claim);

            if (user.Rol is not null)
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Rol.Name));
            foreach (var permission in EffectivePermission.FromUser(user))
                identity.AddClaim(new Claim(ClaimsPrincipalExtension.PermissionClaimType, permission));
        },
        OnForbidden = context =>
        {
            if (AcceptsHtml(context.Request))
                context.Response.Redirect(context.Request.PathBase + "/Auth/AccessDenied");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (AcceptsHtml(context.Request))
            {
                context.HandleResponse();
                context.Response.Redirect(context.Request.PathBase + "/Auth/Login");
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin.panel", policy => policy.RequireAuthenticatedUser()
        .RequireAssertion(context => context.User.HasAnyPermission("admin.panel", "rol.ver", "permiso.ver", "usuario.ver")))
    .AddPolicy("permiso.actualizar", policy => policy.RequireAuthenticatedUser()
        .RequireAssertion(context => context.User.HasAnyPermission("permiso.actualizar", "permiso.crear")))
    .AddPolicy("permiso.borrar", policy => policy.RequireAuthenticatedUser()
        .RequireAssertion(context => context.User.HasAnyPermission("permiso.borrar", "permiso.crear")));
builder.Services.AddSingleton<IAuthorizationPolicyProvider, JwtPermissionPolicies.PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, JwtPermissionPolicies.PermissionAuthorizationHandler>();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));

builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRoleRepository, RolRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

var app = builder.Build();

if (args.Contains("--seed", StringComparer.Ordinal))
{
    if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
        throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection antes de inicializar la base.");

    await using var scope = app.Services.CreateAsyncScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDBContext>());
    return;
}

// Configura el flujo de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS indica al navegador que use HTTPS.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages(context =>
{
    var httpContext = context.HttpContext;
    if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound &&
        HttpMethods.IsGet(httpContext.Request.Method) && AcceptsHtml(httpContext.Request))
        httpContext.Response.Redirect(httpContext.Request.PathBase + "/Home/NotFoundPage");

    return Task.CompletedTask;
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static bool AcceptsHtml(HttpRequest request) =>
    request.GetTypedHeaders().Accept.Any(mediaType =>
        mediaType.Quality.GetValueOrDefault(1) > 0 &&
        string.Equals(mediaType.MediaType.Value, "text/html", StringComparison.OrdinalIgnoreCase));

