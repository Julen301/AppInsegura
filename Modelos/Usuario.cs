namespace AppInsegura.Modelos
{
    public class Usuario
    {
        public string Nombre { get; set; } = "";
        public string ContrasenaHash { get; set; } = "";
        public string ContrasenaSal { get; set; } = ""; // CORRECCIÓN F6: sal única por usuario para el hash
        public string Rol { get; set; } = "jugador";
        public string TokenSesion { get; set; } = "";
        public int IntentosFallidos { get; set; } = 0; // CORRECCIÓN F10: control de fuerza bruta
    }
}
