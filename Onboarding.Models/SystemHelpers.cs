using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Onboarding.Config;

namespace Onboarding.Models
{
    public static class SystemHelpers
    {
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
            return Encoding.Default.GetString(blob);
        }

        /// <summary>
        ///     Save .xml file to local disk, reading data from blob field of a request.
        /// </summary>
        /// <param name="onboardingRequest">The given request to be handled.</param>
        public static void SaveXmlToDisk(OnboardingRequest onboardingRequest)
        {
            var doc = new XmlDocument();
            doc.LoadXml(GenerateStringFromBlob(onboardingRequest.Blob));
            doc.Save(Constants.DepotPath + GenerateFilename(onboardingRequest));
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
            var startInfo = new ProcessStartInfo
            {
                FileName = Constants.CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = Constants.CmdConfigArgs
                                  + CmdSyncDepot() + " && exit"
            };
            var process = Process.Start(startInfo);
        }

        /// <summary>
        ///     Retrieve a list of ServiceTypes,.
        /// </summary>
        //public static Dictionary<string, string> RetriveServiceTypeMap()
        public static List<string>  RetriveServiceTypeMap()
        {
            //var serviceTypeMap = new Dictionary<string, string>();
            var serviceList = new List<string>();
            foreach (var file in Directory.EnumerateFiles(Constants.ProductCatalogPath, "*.xml"))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                var name = xmlDoc.GetElementsByTagName("ServiceType")[0];
                var id = xmlDoc.GetElementsByTagName("AppPrincipalID")[0];
                if (name != null && id != null)
                {
                    //serviceTypeMap.Add(name.InnerText, id.InnerText);
                    serviceList.Add(name.InnerText);
                }
            }
            return serviceList;
        }

        /// <summary>
        ///     Retrieve a list of TaskSets. 
        /// </summary>
        public static Dictionary<string, string> RetrieveTaskSetMap()
        {
            var taskSetMap = new Dictionary<string, string>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.RbacpolicyPath + Constants.TaskSetsFilename);
            var nameList = xmlDoc.GetElementsByTagName("DisplayName");
            var idList = xmlDoc.GetElementsByTagName("TaskSetId");
            if (nameList != null)
            {
                for (var i = 0; i < nameList.Count; i++)
                {
                    taskSetMap.Add(nameList[i].InnerText, idList[i].InnerText);
                }
            }
            return taskSetMap;
        }

        /// <summary>
        ///     Retrieve a list of Scopes. 
        /// </summary>
        public static Dictionary<string, string> RetrieveScopeMap()
        {
            var scopeMap = new Dictionary<string, string>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.RbacpolicyPath + Constants.ScopesFilename);
            var nameList = xmlDoc.GetElementsByTagName("DisplayName");
            var idList = xmlDoc.GetElementsByTagName("ScopeId");
            if (nameList != null)
            {
                for (var i = 0; i < nameList.Count; i++)
                {
                    scopeMap.Add(nameList[i].InnerText, idList[i].InnerText);
                }
            }
            return scopeMap;
        }

        /// <summary>
        ///     Retrieve a list of descripstions.
        /// </summary>
        public static Dictionary<string, string> RetrieveDescriptions()
        {
            var descMap = new Dictionary<string, string>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.DescriptionFilePath);
            var list = xmlDoc.GetElementsByTagName("Descriptions")[0].ChildNodes;
            for (var i = 0; i < list.Count; i++)
            {
                descMap.Add(list[i].Name, list[i].InnerText);
            }
            return descMap;
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