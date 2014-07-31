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
    public static class MembershipCheckHelpers
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

        /// <summary>
        /// Check if the reviewer is in the security group of requestor.
        /// </summary>
        /// <param name="reviewer">Alias of reviewer</param>
        /// <param name="requestorGroup">Requestor's group</param>
        /// <returns></returns>
        public static bool IsInSecurityGroup(string reviewer, string requestorGroup)
        {
            var userTokens = ParseDomainQualifiedName(reviewer, "user");
            using (var userContext = new PrincipalContext(ContextType.Domain, userTokens[0]))
            {
                using (
                    var identity = UserPrincipal.FindByIdentity(userContext, IdentityType.SamAccountName, userTokens[1])
                    )
                {
                    if (identity != null)
                    {
                        var groupTokens = ParseDomainQualifiedName(requestorGroup, "group");
                        using (var groupContext = new PrincipalContext(ContextType.Domain, groupTokens[0]))
                        {
                            using (
                                var identity2 = GroupPrincipal.FindByIdentity(groupContext, IdentityType.SamAccountName,
                                    groupTokens[1]))
                            {
                                if (identity2 != null)
                                {
                                    return identity.IsMemberOf(identity2);
                                }
                            }
                        }
                    }
                }
            }
            return true;
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
