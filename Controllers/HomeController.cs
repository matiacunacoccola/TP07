using Microsoft.AspNetCore.Mvc;
using TP07.Models;

namespace TP07.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        int idUsuario = SesionUsuario.ObtenerId(HttpContext.Session);

        BD datos = new BD();

        ViewBag.Publicaciones = datos.ListarPublicaciones(0, idUsuario);
        ViewBag.UsuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");
        ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuarioLogueado");

        return View();
    }

    public IActionResult IniciarSesion()
    {
        return View();
    }

    [HttpPost]
    public IActionResult IniciarSesion(string nombreUsuario, string contrasena)
    {
        BD datos = new BD();

        Usuario usuario = datos.BuscarCredenciales(nombreUsuario, contrasena);

        if (usuario == null)
        {
            ViewBag.Error = "Revisá tu usuario y tu clave.";
            return View();
        }

        SesionUsuario.Guardar(HttpContext.Session, usuario);

        return RedirectToAction("Index");
    }

    public IActionResult Registrarse()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrarse(Usuario usuario)
    {
        BD datos = new BD();

        if (datos.NombreRegistrado(usuario.NombreUsuario))
        {
            ViewBag.Error = "Elegí otro usuario: ese ya está registrado.";
            return View();
        }

        datos.GuardarUsuario(usuario);

        return RedirectToAction("IniciarSesion");
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CrearPublicacion(string titulo, string descripcion, string imagen)
    {
        int idUsuario = SesionUsuario.ObtenerId(HttpContext.Session);

        if (idUsuario == 0)
        {
            return RedirectToAction("IniciarSesion");
        }

        if (titulo != null && titulo.Replace(" ", "") != "" &&
            descripcion != null && descripcion.Replace(" ", "") != "" &&
            (imagen == "atardecer.jpg" ||
             imagen == "bariloche.jpg" ||
             imagen == "futbol.jpg"))
        {
            Publicacion publicacion = new Publicacion();

            publicacion.IdUsuario = idUsuario;
            publicacion.Titulo = titulo;
            publicacion.Descripcion = descripcion;
            publicacion.Imagen = imagen;
            publicacion.FechaPublicacion = DateTime.Now;

            BD datos = new BD();

            datos.GuardarPublicacion(publicacion);
        }

        return RedirectToAction("Index");
    }

    public Publicacion ActualizarReaccion(int idPublicacion)
    {
        int idUsuario = SesionUsuario.ObtenerId(HttpContext.Session);

        Publicacion resultado = new Publicacion();

        if (idUsuario != 0)
        {
            BD datos = new BD();

            resultado = datos.AlternarReaccion(idPublicacion, idUsuario);
        }

        return resultado;
    }

    public Comentario EnviarComentario(int idPublicacion, string texto)
    {
        Comentario comentario = new Comentario();

        int idUsuario = SesionUsuario.ObtenerId(HttpContext.Session);

        if (idUsuario != 0 &&
            texto != null &&
            texto.Replace(" ", "") != "")
        {
            comentario.IdPublicacion = idPublicacion;
            comentario.IdUsuarioComenta = idUsuario;
            comentario.Texto = texto;
            comentario.FechaComentario = DateTime.Now;
            comentario.NombreUsuario = HttpContext.Session.GetString("NombreUsuarioLogueado");

            BD datos = new BD();

            if (datos.GuardarComentario(comentario) == false)
            {
                comentario = new Comentario();
            }
        }

        return comentario;
    }

    public List<Publicacion> ObtenerMas(int desde)
    {
        List<Publicacion> publicaciones = new List<Publicacion>();

        if (desde >= 0)
        {
            int idUsuario = SesionUsuario.ObtenerId(HttpContext.Session);

            BD datos = new BD();

            publicaciones = datos.ListarPublicaciones(desde, idUsuario);
        }

        return publicaciones;
    }
}