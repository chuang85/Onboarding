using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Onboarding.Utils
{
    public static class SystemHelpers
    {
        private const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";
        private const string CmdConfigArgs = @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";
        private const string SourcePath = @"C:\Users\t-chehu\Source\Repos\Onboarding\Onboarding\App_Data\";
        public const string DestPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        private const string SavingPathXml = @"../../App_Data/";

        /// <summary>
        /// Write string into an xml file and then optionally save it.
        /// </summary>
        /// <param name="xmlString">Xml in format of string, to be written into file.</param>
        /// <param name="path">The saving path.</param>
        /// <param name="filename">Name of file</param>
        /// <returns>The xml in type of byte[]</returns>
        public static byte[] SavesStringToXml(string xmlString, string filename)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            xdoc.Save(HttpContext.Current.Server.MapPath(SavingPathXml + filename));
            return Encoding.Default.GetBytes(xdoc.OuterXml);
        }

        public static void RunCmd(string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(CmdPath);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = CmdConfigArgs
                + CmdCopyArgs(SourcePath, DestPath, filename)
                + CmdAddToDepotArgs(DestPath, filename);
            Process process = Process.Start(startInfo);
        }

        private static string CmdCopyArgs(string source, string dest, string filename)
        {
            return " && copy " + source + filename + " " + dest;
        }

        private static string CmdAddToDepotArgs(string depotPath, string filename)
        {
            return " && cd " + depotPath + " && sd add " + filename;
        }
    }
}