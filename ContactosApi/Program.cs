using System.Text;
using ContactosApi.Models;
using ContactosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("La configuracion JWT no existe.");

// Servicios propios de la app.
builder.Services.AddSingleton<ContactoService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger queda preparado para probar endpoints con Bearer token.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contactos API",
        Version = "v1",
        Description = "API de ejemplo para gestionar contactos con Minimal APIs, Controllers y JWT."
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresar el token JWT con el formato: Bearer {token}",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [jwtSecurityScheme] = []
    });
});

// La API esperara autenticacion Bearer en requests protegidas.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Reglas que debe cumplir cualquier token recibido.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        await Results.Problem(
            title: "Ocurrio un error inesperado.",
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(context);
    });
});

// Swagger solo se habilita en desarrollo.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Primero se autentica al usuario y despues se revisan permisos.
app.UseAuthentication();
app.UseAuthorization();

// Minimal API para listar todos los contactos.
app.MapGet("/minimal/contactos", (ContactoService contactoService) =>
{
    return Results.Ok(contactoService.ObtenerTodos());
})
.WithName("ObtenerContactosMinimal")
.WithTags("Minimal")
.WithOpenApi();

// Login: valida credenciales y devuelve un JWT.
app.MapPost("/api/auth/login", (LoginRequest request, AuthService authService, TokenService tokenService) =>
{
    if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { mensaje = "Usuario y password son obligatorios." });
    }

    var usuario = authService.Login(request.Usuario, request.Password);

    if (usuario is null)
    {
        return Results.Unauthorized();
    }

    var tokenString = tokenService.GenerarToken(usuario);

    return Results.Ok(new
    {
        token = tokenString,
        usuario = usuario.NombreUsuario,
        rol = usuario.Rol
    });
})
.WithName("Login")
.WithTags("Auth")
.WithOpenApi();

// Activa los endpoints definidos en controllers.
app.MapControllers();

app.Run();
