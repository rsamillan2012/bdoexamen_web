using BDO.Examen.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BDO.Examen.Web.Controllers
{
    [NoCache]
    [Authorize]
    public class DashboardController : MasterController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [ActionName("get-config")]
        public JsonResult TraerParametros()
        {
            string _token = string.Empty;
            bool _result = true;

            string[] _array_gdec = this.MasterCredentials();
            string _tkn = this.TokenCredentials(); //_array_gdec[4];

            //this.sw_getconfigutarion();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Authorization =
                    //    new AuthenticationHeaderValue("Bearer", _tkn);

                    //HttpResponseMessage response = client.GetAsync(this.sw_getconfigutarion()).Result;
                    //string responseBody = response.Content.ReadAsStringAsync().Result;
                    //var entidad = JsonConvert.DeserializeObject<result_bind>(responseBody);

                    //_o_user = entidad.o_usr;
                    //_o_pwd = entidad.o_pwd;

                }
            }
            catch (Exception ex)
            {
                _result = false;
                this.ErrorLog(ex);

            }

            return Json(new { token = _tkn, result = _result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("enviar-informacion")]
        public ActionResult EnviarFirma(HttpPostedFileBase rubriescaneada, HttpPostedFileBase certificadodigital)
        {

            var _cod_firma = Request.Params["cod_firma"];
            var _tipo_firma = Request.Params["tipo_firma"];
            var _razon_social = Request.Params["razon_social"];
            var _representante_legal = Request.Params["representante_legal"];
            var _empresa_acreditadora = Request.Params["empresa_acreditadora"];
            var _fecha_emision = Request.Params["fecha_emision"];
            var _fecha_vencimiento = Request.Params["fecha_vencimiento"];
            var _CertificadoDigital = string.Empty;
            string _tkn = this.TokenCredentials(); //_array_gdec[4];
            string str_rubiescaneada_base64 = string.Empty;
            if (rubriescaneada != null) {
                string theFileName = Path.GetFileName(rubriescaneada.FileName);
                byte[] thePictureAsBytes = new byte[rubriescaneada.ContentLength];
                using (BinaryReader theReader = new BinaryReader(rubriescaneada.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(rubriescaneada.ContentLength);
                }

                //se convierte la imagen en base 64
                str_rubiescaneada_base64 = Convert.ToBase64String(thePictureAsBytes);

            }

            //documento
            if (certificadodigital != null) {
                var fileName = certificadodigital.FileName;
                string _path = System.Configuration.ConfigurationManager.AppSettings["path_carpeta_clientes"].ToString();
                bool exists = System.IO.Directory.Exists(_path);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(_path);
                }

                string targetPath = _path + fileName;

                _CertificadoDigital = "~/examen/data/" + fileName;

                certificadodigital.SaveAs(targetPath);

            }

            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _tkn);

                var entidad = new firmadigital_bind();
                if (_cod_firma != "0") {
                    entidad.IdFirma = int.Parse(_cod_firma);
                }

                entidad.TipoFirma = _tipo_firma;
                entidad.RazonSocial = _razon_social;
                entidad.RepresentanteLegal = _representante_legal;
                entidad.EmpresaAcreditadora = _empresa_acreditadora;
                entidad.FechaEmision = _fecha_emision;
                entidad.FechaVencimiento = _fecha_vencimiento;
                entidad.RutaRubrica = str_rubiescaneada_base64;
                entidad.CertificadoDigital = _CertificadoDigital;

                string atsakas = "";
                try
                {
                    var myContent = JsonConvert.SerializeObject(entidad);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response;
                    if (_cod_firma == "0")
                    {
                        response = client.PostAsync(this.sw_guardar(), byteContent).Result;
                    }
                    else {
                        response = client.PutAsync(this.sw_modificar(), byteContent).Result;
                    }
                                            
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    atsakas = responseBody;
                }
                catch (Exception ex)
                {
                    this.ErrorLog(ex);
                    throw;
                }

            }

            RedirectToRouteResult _result = RedirectToAction("principal", "dashboard");
            return _result;
        }

        [ActionName("principal")]
        public ActionResult Principal()
        {
            string pagename = "certificadofirma/listado-js";
            ViewBag.CantAcord = "1";
            ViewBag.sw_listado = this.sw_listado();
            ViewBag.sw_guardar = this.sw_guardar();
            ViewBag.sw_getentidad = this.sw_getentidad();
            ViewBag.sw_modificar = this.sw_modificar();
            ViewBag.sw_eliminar = this.sw_eliminar();
            ViewBag.sw_listadocriterio = this.sw_listadocriterio();

            return View(pagename);
        }



        [ActionName("reporte-excel")]
        public ActionResult ReporteExcel(string supervisor)
        {
            try
            {
                string filename = "exportacion.xls";


                string _HTML = this.GenerarExcel(supervisor);

                //byte[] fileContents = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] fileContents = Encoding.UTF8.GetBytes(_HTML);
                return File(fileContents, "application/vnd.ms-excel", filename);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + ", " + ex.StackTrace.ToString());
                Response.End();
            }
            return View();

        }


        private string GenerarExcel(string supervisor)
        {
            string _HTML = string.Empty;
            try
            {

                string[] _array_gdec2 = this.MasterCredentials();
                string _tkn = this.TokenCredentials(); //_array_gdec2[4];

                _HTML += "<table>";
                _HTML += "<tr>";
                _HTML += "<td><b>Nombre/RazonSocial</b></td>";
                _HTML += "<td><b>Tipo Firma</b></td>";
                _HTML += "<td><b>Representante Legal</b></td>";
                _HTML += "<td><b>Empresa Acreditadora</b></td>";
                _HTML += "<td><b>Fecha Emision</b></td>";
                _HTML += "<td><b>Fecha Vencimiento</b></td>";
                _HTML += "</tr>";

                json_result_list<firmadigital_bind> EntidadContrato = new json_result_list<firmadigital_bind>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _tkn);

                    string _url = this.sw_listado_excel();

                    HttpResponseMessage response = client.GetAsync(_url).Result;
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    EntidadContrato = JsonConvert.DeserializeObject<json_result_list<firmadigital_bind>>(responseBody);


                    foreach (var item in EntidadContrato.result)
                    {
                        _HTML += "<tr>";
                        _HTML += "<td>" + item.RazonSocial + "</td>";
                        _HTML += "<td>" + item.TipoFirma + "</td>";
                        _HTML += "<td>" + item.RepresentanteLegal + "</td>";
                        _HTML += "<td>" + item.EmpresaAcreditadora + "</td>";
                        _HTML += "<td>" + item.FechaEmision + "</td>";
                        _HTML += "<td>" + item.FechaVencimiento + "</td>";
                        _HTML += "</tr>";
                    }
                }


                _HTML += "</table>";

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            return _HTML;
        }



    }
}