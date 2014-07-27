using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Config
{
    public static class Constants
    {
        public const string ProjectShortName = "MSODS";
        public const string EmailDomain = "@microsoft.com";
        public const string CmdPath = @"C:\WINDOWS\system32\cmd.exe";
        public const string CmdConfigArgs =
            @"/k set inetroot=e:\cumulus_main&set corextbranch=main&e:\cumulus_main\tools\path1st\myenv.cmd";
        public const string DepotPath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\";
        public const string ProductCatalogPath = @"E:\CUMULUS_MAIN\sources\dev\ds\content\productcatalog\";
        public const string RbacpolicyPath = @"E:\CUMULUS_MAIN\sources\dev\ds\content\rbacpolicy\";
        public const string TaskSetsFilename = "TasksSets.xml";
        public const string ScopesFilename = "Scopes.xml";
        public const string DescriptionFilePath = @"E:\CUMULUS_MAIN\sources\dev\RestServices\GraphService\Tools\descriptions.xml";
        public const string AppDataPathXml = @"../../App_Data/";
        public const string SecurityGroup = @"";
    }
}
