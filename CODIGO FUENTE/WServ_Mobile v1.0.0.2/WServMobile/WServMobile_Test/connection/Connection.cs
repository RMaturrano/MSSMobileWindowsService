using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WServMobile_Test.entity;
using WServMobile_Test.helpers;

namespace WServMobile_Test.connection
{
    public class Connection
    {
        private string FILE_CONFIG = "\\conexion.xml";
        private static readonly Lazy<Connection> mConnection = new Lazy<Connection>(() => new Connection());

        private Connection()
        {
            initialize();
        }

        public static Connection Instance
        {
            get
            {
                return mConnection.Value;
            }
        }

        private void initialize()
        {
            try
            {
                XDocument connectionXML = XDocument.Load(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + FILE_CONFIG));

                var xmlURLs = (from op in connectionXML.Descendants("HANA")
                                select new
                                {
                                    BaseUrl = op.Element("BaseUrl").Value,
                                    urlServiceLayer = op.Element("UrlServiceLayer").Value,

                                    urlGetEmpresa = op.Element("GetEmpresa").Value,
                                    urlGetOrdenVenta = op.Element("GetOrdenVenta").Value,
                                    urlGetPagoRecibido = op.Element("GetPagoRecibido").Value,
                                    urlGetSocioNegocio = op.Element("GetSocioNegocio").Value,
                                    urlGetIncidencia = op.Element("GetIncidencia").Value,
                                    urlGetUbicaciones = op.Element("GetUbicaciones").Value,
                                    urlGetDevolucion = op.Element("GetDevolucion").Value,
                                    urlGetNotaCredito = op.Element("GetNotaCredito").Value,

                                    urlPatchOrdenVenta = op.Element("PatchOrdenVenta").Value,
                                    urlPatchPagoRecibido = op.Element("PatchPagoRecibido").Value,
                                    urlPatchSocioNegocio = op.Element("PatchSocioNegocio").Value,
                                    urlPatchIncidencia = op.Element("PatchIncidencia").Value,
                                    urlPatchUbicacion = op.Element("PatchUbicacion").Value,
                                    urlPatchDevolucion = op.Element("PatchDevolucion").Value,
                                    urlPatchNotaCredito = op.Element("PatchNotaCredito").Value,

                                    urlValidarOV = op.Element("ValidarOrden").Value,
                                    urlValidarPR = op.Element("ValidarPago").Value,
                                    urlValidarBP = op.Element("ValidarSocio").Value,
                                    urlValidarAC = op.Element("ValidarIncidencia").Value,
                                    urlValidarRT = op.Element("ValidarDevolucion").Value,
                                    urlValidarNC = op.Element("ValidarNotaCredito").Value,

                                    jsonLog = op.Element("Json").Value
                                }).Single();

                this.BaseUrl = xmlURLs.BaseUrl;
                this.urlServiceLayer = xmlURLs.urlServiceLayer;

                this.urlGetEmpresa = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetEmpresa;
                this.urlGetOrdenVenta = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetOrdenVenta;
                this.urlGetPagoRecibido = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetPagoRecibido;
                this.urlGetSocioNegocio = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetSocioNegocio;
                this.urlGetIncidencia = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetIncidencia;
                this.urlGetUbicaciones = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetUbicaciones;
                this.urlGetDevolucion = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetDevolucion;
                this.urlGetNotaCredito = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlGetNotaCredito;

                this.urlPatchOrdenVenta = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchOrdenVenta;
                this.urlPatchPagoRecibido = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchPagoRecibido;
                this.urlPatchSocioNegocio = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchSocioNegocio;
                this.urlPatchIncidencia = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchIncidencia;
                this.urlPatchUbicacion = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchUbicacion;
                this.urlPatchDevolucion = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchDevolucion;
                this.urlPatchNotaCredito = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlPatchNotaCredito;

                this.pathJSONLog = xmlURLs.jsonLog;
                this.urlValidarOrdenVenta = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarOV;
                this.urlValidarPagoRecibido = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarPR;
                this.urlValidarSocioNegocio = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarBP;
                this.urlValidarIncidencia = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarAC;
                this.urlValidarDevolucion = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarRT;
                this.urlValidarNotaCredito = Util.castURL(this.BaseUrl, "/") + xmlURLs.urlValidarNC;

                this.datosValidos = true;
            }
            catch (Exception ex)
            {
                this.datosValidos = false;
                MainProcess.log.Error("Error en archivo de conexión", ex);
            }
        }

        public bool datosValidos { get; set; }
        public string BaseUrl { get; set; }
        public string urlServiceLayer { get; set; }
        
        public string urlGetEmpresa { get; set; }
        public string urlGetOrdenVenta { get; set; }
        public string urlGetPagoRecibido { get; set; }
        public string urlGetSocioNegocio { get; set; }
        public string urlGetIncidencia { get; set; }
        public string urlGetUbicaciones { get; set; }
        public string urlGetDevolucion { get; set; }
        public string urlGetNotaCredito { get; set; }

        public string urlPatchOrdenVenta { get; set; }
        public string urlPatchPagoRecibido { get; set; }
        public string urlPatchSocioNegocio { get; set; }
        public string urlPatchIncidencia { get; set; }
        public string urlPatchUbicacion { get; set; }
        public string urlPatchDevolucion { get; set; }
        public string urlPatchNotaCredito { get; set; }
        
        public string urlValidarOrdenVenta { get; set; }
        public string urlValidarPagoRecibido { get; set; }
        public string urlValidarSocioNegocio { get; set; }
        public string urlValidarIncidencia { get; set; }
        public string urlValidarDevolucion { get; set; }
        public string urlValidarNotaCredito { get; set; }
        
        public string pathJSONLog { get; set; }
    }
}
