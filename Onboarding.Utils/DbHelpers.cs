using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using log4net;
using Onboarding.Config;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class DbHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get all the requests that are not in state Completed or Canceled.
        /// So that can handle each request in the list
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        /// <returns>Requests that are not in state Completed or Canceled.</returns>
        public static IQueryable<OnboardingRequest> UncompletedRequests(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
                where d.State != RequestState.Completed &&
                d.State != RequestState.Canceled
                select d;
        }

        /// <summary>
        /// Store TaskSetName-TaskSetId pair into database.
        /// To prevent duplication, do not add if the TaskSetId already exists.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateTaskSets(OnboardingDbContext db)
        {
            Logger.Debug("Updating TaskSets by retrieving from source depot...");
            SystemHelpers.SyncDepot();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.RbacpolicyPath + Constants.TaskSetsFilename);
            var nameList = xmlDoc.GetElementsByTagName("DisplayName");
            var idList = xmlDoc.GetElementsByTagName("TaskSetId");
            if (nameList != null)
            {
                for (var i = 0; i < nameList.Count; i++)
                {
                    var entity = new TaskSet
                    {
                        TaskSetName = nameList[i].InnerText,
                        TaskSetId = idList[i].InnerText
                    };
                    if (db.TaskSets.Any(e => e.TaskSetId == entity.TaskSetId))
                    {
                        db.TaskSets.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        db.TaskSets.Add(entity);
                    }
                }
            }
            var num = db.SaveChanges();
            Logger.Debug(num + " TaskSets have been updated in database");
        }

        /// <summary>
        /// Store ScopeName-ScopeId pair into database.
        /// To prevent duplication, do not add if the ScopeId already exists.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateScopes(OnboardingDbContext db)
        {
            Logger.Debug("Updating Scopes by retrieving from source depot...");
            SystemHelpers.SyncDepot();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.RbacpolicyPath + Constants.ScopesFilename);
            var nameList = xmlDoc.GetElementsByTagName("DisplayName");
            var idList = xmlDoc.GetElementsByTagName("ScopeId");
            if (nameList != null)
            {
                for (var i = 0; i < nameList.Count; i++)
                {
                    var entity = new Scope
                    {
                        ScopeName = nameList[i].InnerText,
                        ScopeId = idList[i].InnerText
                    };
                    if (db.Scopes.Any(e => e.ScopeId == entity.ScopeId))
                    {
                        db.Scopes.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Scopes.Add(entity);
                    }
                }
            }
            var num = db.SaveChanges();
            Logger.Debug(num + " Scopes have been updated in database");
        }

        /// <summary>
        /// Store name-description pair into database.
        /// Change source xml to update description at request creation page.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateDescriptions(OnboardingDbContext db)
        {
            Logger.Debug("Updating descriptions by retrieving from source depot...");
            SystemHelpers.SyncDepot();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.DescriptionFilePath);
            var list = xmlDoc.GetElementsByTagName("Descriptions")[0].ChildNodes;
            for (var i = 0; i < list.Count; i++)
            {
                var entity = new Description
                {
                    Name = list[i].Name,
                    Content = list[i].InnerText
                };
                if (db.Descriptions.Any(e => e.Name == entity.Name))
                {
                    db.Descriptions.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    db.Descriptions.Add(entity);
                }
            }
            var num = db.SaveChanges();
            Logger.Debug(num + " descriptions have been updated in database");
        }

        /// <summary>
        /// Store filename-xml pair into database.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateExistingSpts(OnboardingDbContext db)
        {
            Logger.Debug("Updating ExistingSpts by retrieving from source depot...");
            SystemHelpers.SyncDepot();
            foreach (var file in Directory.EnumerateFiles(Constants.ProductCatalogPath, "*.xml"))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                var dirObjs = xmlDoc.GetElementsByTagName("DirectoryChanges")[0].ChildNodes;
                XmlNode targetObj = null;
                for (var i = 0; i < dirObjs.Count; i++)
                {
                    if (XmlHelpers.IsValidSpt(dirObjs[i].OuterXml))
                    {
                        targetObj = dirObjs[i];
                    }

                }
                if (targetObj != null)
                {
                    var tempDoc = new XmlDocument();
                    tempDoc.LoadXml(targetObj.OuterXml);
                    var entity = new ExistingSpt
                    {
                        Name = XmlHelpers.GetFileNameFromPath(file),
                        XmlContent = targetObj.OuterXml,
                        ServiceType = tempDoc.GetElementsByTagName("ServiceType")[0].InnerText
                    };
                    if (db.ExistingSpts.Any(e => e.Name == entity.Name))
                    {
                        db.ExistingSpts.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        db.ExistingSpts.Add(entity);
                    }
                }
            }
            var num = db.SaveChanges();
            Logger.Debug(num + " ExistingSpts have been updated in database");
        }
    }
}