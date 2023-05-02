namespace CrearLoginBasico.Models;

public struct UsuarioModel
{
    public class Login
    {
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public bool MantenerActivo { get; set; }
    }
}
