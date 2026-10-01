using Dapper;
using Microsoft.Data.SqlClient;

namespace TP07.Models;

public class BD
{
    private string connectiontostring = @"Server=localhost;DataBase=DBRedSocial;Integrated Security=True;TrustServerCertificate=True;";

    public void GuardarUsuario(Usuario usuario)
    {
        string consulta = @"INSERT INTO Usuarios
                            (NombreUsuario, Contraseña, Nombre, Apellido, TipoUsuario)
                            VALUES
                            (@NombreUsuario, @Contrasena, @Nombre, @Apellido, @TipoUsuario)";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            conexion.Execute(consulta, new
            {
                NombreUsuario = usuario.NombreUsuario,
                Contrasena = usuario.Contrasena,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                TipoUsuario = usuario.TipoUsuario
            });
        }
    }

    public bool NombreRegistrado(string nombreUsuario)
    {
        string consulta = "SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            List<Usuario> usuarios = conexion.Query<Usuario>(consulta, new
            {
                NombreUsuario = nombreUsuario
            }).ToList();

            if (usuarios.Count > 0)
            {
                return true;
            }

            return false;
        }
    }

    public Usuario BuscarCredenciales(string nombreUsuario, string contrasena)
    {
        string consulta = @"SELECT Id,
                                   NombreUsuario,
                                   Contraseña AS Contrasena,
                                   Nombre,
                                   Apellido,
                                   TipoUsuario
                            FROM Usuarios
                            WHERE NombreUsuario = @NombreUsuario
                            AND Contraseña = @Contrasena";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            return conexion.QueryFirstOrDefault<Usuario>(consulta, new
            {
                NombreUsuario = nombreUsuario,
                Contrasena = contrasena
            });
        }
    }

    public Usuario BuscarUsuario(int idUsuario)
    {
        string consulta = @"SELECT Id,
                                   NombreUsuario,
                                   Contraseña AS Contrasena,
                                   Nombre,
                                   Apellido,
                                   TipoUsuario
                            FROM Usuarios
                            WHERE Id = @IdUsuario";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            return conexion.QueryFirstOrDefault<Usuario>(consulta, new
            {
                IdUsuario = idUsuario
            });
        }
    }

    public void GuardarPublicacion(Publicacion publicacion)
    {
        string consulta = @"INSERT INTO Publicaciones
                            (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
                            VALUES
                            (@IdUsuario, @Titulo, @Descripcion, @Imagen, @FechaPublicacion)";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            conexion.Execute(consulta, new
            {
                IdUsuario = publicacion.IdUsuario,
                Titulo = publicacion.Titulo,
                Descripcion = publicacion.Descripcion,
                Imagen = publicacion.Imagen,
                FechaPublicacion = publicacion.FechaPublicacion
            });
        }
    }

    public List<Publicacion> ListarPublicaciones(int desde, int idUsuarioLogueado)
    {
        List<Publicacion> publicaciones = new List<Publicacion>();

        string consulta = @"SELECT *
                            FROM Publicaciones
                            ORDER BY FechaPublicacion DESC, Id DESC
                            OFFSET @Desde ROWS FETCH NEXT 10 ROWS ONLY";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            publicaciones = conexion.Query<Publicacion>(consulta, new
            {
                Desde = desde
            }).ToList();
        }

        foreach (Publicacion publicacion in publicaciones)
        {
            Usuario usuario = BuscarUsuario(publicacion.IdUsuario);

            if (usuario != null)
            {
                publicacion.NombreUsuario = usuario.NombreUsuario;
            }

            string buscarReacciones = "SELECT IdUsuario FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion";

            using (SqlConnection conexion = new SqlConnection(connectiontostring))
            {
                List<int> usuarios = conexion.Query<int>(buscarReacciones, new
                {
                    IdPublicacion = publicacion.Id
                }).ToList();

                publicacion.CantidadMeGusta = usuarios.Count;

                foreach (int id in usuarios)
                {
                    if (id == idUsuarioLogueado)
                    {
                        publicacion.YaLeGusto = true;
                    }
                }
            }

            publicacion.Comentarios = ListarComentarios(publicacion.Id);
        }

        return publicaciones;
    }

    public Publicacion AlternarReaccion(int idPublicacion, int idUsuario)
    {
        Publicacion resultado = new Publicacion();

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            List<Publicacion> publicaciones = conexion.Query<Publicacion>(
                "SELECT Id FROM Publicaciones WHERE Id = @IdPublicacion",
                new
                {
                    IdPublicacion = idPublicacion
                }).ToList();

            if (publicaciones.Count == 0)
            {
                return resultado;
            }

            List<int> reacciones = conexion.Query<int>(
                "SELECT Id FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario",
                new
                {
                    IdPublicacion = idPublicacion,
                    IdUsuario = idUsuario
                }).ToList();

            if (reacciones.Count > 0)
            {
                conexion.Execute(
                    "DELETE FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario",
                    new
                    {
                        IdPublicacion = idPublicacion,
                        IdUsuario = idUsuario
                    });
            }
            else
            {
                conexion.Execute(
                    "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@IdPublicacion, @IdUsuario)",
                    new
                    {
                        IdPublicacion = idPublicacion,
                        IdUsuario = idUsuario
                    });
            }

            List<int> usuarios = conexion.Query<int>(
                "SELECT IdUsuario FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion",
                new
                {
                    IdPublicacion = idPublicacion
                }).ToList();

            resultado.Id = idPublicacion;
            resultado.CantidadMeGusta = usuarios.Count;

            foreach (int id in usuarios)
            {
                if (id == idUsuario)
                {
                    resultado.YaLeGusto = true;
                }
            }
        }

        return resultado;
    }

    public List<Comentario> ListarComentarios(int idPublicacion)
    {
        List<Comentario> comentarios = new List<Comentario>();

        string consulta = "SELECT * FROM Comentarios WHERE IdPublicacion = @IdPublicacion ORDER BY FechaComentario ASC";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            comentarios = conexion.Query<Comentario>(consulta, new
            {
                IdPublicacion = idPublicacion
            }).ToList();
        }

        foreach (Comentario comentario in comentarios)
        {
            Usuario usuario = BuscarUsuario(comentario.IdUsuarioComenta);

            if (usuario != null)
            {
                comentario.NombreUsuario = usuario.NombreUsuario;
            }
        }

        return comentarios;
    }

    public bool GuardarComentario(Comentario comentario)
    {
        string consulta = @"INSERT INTO Comentarios
                            (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario)
                            SELECT @IdPublicacion, @IdUsuarioComenta, @Texto, @FechaComentario
                            FROM Publicaciones
                            WHERE Id = @IdPublicacion";

        using (SqlConnection conexion = new SqlConnection(connectiontostring))
        {
            int filas = conexion.Execute(consulta, new
            {
                IdPublicacion = comentario.IdPublicacion,
                IdUsuarioComenta = comentario.IdUsuarioComenta,
                Texto = comentario.Texto,
                FechaComentario = comentario.FechaComentario
            });

            return filas > 0;
        }
    }
}