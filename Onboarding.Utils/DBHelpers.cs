using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using Onboarding.Models;

namespace Onboarding.Utils
{
    public class DbHelpers
    {
        public static IQueryable<OnboardingRequest> UncompletedRequests(OnboardingDbContext db)
        {
            return
                from d in db.OnboardingRequests
                where d.Type.Equals("CreateSPT") && 
                !d.State.Equals("Completed")
                select d;
        }

        public static void AddOrUpdateServiceTypes(OnboardingDbContext db)
        {
            SystemHelpers.SyncDepot();
            foreach (var st in SystemHelpers.RetriveServiceTypeMap())
            {
                var entity = new ServiceType
                {
                    ServiceTypeName = st.Key,
                    ServiceTypeId = st.Value
                };
                if (db.ServiceTypes.Any(e => e.ServiceTypeId == entity.ServiceTypeId))
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
    }
}