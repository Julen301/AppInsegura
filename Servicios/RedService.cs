using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppInsegura.Servicios
{
    public class RedService
    {
        // CORRECCIÓN F11: ya no hay una clave real escrita en el código.
        // En una app cliente no hay secretos de verdad: lo ideal es que esta llamada
        // pase por un backend propio que añada la clave en el servidor. Como ejemplo
        // de cómo evitar que quede grabada en el binario, aquí se lee de una variable de entorno.
        private static readonly string ApiKey = Environment.GetEnvironmentVariable("MIAPP_API_KEY") ?? "";

        // CORRECCIÓN F12: HTTPS en vez de HTTP
        private const string UrlServidor = "https://api.miapp-insegura.local/puntuaciones";

        public void EnviarPuntuacion(string nombreUsuario, int puntuacion)
        {
            try
            {
                EnviarPuntuacionAsync(nombreUsuario, puntuacion).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se ha podido conectar con el servidor (es normal si no tienes conexión real).");
                System.IO.File.AppendAllText("app_errores.log", $"{DateTime.Now:u} - RedService: {ex}\n");
            }
        }

        private async Task EnviarPuntuacionAsync(string nombreUsuario, int puntuacion)
        {
            using var cliente = new HttpClient();

            // CORRECCIÓN F13: la api_key ya no viaja en la URL (queda en logs/proxies/historial),
            // ahora va en la cabecera de autorización
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

            string url = $"{UrlServidor}?usuario={Uri.EscapeDataString(nombreUsuario)}&puntos={puntuacion}";

            HttpResponseMessage respuesta = await cliente.GetAsync(url);
            Console.WriteLine($"Respuesta del servidor: {(int)respuesta.StatusCode}");
        }
    }
}


