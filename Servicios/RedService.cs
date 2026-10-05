using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppInsegura.Servicios
{
    public class RedService
    {
        private const string ApiKey = "sk_live_51Hj29aKlmZ9QwErTyUiOpAsDfGh";
        private const string UrlServidor = "http://api.miapp-insegura.local/puntuaciones"; //mal http = https

        public void EnviarPuntuacion(string nombreUsuario, int puntuacion)
        {
            try
            {
                EnviarPuntuacionAsync(nombreUsuario, puntuacion).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se ha podido conectar con el servidor (es normal si no tienes conexión real):"); // demasiado texto solo lo justo para el usuaria da pistas
                Console.WriteLine(ex.Message);
            }
        }

        private async Task EnviarPuntuacionAsync(string nombreUsuario, int puntuacion)
        {
            using var cliente = new HttpClient(); //Http mal el trafico se puede ller o modificar a de ser Https
            string url = $"{UrlServidor}?usuario={nombreUsuario}&puntos={puntuacion}&api_key={ApiKey}";

            Console.WriteLine($"Enviando puntuación a: {url}");
            HttpResponseMessage respuesta = await cliente.GetAsync(url); //mal http mal 
            Console.WriteLine($"Respuesta del servidor: {(int)respuesta.StatusCode}");
        }
    }
}
