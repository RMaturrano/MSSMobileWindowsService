using System;
using System.Drawing;
using System.IO;


namespace WServMobile.dao
{
    public class DirectoryDAO
    {
        public static void createDirectoryLocal(string path)
        {
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

        public static void createFileDirectoryLocal(string base64,string ruta)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                MemoryStream ms = new MemoryStream(imageBytes, 0,
                imageBytes.Length);

                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                image.Save(@ruta);
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("DirectoryDAO > createFileDirectoryLocal() > " + ex.Message);
            }
        }
    }
}
