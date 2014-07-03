using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Onboarding.Utils
{
    public static class SystemHelpers
    {
        /// <summary>
        /// Write string into an xml file and then optionally save it.
        /// </summary>
        /// <param name="xmlString">Xml in format of string, to be written into file.</param>
        /// <param name="path">The saving path.</param>
        /// <param name="filename">Name of file</param>
        /// <returns>The xml in type of byte[]</returns>
        public static byte[] SavesStringToXml(string xmlString, string path, string filename)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            xdoc.Save(HttpContext.Current.Server.MapPath(path + filename));
            return Encoding.Default.GetBytes(xdoc.OuterXml);
        }

        public static string CmdCopyArgs(string source, string dest, string filename)
        {
            return " && copy " + source + filename + " " + dest;
        }

        public static string CmdAddToDepotArgs(string depotPath, string filename)
        {
            return " && cd " + depotPath + " && sd add " + filename;
        }
    }
}