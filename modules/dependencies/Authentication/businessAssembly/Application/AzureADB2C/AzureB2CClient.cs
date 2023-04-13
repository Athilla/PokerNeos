using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Authentication.Application.AuthenticationNeos;
using Transversals.Business.Authentication.Application.AzureADB2C;
using Transversals.Business.Authentication.Application.AzureADB2C.Model;
using Transversals.Business.Authentication.Application.Factory.Abstractions;
using Transversals.Business.Authentication.Application.Services;

namespace Transversals.Business.Authentication.Domain.AzureADB2C
{
    /// <summary>
    /// Class AzureB2CClient, it used for managed data into Azure ad b2c tenant.
    /// </summary>
    /// <seealso cref="Transversals.Business.Authentication.Application.Factory.Abstractions.IAuthProvider" />
    public class AzureB2CClient : IAuthProvider
    {
        private readonly ILogger<AzureB2CClient> _logger;
        private readonly IGraphService _graphService;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureB2CClient"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="emailService">The email service.</param>
        public AzureB2CClient(ILogger<AzureB2CClient> logger,
            IGraphService graphService,
            IEmailService emailService)
        {
            _logger = logger;
            _graphService = graphService;
            _emailService = emailService;
        }

        public async Task<UserAuthenticationResult[]> RegisterUsersAsync(IEnumerable<UserAuthentication> userList)
        {
            List<UserAuthenticationResult> result = new List<UserAuthenticationResult>(userList.Count());
            foreach (var user in userList)
            {
                var currentResult = new UserAuthenticationResult();
                currentResult.Name = user.Name;

                try
                {
                    UserB2C b2C = new UserB2C
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };
                    await this.CreateUserAsync(b2C);
                    currentResult.State = UserAuthenticationResult.StateType.Succeeded;

                }
                catch (ServiceException sex)
                {
                    _logger.LogError(sex, "{message}", sex.Message);
                    currentResult.State = UserAuthenticationResult.StateType.Failed;
                    if (sex.Error?.Details != null && sex.Error.Details.Any())
                    {
                        var detail = sex.Error.Details.First();
                        if (detail.Code == "ObjectConflict" && detail.Target == "userPrincipalName")
                            currentResult.State = UserAuthenticationResult.StateType.AlreadyCreated;
                    }
                }
                catch (Exception ex)
                {
                    currentResult.State = UserAuthenticationResult.StateType.Failed;
                    _logger.LogError(ex, "{message}", ex.Message);
                }
                result.Add(currentResult);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the available user.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserAuthentication>> GetAvailableUsersAsync()
        {
            List<UserB2C> tt = await this.ListUsersAsync();
            return tt.ToUserAuthenticationList();
        }

        private static string GetCompleteAttributeName(string attributeName, string? B2cExtensionAppClientId)
        {
            if (string.IsNullOrWhiteSpace(B2cExtensionAppClientId))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(B2cExtensionAppClientId));
            }

            return $"extension_{B2cExtensionAppClientId.Replace("-", "")}_{attributeName}";
        }

        /// <summary>Creates the user.</summary>
        /// <param name="user">The user.</param>
        private Task<Result<string>> CreateUserAsync(UserB2C user)
        {
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName) || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Parameter or properties of this paramter cannot ne null", nameof(user));
            }
            return CreateUserInternalAsync(user);
        }

        /// <summary>Creates the user.</summary>
        /// <param name="user">The user.</param>
        private async Task<Result<string>> CreateUserInternalAsync(UserB2C user)
        {
            // Declare the names of the custom attributes
            const string customAttributeMustResetPassword = "mustResetPassword";

            // Fill custom attributes
            IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
            extensionInstance.Add(GetCompleteAttributeName(customAttributeMustResetPassword, _graphService.B2cExtensionAppClientId), "true");

            string pw = PasswordGenerator.GenerateNewPassword();
            var result = await _graphService.Client.Users
                .Request()
                .AddAsync(new User
                {
                    GivenName = user.FirstName,
                    Surname = user.LastName,
                    DisplayName = $"{user.FirstName} {user.LastName}",
                    Identities = new List<ObjectIdentity>
                    {
                            new ObjectIdentity()
                            {
                                SignInType = "emailAddress",
                                Issuer = _graphService.TenantName,
                                IssuerAssignedId = user.Email
                            }
                    },
                    PasswordProfile = new PasswordProfile
                    {
                        Password = pw,
                        ForceChangePasswordNextSignIn = false,
                    },
                    PasswordPolicies = "DisablePasswordExpiration",
                    AdditionalData = extensionInstance
                })
                .ConfigureAwait(false);

            //we send Email to user
            await this.SendConfirmationMailAsync(user, pw);
            return Result.Ok(result.Id);
        }

        private async Task<List<UserB2C>> ListUsersAsync()
        {
            try
            {
                // Get all users (one page)
                var result = await _graphService.Client.Users
                    .Request()
                    .Select($"id,Surname,givenName,displayName,identities")
                    .GetAsync();

                ConcurrentBag<UserB2C> users = new();
                users = await TreatResultAsync(users, result).ConfigureAwait(false);

                return users.ToList();
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "{message}", e.Message);
                return new List<UserB2C>();
            }

        }

        private async Task<ConcurrentBag<UserB2C>> TreatResultAsync(ConcurrentBag<UserB2C> users, IGraphServiceUsersCollectionPage result)
        {
            foreach (var user in result.CurrentPage)
            {
                var u = user.ToUserB2C();
                if (u != null)
                    users.Add(u);
            }
            if (result.NextPageRequest != null)
            {
                users = await TreatResultAsync(users, await result.NextPageRequest.GetAsync());
            }

            return users;
        }

        private async Task SendConfirmationMailAsync(UserB2C user, string password)
        {

            var file = Path.Combine(AppContext.BaseDirectory, "AzureADB2C", "Html", "inscription.templatehtml");
            string readContents;
            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }

            var emailContent = _graphService.EmailContent;
            string content =
                readContents.Replace("{LinkHref}", emailContent.RedirectUrl)
                            .Replace("{Title}", emailContent.Title)
                            .Replace("{Content1}", emailContent.Content1.Replace("{Email}", user.Email).Replace("{Password}", password))
                            .Replace("{Content2}", emailContent.Content2.Replace("{Email}", user.Email).Replace("{Password}", password))
                            .Replace("{LinkText}", emailContent.LinkText);
            await _emailService.SendAsync(emailContent.Title, content, user.Email!);
        }

    }
}

