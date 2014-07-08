using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;

namespace Onboarding.Utils
{
    public static class SystemHelpers
    {
        public const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";
        public const string CmdConfigArgs = @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";
        public const string DepotPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        public const string AppDataPathXml = @"../../App_Data/";

        /// <summary>
        /// Write string into an xml file and then save it
        /// </summary>
        /// <param name="xmlString">Xml in format of string, to be written into file.</param>
        /// <param name="filename">Name of file</param>
        /// <returns>The xml in type of byte[]</returns>
        public static byte[] SaveStringToXml(string xmlString, string filename)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            xdoc.Save(HttpContext.Current.Server.MapPath(AppDataPathXml + filename));
            xdoc.Save(DepotPath + filename);
            return Encoding.Default.GetBytes(xdoc.OuterXml);
        }

        /// <summary>
        /// Copy SPT xml from App_Data to source depot destination path and run "sd add"
        /// </summary>
        /// <param name="filename">SPT xml file's name.</param>
        public static void AddFileToDepot(string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(CmdPath);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = CmdConfigArgs
                + CmdAddToDepotArgs(DepotPath, filename);
            Process process = Process.Start(startInfo);
        }

        private static string CmdAddToDepotArgs(string depotPath, string filename)
        {
            return " && cd " + depotPath + " && sd add " + filename;
        }
    }
}