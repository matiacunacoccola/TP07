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
    const tarjeta = document.getElementById("plantillaPublicacion").cloneNode(true);

    tarjeta.id = "publicacion" + publicacion.id;
    tarjeta.style.display = "block";

    tarjeta.querySelector(".titulo").textContent = publicacion.titulo;

    tarjeta.querySelector(".autor").textContent =
        publicacion.nombreUsuario + " - " + publicacion.fechaPublicacion;

    tarjeta.querySelector(".foto").src =
        "/img/publicaciones/" + publicacion.imagen;

    tarjeta.querySelector(".descripcion").textContent =
        publicacion.descripcion;

    const cantidad = tarjeta.querySelector(".cantidad");

    cantidad.id = "cantidadMeGusta" + publicacion.id;
    cantidad.textContent = "Me Gusta: " + publicacion.cantidadMeGusta;

    const botonReaccion = tarjeta.querySelector(".botonReaccion");

    botonReaccion.id = "botonMeGusta" + publicacion.id;
    botonReaccion.onclick = () => cambiarReaccion(publicacion.id);

    if (publicacion.yaLeGusto)
    {
        botonReaccion.textContent = "Ya no me gusta";
    }

    tarjeta.querySelector(".comentariosPublicacion").id =
        "comentarios" + publicacion.id;

    tarjeta.querySelector(".textoNuevo").id =
        "textoComentario" + publicacion.id;

    const botonComentar = tarjeta.querySelector(".botonComentar");

    botonComentar.id = "botonComentar" + publicacion.id;
    botonComentar.onclick = () => enviarComentario(publicacion.id);

    if (document.getElementById("estadoSesion").value == "no")
    {
        botonReaccion.style.display = "none";

        tarjeta.querySelector(".escribirComentario").style.display = "none";
    }

    document.getElementById("listaPublicaciones").appendChild(tarjeta);

    publicacion.comentarios.forEach(comentario =>
    {
        mostrarComentario(comentario, publicacion.id);
    });
}

function mostrarComentario(comentario, idPublicacion)
{
    const nuevo = document.getElementById("plantillaComentario").cloneNode(true);

    nuevo.id = "";
    nuevo.style.display = "block";

    nuevo.querySelector(".autorComentario").textContent =
        comentario.nombreUsuario + " - " + comentario.fechaComentario;

    nuevo.querySelector(".textoComentario").textContent =
        comentario.texto;

    document.getElementById("comentarios" + idPublicacion).appendChild(nuevo);
}