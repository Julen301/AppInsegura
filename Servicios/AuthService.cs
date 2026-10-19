using System;
using System.Security.Cryptography;
using AppInsegura.Datos;
using AppInsegura.Modelos;

namespace AppInsegura.Servicios
{
    public class AuthService
    {
        private readonly BaseDatosUsuarios baseDatos;
        private const int MaxIntentosFallidos = 5; // CORRECCIÓN F10

        public AuthService(BaseDatosUsuarios baseDatos)
        {
            this.baseDatos = baseDatos;
        }

        public Usuario Registrar(string nombre, string contrasena, string rol = "jugador")
        {
            // CORRECCIÓN F5: validación básica de entrada (lista blanca)
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 20)
                throw new ArgumentException("Nombre de usuario no válido.");
            if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8)
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");

            byte[] sal = GenerarSal(); // CORRECCIÓN F6: sal aleatoria segura por usuario

            var nuevo = new Usuario
            {
                Nombre = nombre,
                ContrasenaSal = Convert.ToBase64String(sal),
                ContrasenaHash = CalcularHash(contrasena, sal),
                Rol = rol,
                TokenSesion = ""
            };

            baseDatos.Agregar(nuevo);
            return nuevo;
        }

        public Usuario? IniciarSesion(string nombre, string contrasena)
        {
            Usuario? usuario = baseDatos.BuscarExacto(nombre);
            if (usuario == null) return null;

            // CORRECCIÓN F10: fallar de forma segura tras demasiados intentos
            if (usuario.IntentosFallidos >= MaxIntentosFallidos)
            {
                Console.WriteLine("Cuenta bloqueada temporalmente por demasiados intentos fallidos.");
                return null;
            }

            byte[] sal = Convert.FromBase64String(usuario.ContrasenaSal);
            string hashIntento = CalcularHash(contrasena, sal);

            if (usuario.ContrasenaHash != hashIntento)
            {
                usuario.IntentosFallidos++;
                return null;
            }

            usuario.IntentosFallidos = 0;
            usuario.TokenSesion = GenerarTokenSesion();

            // CORRECCIÓN F8: ya no se registra el token en el log, solo el evento
            Console.WriteLine($"[LOG] Login correcto -> usuario: {usuario.Nombre}");

            // CORRECCIÓN F9: se elimina el guardado de la sesión en disco en texto plano.
            // La sesión vive solo en memoria mientras dura la ejecución.

            return usuario;
        }

        // CORRECCIÓN F6: PBKDF2 con sal en vez de MD5 sin sal (librería estándar del sistema)
        private string CalcularHash(string contrasena, byte[] sal)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(contrasena, sal, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }

        private byte[] GenerarSal()
        {
            byte[] sal = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(sal);
            return sal;
        }

        // CORRECCIÓN F7: generador criptográficamente seguro y token más largo
        private string GenerarTokenSesion()
        {
            byte[] tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            return Convert.ToHexString(tokenBytes);
        }
    }
}
