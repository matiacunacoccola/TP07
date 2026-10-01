function comprobarFormulario()
{
    const usuario = document.getElementById("nombreUsuario").value;
    const clave = document.getElementById("contrasena").value;
    const nombre = document.getElementById("nombre").value;
    const apellido = document.getElementById("apellido").value;
    const tipo = document.getElementById("tipoUsuario").value;

    let mensaje = "";

    if (usuario.length < 4)
    {
        mensaje = "Elegí un usuario de 4 caracteres o más.";
    }
    else if (clave.length < 6)
    {
        mensaje = "La clave necesita 6 caracteres o más.";
    }
    else if (nombre == "" || soloLetras(nombre) == false)
    {
        mensaje = "Revisá tu nombre: usá letras y espacios.";
    }
    else if (apellido == "" || soloLetras(apellido) == false)
    {
        mensaje = "Revisá tu apellido: usá letras y espacios.";
    }
    else if (tipo == "")
    {
        mensaje = "Indicá tu vínculo con la comunidad.";
    }

    document.getElementById("errorRegistro").innerHTML = mensaje;

    return mensaje == "";
}

function soloLetras(texto)
{
    const letras = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúÁÉÍÓÚñÑ ";

    for (let i = 0; i < texto.length; i++)
    {
        if (letras.includes(texto[i]) == false)
        {
            return false;
        }
    }

    return true;
}
function cambiarReaccion(idPublicacion)
{
    fetch('/Home/ActualizarReaccion?idPublicacion=' + idPublicacion, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
    .then(response => response.json())
    .then(data =>
    {
        if (data.id != 0)
        {
            document.getElementById("cantidadMeGusta" + idPublicacion).innerHTML =
                "Me Gusta: " + data.cantidadMeGusta;

            if (data.yaLeGusto)
            {
                document.getElementById("botonMeGusta" + idPublicacion).innerHTML =
                    "Ya no me gusta";
            }
            else
            {
                document.getElementById("botonMeGusta" + idPublicacion).innerHTML =
                    "Me Gusta";
            }
        }
        else
        {
            document.getElementById("mensajeAcciones").innerHTML =
                "No se pudo cambiar el Me Gusta. Revisá tu sesión.";
        }
    })
    .catch(error => console.error("Error:", error));
}

function enviarComentario(idPublicacion)
{
    const texto = document.getElementById("textoComentario" + idPublicacion).value;

    if (texto != "")
    {
        fetch('/Home/EnviarComentario?idPublicacion=' + idPublicacion + '&texto=' + texto, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
        .then(response => response.json())
        .then(data =>
        {
            if (data.idPublicacion != 0)
            {
                mostrarComentario(data, idPublicacion);

                document.getElementById("textoComentario" + idPublicacion).value = "";
            }
            else
            {
                document.getElementById("mensajeAcciones").innerHTML =
                    "Revisá el comentario y comprobá que tu sesión siga abierta.";
            }
        })
        .catch(error => console.error("Error:", error));
    }
}
let desde = 10;

function cargarMas()
{
    fetch('/Home/ObtenerMas?desde=' + desde, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
    .then(response => response.json())
    .then(data =>
    {
        data.forEach(publicacion =>
        {
            mostrarPublicacion(publicacion);
        });

        desde = desde + 10;

        if (data.length < 10)
        {
            document.getElementById("botonVerMas").style.display = "none";
        }
    })
    .catch(error => console.error("Error:", error));
}

function mostrarPublicacion(publicacion)
{
    let usuarioLogueado = document.getElementById("estadoSesion").value;
    let contenido = "";

    contenido = contenido + "<div class='publicacion'>";
    contenido = contenido + "<h2>" + publicacion.titulo + "</h2>";

    contenido = contenido + "<p>" + publicacion.nombreUsuario +
        " - " + publicacion.fechaPublicacion + "</p>";

    contenido = contenido + "<img src='/img/publicaciones/" +
        publicacion.imagen + "' alt='Foto compartida'>";

    contenido = contenido + "<p>" + publicacion.descripcion + "</p>";

    contenido = contenido + "<p id='cantidadMeGusta" + publicacion.id +
        "'>Me Gusta: " + publicacion.cantidadMeGusta + "</p>";

    if (usuarioLogueado == "si")
    {
        if (publicacion.yaLeGusto == true)
        {
            contenido = contenido + "<button type='button' id='botonMeGusta" +
                publicacion.id + "' onclick='cambiarReaccion(" +
                publicacion.id + ")'>Ya no me gusta</button>";
        }
        else
        {
            contenido = contenido + "<button type='button' id='botonMeGusta" +
                publicacion.id + "' onclick='cambiarReaccion(" +
                publicacion.id + ")'>Me Gusta</button>";
        }
    }

    contenido = contenido + "<h3>Conversación</h3>";
    contenido = contenido + "<div id='comentarios" + publicacion.id + "'>";

    for (let i = 0; i < publicacion.comentarios.length; i++)
    {
        let comentario = publicacion.comentarios[i];

        contenido = contenido + "<div class='comentario'>";
        contenido = contenido + "<p>" + comentario.nombreUsuario +
            " - " + comentario.fechaComentario + "</p>";
        contenido = contenido + "<p>" + comentario.texto + "</p>";
        contenido = contenido + "</div>";
    }

    contenido = contenido + "</div>";

    if (usuarioLogueado == "si")
    {
        contenido = contenido + "<textarea id='textoComentario" +
            publicacion.id +
            "' rows='2' placeholder='Sumá tu opinión'></textarea>";

        contenido = contenido + "<button type='button' id='botonComentar" +
            publicacion.id + "' onclick='enviarComentario(" +
            publicacion.id + ")'>Comentar</button>";
    }

    contenido = contenido + "</div>";

    document.getElementById("listaPublicaciones").innerHTML =
        document.getElementById("listaPublicaciones").innerHTML + contenido;
}

function mostrarComentario(comentario, idPublicacion)
{
    let contenido = "";

    contenido = contenido + "<div class='comentario'>";
    contenido = contenido + "<p>" + comentario.nombreUsuario +
        " - " + comentario.fechaComentario + "</p>";
    contenido = contenido + "<p>" + comentario.texto + "</p>";
    contenido = contenido + "</div>";

    document.getElementById("comentarios" + idPublicacion).innerHTML =
        document.getElementById("comentarios" + idPublicacion).innerHTML + contenido;
}