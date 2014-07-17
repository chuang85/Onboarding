using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Onboarding.Models
{
    public static class SystemHelpers
    {
        public const string ProjectShortName = "MSODS";
        private const string EmailDomain = "@microsoft.com";
        private const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";
        private const string CmdConfigArgs =
            @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";
        public const string DepotPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        private const string ProductCatalogPath = @"E:\CUMULUS_MAIN\sources\dev\ds\content\productcatalog\";
        private const string RbacpolicyPath = @"E:\CUMULUS_MAIN\sources\dev\ds\content\rbacpolicy\";
        private const string TaskSetsFilename = "TasksSets.xml";
        private const string ScopesFilename = "Scopes.xml";

        private const string AppDataPathXml = @"../../App_Data/";

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
            doc.Save(DepotPath + GenerateFilename(onboardingRequest));
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
                FileName = CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = CmdConfigArgs
                                  + CmdAddToDepotArgs(filename, changeSpecName)
            };
            var process = Process.Start(startInfo);
            DeleteChangeSpec(changeSpecName);
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
            var process = Process.Start(startInfo);
        }

        /// <summary>
        ///     Keep the depot up-to-date, before retrieve the ServiceType list.
        /// </summary>
        public static void SyncDepot()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = CmdPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = CmdConfigArgs
                                  + CmdSyncDepot()
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
            foreach (var file in Directory.EnumerateFiles(ProductCatalogPath, "*.xml"))
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
            xmlDoc.Load(RbacpolicyPath + TaskSetsFilename);
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
            xmlDoc.Load(RbacpolicyPath + ScopesFilename);
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

        public static string GenerateReivewName(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.Type + "-RequestId-" + onboardingRequest.RequestId;
        }

        public static string GenerateEmailAddress(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.CreatedBy + EmailDomain;
        }

        public static string GenerateFilename(OnboardingRequest onboardingRequest)
        {
            return onboardingRequest.DisplayName + "_" + onboardingRequest.CreatedBy + ".xml";
        }

        private static string CmdRevertFile(string filename)
        {
            return " && cd " + DepotPath + " && sd revert -d " + filename;
        }

        private static string CmdAddToDepotArgs(string filename, string changeSpecName)
        {
            return " && cd " + DepotPath + " && sd add " + filename + " && sdp pack " + filename + " -I " + changeSpecName;
        }

        private static string CmdSyncDepot()
        {
            return " && cd " + DepotPath + " && sd sync";
        }

        private static string CreateChangeSpec(string sourceFilename)
        {
            var changeSpecName = Guid.NewGuid() + ".txt";
            string[] lines =
            {
                "Description:some desc",
                "Files:" + DepotPath + sourceFilename + " # edit"
            };
            File.WriteAllLines(DepotPath + changeSpecName, lines);
            return changeSpecName;
        }

        private static void DeleteChangeSpec(string changeSpecName)
        {
            File.Delete(changeSpecName);
        }
    }
}