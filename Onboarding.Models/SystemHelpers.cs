using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using log4net;
using Onboarding.Config;
using Onboarding.Models;

namespace Onboarding.Models
{
    public static class SystemHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Write string into binary array.
        /// </summary>
        /// <param name="xmlString">Xml in format of string.</param>
        /// <returns>The xml in type of byte[]</returns>
        public static byte[] GenerateBlobFromString(string xmlString)
        {
            return Encoding.Default.GetBytes(xmlString);
        }

        /// <summary>
        ///     Convert binary data back to string format.
        /// </summary>
        /// <param name="blob">Xml in format of byte array.</param>
        /// <returns>The xml in string format</returns>
        public static string GenerateStringFromBlob(byte[] blob)
        {
            if (blob != null)
            {
                return Encoding.Default.GetString(blob);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Save .xml file to local disk, reading data from blob field of a request.
        /// </summary>
        /// <param name="onboardingRequest">The given request to be handled.</param>
        public static void SaveXmlToDisk(OnboardingRequest onboardingRequest)
        {
            if (onboardingRequest.Blob != null)
            {
                var doc = new XmlDocument();
                doc.LoadXml(GenerateStringFromBlob(onboardingRequest.Blob));
                doc.Save(Constants.DepotPath + GenerateFilename(onboardingRequest));
            }
        }

        /// <summary>
        ///     Copy SPT xml from App_Data to source depot destination path and run "sd add"
        ///     Create dpk file through a changespec so that editor won't popup
        /// </summary>
        /// <param name="filename">SPT xml file's name.</param>
        public static void AddFileToDepotAndPack(string filename)
        {
            var changeSpecName = CreateChangeSpec(filename);
            var startInfo = new ProcessStartInfo
            {
                FileName = Constants.CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Constants.CmdConfigArgs
                                  + CmdAddToDepotArgs(filename, changeSpecName) + " && exit"
            };
            var process = Process.Start(startInfo);
            DeleteChangeSpec(filename, changeSpecName);
        }

        /// <summary>
        ///     Revert the file from changelist.
        /// </summary>
        /// <param name="filename">SPT xml file's name.</param>
        public static void RevertFile(string filename)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Constants.CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Constants.CmdConfigArgs
                                  + CmdRevertFile(filename) + " && exit"
            };
            var process = Process.Start(startInfo);
        }

        /// <summary>
        ///     Keep the depot up-to-date, before retrieve the ServiceType list.
        /// </summary>
        public static void SyncDepot()
        {
            Logger.Debug("Syncing source depot...");
            var startInfo = new ProcessStartInfo
            {
                FileName = Constants.CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Constants.CmdConfigArgs
                                  + CmdSyncDepot() + " && exit"
            };
            var process = Process.Start(startInfo);
        }

        public static string GenerateReivewName(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.Type + "-RequestId-" + onboardingRequest.RequestId;
        }

        public static string GenerateFilename(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.RequestId + "_" + NameWithoutDomain(onboardingRequest.CreatedBy) + ".xml";
        }

        private static string CmdRevertFile(string filename)
        {
            return " && cd " + Constants.DepotPath + " && sd revert -d " + filename;
        }

        private static string CmdAddToDepotArgs(string filename, string changeSpecName)
        {
            return " && cd " + Constants.DepotPath + " && sd add " + filename + " && sdp pack " + filename + " -I " + changeSpecName;
        }

        private static string CmdSyncDepot()
        {
            return " && cd " + Constants.DepotPath + " && sd sync";
        }

        private static string CreateChangeSpec(string sourceFilename)
        {
            var changeSpecName = Guid.NewGuid() + ".txt";
            string[] lines =
            {
                "Description:some desc",
                "Files:" + Constants.DepotPath + sourceFilename + " # edit"
            };
            File.WriteAllLines(Constants.DepotPath + changeSpecName, lines);
            return changeSpecName;
        }

        private static void DeleteChangeSpec(string dpkfilename, string changeSpecName)
        {
            while (true)
            {
                if (File.Exists(Constants.DepotPath + changeSpecName) &&
                    File.Exists(Constants.DepotPath + dpkfilename + ".dpk"))
                {
                    File.Delete(Constants.DepotPath + changeSpecName);
                    break;
                }
            }
        }

        private static string NameWithoutDomain(string raw)
        {
            var tokens = raw.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2)
                throw new ArgumentException(raw);
            return tokens[1];
        }
    }
}