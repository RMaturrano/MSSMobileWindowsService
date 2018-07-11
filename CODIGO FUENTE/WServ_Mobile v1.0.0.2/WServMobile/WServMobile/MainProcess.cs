using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WServMobile.connection;
using WServMobile.dao;
using WServMobile.entity;

namespace WServMobile
{
    public class MainProcess
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Connection mConn;

        public void ejecutarProcesos()
        {
            try
            {
                mConn = Connection.Instance;
                if (mConn.datosValidos)
                {
                    DirectoryDAO.createDirectoryLocal(mConn.pathJSONLog);
                    List<CompanyBean> sociedades = CompanyDAO.obtenerSociedades(mConn.urlGetEmpresa);

                    if (sociedades.Count > 0)
                    {
                        foreach (var sociedad in sociedades)
                        {
                            SocioNegocio.registrarSociosEnSAP(sociedad);
                            OrdenVenta.registrarOrdenesEnSAP(sociedad);
                            PagoRecibido.registrarPagosEnSAP(sociedad);
                            Incidencia.registrarIncidenciasEnSAP(sociedad);
                            Devolucion.registrarDevolucionesEnSAP(sociedad);
                            NotaCredito.registrarNotasCreditoEnSAP(sociedad);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("MainProcess > " + e.Message);
            }
        }
    }
}
