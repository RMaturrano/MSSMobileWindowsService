namespace WServMobile
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new Main() 
            //};
            //ServiceBase.Run(ServicesToRun);
            while (true) { new MainProcess().ejecutarProcesos(); }
        }
    }
}
