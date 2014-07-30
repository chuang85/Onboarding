using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Onboarding.Config;

namespace Onboarding.Utils
{
    public class XmlHelpers
    {
        /// <summary>
        /// Get the filename from a fullpath.
        /// e.g. 'somedir\somename.xml' => 'somename'
        /// </summary>
        /// <param name="fullpath">Fullpath of the file.</param>
        /// <returns>Only the filename</returns>
        public static string GetFileNameFromPath(string fullpath)
        {
            var strArr = fullpath.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            return strArr[strArr.Length - 1].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        /// <summary>
        /// Check if the given string in valid xml format.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument container.</param>
        /// <param name="input">String to be checked.</param>
        /// <returns>True if is valid xml.</returns>
        public static bool IsValidXml(XmlDocument xmlDoc, string input)
        {
            try
            {
                xmlDoc.LoadXml(input);
                return true;
            }
            catch (XmlException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the given xml is a valid Spt.
        /// A valid spt should have attribute "type=service".
        /// </summary>
        /// <param name="source">Xml to be checked.</param>
        /// <returns>True if is valid SPT.</returns>
        public static bool IsValidSpt(string source)
        {
            var xmlDoc = new XmlDocument();
            if (IsValidXml(xmlDoc, source))
            {
                var obj = xmlDoc.GetElementsByTagName("DirectoryObject");
                var sptTag = xmlDoc.GetElementsByTagName("ServicePrincipalTemplate");
                if (sptTag[0] != null && obj[0] != null && obj[0].Attributes != null)
                {
                    if (obj[0].Attributes["xsi:type"] != null && obj[0].Attributes["xsi:type"].Value.Equals("Service"))
                    {
                        return true;
                    }
                    if (obj[0].Attributes["p2:type"] != null && obj[0].Attributes["p2:type"].Value.Equals("Service"))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
    }
}
