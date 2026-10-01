namespace TP07.Models;

public class SesionUsuario
{
    public static int ObtenerId(ISession sesion)
    {
        string numero = sesion.GetString("IdUsuarioLogueado");

        if (numero == null || numero == "")
        {
            return 0;
        }

        return int.Parse(numero);
    }

    public static void Guardar(ISession sesion, Usuario usuario)
    {
        sesion.SetString("IdUsuarioLogueado", usuario.Id.ToString());
        sesion.SetString("UsuarioLogueado", usuario.NombreUsuario);
        sesion.SetString("NombreUsuarioLogueado", usuario.NombreUsuario);
        sesion.SetString("NombreLogueado", usuario.Nombre);
        sesion.SetString("ApellidoLogueado", usuario.Apellido);
        sesion.SetString("TipoUsuarioLogueado", usuario.TipoUsuario);
    }
}