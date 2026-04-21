using ContactosApi.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactosApi.Services;

public class AuthService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();
    private readonly List<Usuario> _usuarios =
    [
        new Usuario
        {
            NombreUsuario = "admin",
            PasswordHash = string.Empty,
            Rol = "Admin"
        }
    ];

    public AuthService()
    {
        foreach (var usuario in _usuarios)
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, "1234");
        }
    }

    public Usuario? Login(string usuario, string password)
    {
        var usuarioEncontrado = _usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);

        if (usuarioEncontrado is null)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuarioEncontrado,
            usuarioEncontrado.PasswordHash,
            password);

        return resultado == PasswordVerificationResult.Failed
            ? null
            : usuarioEncontrado;
    }
}
