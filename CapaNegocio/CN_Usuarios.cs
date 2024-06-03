using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
namespace CapaNegocio
{
    public class CN_Usuarios
    {
        private CD_Usuarios objCapaDato = new CD_Usuarios();
        public List<Usuario> Lista()
        {
            return objCapaDato.lista();
        }
        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if(string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El Nombre del usuario no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El Apellidos del usuario no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El Correo del usuario no puede estar vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                string clave = CN_Recursos.GenerarClave();
                string asunto = "Creacion de Cuenta";
                string mensaje_correo = "<h3>Su Cuenta Fue Creada Correctamente</h3></br><p>Su contraseña Para Aceder es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", clave);
                bool respuesta = CN_Recursos.EnviarCorreo(obj.Correo, asunto, mensaje_correo);
                if (respuesta)
                {
                    obj.Clave = CN_Recursos.ConvertirSha256(clave);
                    return objCapaDato.Registrar(obj, out Mensaje);
                }
                else
                {
                    Mensaje = "No se Puede Enviar el Correo";
                    return 0;
                }
                obj.Clave = CN_Recursos.ConvertirSha256(clave);
                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0; 
            }
        }
        public bool Editar(Usuario obj, out string Mensaje)
        {

            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El Nombre del usuario no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El Apellidos del usuario no puede estar vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El Correo del usuario no puede estar vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(obj, out Mensaje);
            }else
            {
                return false;
            }
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }
        public bool CambiarClave(int idusuario, string nuevaclave, out string Mensaje)
        {
            return objCapaDato.CambiarClave(idusuario, nuevaclave, out Mensaje);
        }
        public bool ReestablecerClave(int idusuario, string Correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado = objCapaDato.ReestrabecerClave(idusuario, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);
            if (resultado)
            {
                string asunto = "Contraseña Reestablecidad";
                string mensaje_correo = "<h3>Su Cuenta Fue Reestablecidad Correctamente</h3></br><p>Su contraseña Para Aceder Ahoraes: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", nuevaclave);
                bool respuesta = CN_Recursos.EnviarCorreo(Correo, asunto, mensaje_correo);
                if (respuesta)
                {
                    return true;
                }
                else
                {
                    Mensaje = "No se Pudo Enviar el Correo";
                    return false;
                }
            }
            else
            {
                Mensaje = "No se Pudo Reestablecer Clave";
                return false;
            }
        }
    }
}
