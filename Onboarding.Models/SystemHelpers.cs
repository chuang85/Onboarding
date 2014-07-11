using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;

namespace Onboarding.Models
{
    public static class SystemHelpers
    {
        public const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";

        public const string CmdConfigArgs =
            @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";

        public const string DepotPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        public const string AppDataPathXml = @"../../App_Data/";

        /// <summary>
        ///     Write string into an xml file and then save it
        /// </summary>
        /// <param name="xmlString">Xml in format of string, to be written into file.</param>
        /// <param name="filename">Name of file</param>
        /// <returns>The xml in type of byte[]</returns>
        public static byte[] SaveStringToXml(string xmlString, string filename)
        {
            var xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            xdoc.Save(HttpContext.Current.Server.MapPath(AppDataPathXml + filename));
            xdoc.Save(DepotPath + filename);
            return Encoding.Default.GetBytes(xdoc.OuterXml);
        }

        /// <summary>
        ///     Copy SPT xml from App_Data to source depot destination path and run "sd add"
        /// </summary>
        /// <param name="filename">SPT xml file's name.</param>
        public static void AddFileToDepot(string filename)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = CmdConfigArgs
                                  + CmdAddToDepotArgs(filename)
            };
            Process.Start(startInfo);
        }

        /// <summary>
        ///     Revert the file from changelist.
        /// </summary>
        /// <param name="filename">SPT xml file's name.</param>
        public static void RevertFile(string filename)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = CmdConfigArgs
                                  + CmdRevertFile(filename)
            };
            Process.Start(startInfo);
        }

        private static string CmdRevertFile(string filename)
        {
            return " && cd " + DepotPath + " && sd revert -d " + filename;
        }

        private static string CmdAddToDepotArgs(string filename)
        {
            return " && cd " + DepotPath + " && sd add " + filename + " && sdp pack " + filename + " -C \"some desc\"";
        }
    }
}