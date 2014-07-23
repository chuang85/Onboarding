using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices;

namespace Onboarding.Utils
{
    public static class MembershipCheckHelper
    {
        /// <summary>
        /// Get the full display name of a given user.
        /// </summary>
        /// <param name="user">In our case, it should be an alias.</param>
        /// <returns>Displayname for the given alias.</returns>
        public static string GetName(string user)
        {
            var userTokens = ParseDomainQualifiedName(user, "user");
            using (var userContext = new PrincipalContext(ContextType.Domain, userTokens[0]))
            {
                using (var identity = UserPrincipal.FindByIdentity(userContext, IdentityType.SamAccountName,
                        userTokens[1]))
                {
                    if (identity != null)
                    {
                        return identity.DisplayName;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the email address of a given user.
        /// </summary>
        /// <param name="user">In our case, it should be an alias.</param>
        /// <returns>Email address for the given alias.</returns>
        public static string GetEmailAddress(string user)
        {
            var userTokens = ParseDomainQualifiedName(user, "user");
            using (var userContext = new PrincipalContext(ContextType.Domain, userTokens[0]))
            {
                using (var identity = UserPrincipal.FindByIdentity(userContext, IdentityType.SamAccountName,
                        userTokens[1]))
                {
                    if (identity != null)
                    {
                        return identity.EmailAddress;
                    }
                }
            }
            return null;
        }

        private static string[] ParseDomainQualifiedName(string name, string parameterName)
        {
            var groupTokens = name.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (groupTokens.Length < 2)
                throw new ArgumentException(name, parameterName);
            return groupTokens;
        }
    }
}
