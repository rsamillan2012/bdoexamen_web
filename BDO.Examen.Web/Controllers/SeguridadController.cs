using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace BDO.Examen.Web.Controllers
{
    public class SeguridadController : MasterController
    {
        private const string SEPARADOR = "|";


        public void LogOut()
        {

            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            // clear token cookie
            HttpCookie cookietkn = new HttpCookie(".CORE", "");
            cookietkn.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookietkn);

            FormsAuthentication.RedirectToLoginPage();
        }

        public void DefinirCookieAutenticacion(string login, string roles, DateTime registrado, string NombreApellido, string tknm)
        {
            // se leen las configuraciones de asp.net
            HttpCookiesSection cookieSection = (HttpCookiesSection)ConfigurationManager.GetSection("system.web/httpCookies");
            AuthenticationSection authenticationSection = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");

            // se crea el ticket de autenticacion con el login del usuario como nombre y como valor el codigo de aplicacion

            //DateTime tiempo_expira = DateTime.Now.AddMinutes(authenticationSection.Forms.Timeout.TotalMinutes);
            DateTime tiempo_expira = DateTime.Now.AddMinutes(40);
            //DateTime tiempo_expira = DateTime.Now.AddMinutes(20);

            FormsAuthenticationTicket authTicket =
                    new FormsAuthenticationTicket(1, login, DateTime.Now, tiempo_expira, false,
                                            string.Join(SEPARADOR, roles, login, registrado.ToString("dd/MM/yyyy"), NombreApellido));

            // se encripta el ticket
            String encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            // se crea una cookie que almacene el cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            // si se requiere comunicacion segura se habilita en la cookie
            if (cookieSection.RequireSSL || authenticationSection.Forms.RequireSSL)
            {
                authCookie.Secure = true;
            }

            // se emite la cookie
            //HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpContext.Response.Cookies.Add(authCookie);
            //Response.Cookies.Add(authCookie);

            //AGREGAMOS UNA COOKIE TIPO TOKEN
            DateTime tiempo_expira_token = DateTime.Now.AddMinutes(40);

            FormsAuthenticationTicket TokenTicket =
                    new FormsAuthenticationTicket(1, "TOKEN", DateTime.Now, tiempo_expira_token, false,
                                            string.Join(SEPARADOR, tknm));

            // se encripta el ticket
            String encryptedTokenTicket = FormsAuthentication.Encrypt(TokenTicket);

            // se crea una cookie que almacene el cookie
            HttpCookie TokenCookie = new HttpCookie(".CORE", encryptedTokenTicket);
            //Agregamos la Cookie
            HttpContext.Response.Cookies.Add(TokenCookie);


        }



        [HttpPost]
        [ActionName("authe")]
        public JsonResult Autenticacion(string email, string tknm, string datos)
        {
            this.DefinirCookieAutenticacion(email, "Administrador", DateTime.Now, datos, tknm);


            string url_final = System.Configuration.ConfigurationManager.AppSettings["base_url_login"].ToString();
            string formulario_principal = url_final + "dashboard/principal";
            //string formulario_principal = "dashboard/principal";

            return Json(new { form_current = formulario_principal, tkm = tknm }, JsonRequestBehavior.AllowGet);
        }


        // GET: Seguridad
        public ActionResult Index()
        {
            string _returnurl = Request["ReturnUrl"];

            ViewBag.sw_mensaje_error_400 = this.sw_mensaje_error_400();
            ViewBag.sw_mensaje_error_500 = this.sw_mensaje_error_500();
            ViewBag.sw_mensaje_error_404 = this.sw_mensaje_error_404();
            ViewBag.sw_mensaje_error = this.sw_mensaje_error();
            ViewBag.sw_mensaje_servicio_nodisponible = this.sw_mensaje_servicio_nodisponible();

            if (Request.IsAuthenticated) { 
            
            }

            ViewBag.sw_login = this.sw_login();
            ViewBag.sw_ingrenormal = _returnurl;
            return View();
        }
    }
}