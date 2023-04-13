using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Authentication.Application.AzureADB2C.Model;

namespace Transversals.Business.Authentication.Application.AzureADB2C
{
    public static class AzureExtensions
    {
        public static UserB2C? ToUserB2C(this User user)
        {
            var u = user?.Identities?.FirstOrDefault(i => i.SignInType == "emailAddress");
            if (user == null || u == null)
                return null;

            var userB2C = new UserB2C()
            {
                LastName = user.Surname,
                FirstName = user.GivenName,
                Email = u.IssuerAssignedId
            };
            return userB2C;
        }

        public static List<UserAuthentication> ToUserAuthenticationList(this List<UserB2C> users)
        {
            List<UserAuthentication> userAuthentications = new List<UserAuthentication>();
            foreach (var user in users)
            {
                var u = new UserAuthentication
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Name = $"{user.FirstName} {user.LastName}"
                };
                userAuthentications.Add(u);
            }
            return userAuthentications;
        }
    }
}
