using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Proyec_Tienda.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Registrar()
        {
            return View();
        }
        public ActionResult Reestablecer()
        {
            return View();
        }
        public ActionResult CambiarClave()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;
            ViewData["Nombre"] = string.IsNullOrEmpty(objeto.Nombre) ? "" : objeto.Nombre;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;
            if (objeto.Clave != objeto.ConfirmarClave)
            {
                ViewBag.Error = "Las Contraseñas no Coinciden";
                return View();
            }
            resultado = new CN_Cliente().Registrar(objeto, out mensaje);
            if (resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Cliente oCliente = null;
            oCliente = new CN_Cliente().Lista().Where(item => item.Correo == correo && item.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();
            if (oCliente == null)
            {
                ViewBag.Error = "Correo o Contraseña no son Correctas";
                return View();
            }
            else
            {
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(oCliente.Correo, false);
                    Session["Cliente"] = oCliente;
                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Tienda");
                }
            }
        }
        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente Cliente = new Cliente();
            Cliente = new CN_Cliente().Lista().Where(item => item.Correo == correo).FirstOrDefault();
            if (Cliente == null)
            {
                ViewBag.Error = "No se Encontro un Cliente Relacionado a ese Correo";
                return View();
            }
            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().ReestablecerClave(Cliente.IdCliente, correo, out mensaje);
            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }
        [HttpPost]
        public ActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmarclave)
        {
            Cliente oCliente = new Cliente();
            oCliente = new CN_Cliente().Lista().Where(u => u.IdCliente == int.Parse(idcliente)).FirstOrDefault();
            if (oCliente.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["IdCliente"] = idcliente;
                ViewData["clave"] = "";
                ViewBag.Error = "La Contraseña Actual no es Correcta";
                return View();
            }
            else if (nuevaclave != confirmarclave)
            {
                TempData["IdCliente"] = idcliente;
                ViewData["clave"] = claveactual;
                ViewBag.Error = "Las Contraseña no Coinciden";
                return View();
            }
            ViewData["clave"] = "";
            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);
            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);
            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = idcliente;
                ViewBag.Error = mensaje;
                return View();
            }
        }
        public ActionResult CerrarSesion()
        {
            Session["Cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");
        }
    }
}