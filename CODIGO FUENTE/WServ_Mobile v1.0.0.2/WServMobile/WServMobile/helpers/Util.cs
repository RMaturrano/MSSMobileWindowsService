using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WServMobile.helpers
{
    public class Util
    {
        public static string castURL(string url, string finalChar)
        {
            string finalUrl = url;
            try
            {
                if (url.Length > 0)
                {
                    string lastChar = url.Substring(url.Length - 1);
                    if (!lastChar.Equals(finalChar))
                        finalUrl += finalChar;
                }
            }
            catch (Exception ex)
            {
                MainProcess.log.Error("Util > castURL() > " + ex.Message);
            }

            return finalUrl;
        }

        public static string replaceEscChar(string data)
        {
            string newData = data;

            if (newData.Length > 0)
            {
                newData = newData.Replace("\"", "");
                newData = newData.Replace(@"\", "");
                newData = newData.Replace("\r\\", "");
                newData = newData.Replace("\\", "");
                newData = newData.Replace("{", "");
                newData = newData.Replace("}", "");
                newData = newData.Replace("[", "");
                newData = newData.Replace("]", "");
                newData = Regex.Replace(newData, @"\t|\n|\r", "");
            }

            return newData.Trim();
        }
    }
}
