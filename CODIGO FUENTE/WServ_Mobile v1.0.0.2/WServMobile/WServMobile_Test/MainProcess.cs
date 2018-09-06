using System.Collections.Generic;
using WServMobile_Test.connection;
using WServMobile_Test.dao;
using WServMobile_Test.entity;

namespace WServMobile_Test
{
    public class MainProcess
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Connection mConn;

        public void ejecutarProcesos()
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
                        //  OrdenVenta.registrarOrdenesEnSAP(sociedad);
                        // PagoRecibido.registrarPagosEnSAP(sociedad);
                        // SocioNegocio.registrarSociosEnSAP(sociedad);
                        // PagoRecibido.registrarPagosEnSAP(sociedad);
                        //Incidencia.registrarIncidenciasEnSAP(sociedad);
                        //Ubicaciones.actualizarUbicacionesEnSAP(sociedad);
                        //Devolucion.registrarDevolucionesEnSAP(sociedad);
                        if (sociedad.id == 2)
                            NotaCredito.registrarNotasCreditoEnSAP(sociedad);

                    }
                }
            }
        }
    }
}
