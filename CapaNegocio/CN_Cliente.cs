using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Cliente objCapaDato = new CD_Cliente();
        public List<Cliente> Lista()
        {
            return objCapaDato.lista();
        }
        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El Nombre del Cliente no Puede Estar Vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El Apellidos del Cliente no Puede Estar Vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El Correo del Cliente no Puede Estar Vacio";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.Clave = CN_Recursos.ConvertirSha256(obj.Clave);
                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }
        }
        public bool CambiarClave(int idcliente, string nuevaclave, out string Mensaje)
        {
            return objCapaDato.CambiarClave(idcliente, nuevaclave, out Mensaje);
        }
        public bool ReestablecerClave(int idcliente, string Correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado = objCapaDato.ReestrabecerClave(idcliente, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);
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
