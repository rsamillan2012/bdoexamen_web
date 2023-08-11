using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Extensions.Logging;

namespace BDO.Examen.Web.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master

        protected virtual string sw_login()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_login"].ToString();
        }
        protected virtual string sw_mensaje_error_400()
        {
            return System.Configuration.ConfigurationManager.AppSettings["mensaje_error_400"].ToString();
        }

        protected virtual string sw_listado()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_listado"].ToString();
        }

        protected virtual string sw_guardar()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_guardar"].ToString();
        }
        protected virtual string sw_getentidad()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_getentidad"].ToString();
        }

        protected virtual string sw_modificar()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_modificar"].ToString();
        }

        protected virtual string sw_eliminar()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_eliminar"].ToString();
        }

        protected virtual string sw_listadocriterio()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_listadocriterio"].ToString();
        }

        protected virtual string sw_listado_excel()
        {
            return System.Configuration.ConfigurationManager.AppSettings["sw_listado_excel"].ToString();
        }

        

        public string[] MasterCredentials()
        {

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string userData = ticket.UserData;
            string[] _array_gdec = userData.Split('|');
            return _array_gdec;
        }

        public string TokenCredentials()
        {
            HttpCookie authCookie = Request.Cookies[".CORE"];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string TokenData = ticket.UserData;
            return TokenData;
        }

        protected virtual string sw_mensaje_error_500()
        {
            return System.Configuration.ConfigurationManager.AppSettings["mensaje_error_500"].ToString();
        }

        protected virtual string sw_mensaje_error_404()
        {
            return System.Configuration.ConfigurationManager.AppSettings["mensaje_error_404"].ToString();
        }


        protected virtual string sw_mensaje_error()
        {
            return System.Configuration.ConfigurationManager.AppSettings["mensaje_error"].ToString();
        }

        protected virtual string sw_mensaje_servicio_nodisponible()
        {
            return System.Configuration.ConfigurationManager.AppSettings["mensaje_servicio_nodisponible"].ToString();
        }


        protected virtual void ErrorLog(Exception ex, string adicional = "")
        {
            var loggerFactory = new LoggerFactory();

            //string nombreFile = string.Format("Log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            string nombreFile = string.Format("Log.txt");

            loggerFactory.AddFile("C:\\BDOError\\" + nombreFile);

            string msg = "Error Opcion: " + ex.Message + ", pila : " + ex.StackTrace.ToString();

            if (!string.IsNullOrEmpty(adicional))
            {
                msg += adicional;
            }

            var logger = loggerFactory.CreateLogger("Test");

            //loggerFactory.AddProvider(new CustomLoggerProvider("provider1", ThrowExceptionAt.None, store));

            // Act
            logger.LogError(msg);

        }



        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isdev = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsDebugger"].ToString());

            if (!isdev)
            {
                string esconexionsegura = string.Empty;
                //if (!filterContext.HttpContext.Request.IsSecureConnection)
                //{
                //    esconexionsegura = "no";
                //}
                //else {
                //    esconexionsegura = "si";
                //}

                //var loggerFactory = new LoggerFactory();
                //string nombreFile = string.Format("Log.txt");
                //loggerFactory.AddFile("C:\\OpcionErrorEspecial\\" + nombreFile);
                //var logger = loggerFactory.CreateLogger("Test");

                //string msg = "";
                //msg += "es conexion segura : " + esconexionsegura + "\n";
                //msg += "mi url : " + filterContext.HttpContext.Request.Url.ToString();
                //logger.LogError(msg);

                //var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                //filterContext.Result = new RedirectResult(url);



            }
        }
    }
}