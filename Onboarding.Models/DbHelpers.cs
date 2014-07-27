using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace Onboarding.Models
{
    public class DbHelpers
    {
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
                where !d.State.Equals("Completed") &&
                !d.State.Equals("Canceled")
                select d;
        }

        /// <summary>
        /// Store ServiceType names into database. 
        /// To prevent duplication, do not add if the name already exists.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateServiceTypes(OnboardingDbContext db)
        {
            SystemHelpers.SyncDepot();
            foreach (var st in SystemHelpers.RetriveServiceTypeMap())
            {
                var entity = new ServiceType
                {
                    //ServiceTypeName = st.Key,
                    //ServiceTypeId = st.Value
                    ServiceTypeName = st
                };
                //if (db.ServiceTypes.Any(e => e.ServiceTypeId == entity.ServiceTypeId))
                if (db.ServiceTypes.Any(e => e.ServiceTypeName == entity.ServiceTypeName))
                {
                    db.ServiceTypes.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    db.ServiceTypes.Add(entity);
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Store TaskSetName-TaskSetId pair into database.
        /// To prevent duplication, do not add if the TaskSetId already exists.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateTaskSets(OnboardingDbContext db)
        {
            SystemHelpers.SyncDepot();
            foreach (var ts in SystemHelpers.RetrieveTaskSetMap())
            {
                var entity = new TaskSet
                {
                    TaskSetName = ts.Key,
                    TaskSetId = ts.Value
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
            db.SaveChanges();
        }

        /// <summary>
        /// Store ScopeName-ScopeId pair into database.
        /// To prevent duplication, do not add if the ScopeId already exists.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateScopes(OnboardingDbContext db)
        {
            SystemHelpers.SyncDepot();
            foreach (var s in SystemHelpers.RetrieveScopeMap())
            {
                var entity = new Scope
                {
                    ScopeName = s.Key,
                    ScopeId = s.Value
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
            db.SaveChanges();
        }

        /// <summary>
        /// Store name-description pair into database.
        /// Change source xml to update description at request creation page.
        /// </summary>
        /// <param name="db">An instance of <see cref="OnboardingDbContext"/>.</param>
        public static void AddOrUpdateDescriptions(OnboardingDbContext db)
        {
            SystemHelpers.SyncDepot();
            foreach (var d in SystemHelpers.RetrieveDescriptions())
            {
                var entity = new Description
                {
                    Name = d.Key,
                    Content = d.Value
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
            db.SaveChanges();
        }
    }
}