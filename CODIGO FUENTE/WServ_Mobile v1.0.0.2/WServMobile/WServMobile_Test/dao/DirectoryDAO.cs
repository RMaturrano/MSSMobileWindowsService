using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WServMobile_Test.dao
{
    public class DirectoryDAO
    {
        public static void createDirectoryLocal(string path){
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("DirectoryDAO > createDirectoryLocal() > " + ex.Message);        
            }
        }
    }
}
