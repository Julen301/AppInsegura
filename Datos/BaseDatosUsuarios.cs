using System.Collections.Generic;
using System.Linq;
using AppInsegura.Modelos;

namespace AppInsegura.Datos
{
    public class BaseDatosUsuarios
    {
        private readonly List<Usuario> usuarios = new List<Usuario>();

        public void Agregar(Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        public List<Usuario> ListarTodos()
        {
            return usuarios;
        }

        public Usuario? BuscarExacto(string nombre)
        {
            return usuarios.FirstOrDefault(u => u.Nombre == nombre);
        }

        // CORRECCIÓN F1: se elimina la construcción de la consulta como texto
        // ("... WHERE nombre = '" + nombre + "'") y el "motor" que la interpretaba,
        // que permitía inyectar ' OR '1'='1 y devolver cualquier usuario.
        // La comparación directa con LINQ es el equivalente a una consulta parametrizada:
        // el valor nunca se interpreta como parte del "código" de la consulta.
        public Usuario? BuscarPorNombre(string nombreBuscado)
        {
            if (string.IsNullOrWhiteSpace(nombreBuscado) || nombreBuscado.Length > 30)
            {
                return null;
            }

            return usuarios.FirstOrDefault(u => u.Nombre == nombreBuscado);
        }
    }
}
