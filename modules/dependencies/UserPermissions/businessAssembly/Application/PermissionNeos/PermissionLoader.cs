using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupeIsa.Neos.Application.Permissions;
using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.Logging;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.Extensions.Configuration;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.UserPermissions.Domain.MemoryCache;

namespace Transversals.Business.UserPermissions.Application.PermissionNeos
{
    public class PermissionLoader : IPermissionLoader
    {
        private readonly IUserInfo? _user;
        private readonly IGetFunctionTree _getFunctionTree;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUserAccountUserRoleRepository _userAccountRoleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ICoreMemoryCache _coreMemoryCache;
        private readonly INeosLogger<PermissionLoader> _logger;

        public PermissionLoader(
            IConfiguration configuration,
            IUserInfoAccessor user,
            IUserAccountRepository userAccountRepository,
            IUserAccountUserRoleRepository userAccountRoleRepository,
            IPermissionRepository permissionRepository,
            ICoreMemoryCache coreMemoryCache,
            IGetFunctionTree getFunctionTree,
            INeosLogger<PermissionLoader> logger)
        {
            _user = user.User;
            _userAccountRepository = userAccountRepository;
            _userAccountRoleRepository = userAccountRoleRepository;
            _permissionRepository = permissionRepository;
            _coreMemoryCache = coreMemoryCache;
            _getFunctionTree = getFunctionTree;
            _logger = logger;
            DefaultBehavior = GetDefaultBehavior(configuration);
        }

        public PermissionDefaultBehavior DefaultBehavior { get; }

        /// <summary>
        /// Build and return a Neos permission list from User roles
        /// </summary>
        /// <param name="functionNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission>> GetPermissionsAsync(string[] functionNames)
        {
            if (_user == null)
            {
                return Array.Empty<GroupeIsa.Neos.Application.Permissions.Permission>();
            }

            IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission> userPermissions = await _coreMemoryCache.GetOrCreateAsync(
                MemoryCacheCategories.Permissions.ToString(),
                _user.Identifier,
                () => GetPermissionsAsync(_user));

            IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission> permissions = userPermissions.Where(p => functionNames.Contains(p.FunctionName));

#if DEBUG
            _logger.LogInformation(GetPermissionsLog(permissions));
#else
            _logger.LogDebug(GetPermissionsLog(permissions));
#endif

            return permissions;
        }
        private async Task<IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission>> GetPermissionsAsync(IUserInfo user)
        {
            string userIdentifier = user.Identifier.ToLower();
            string? userEmail = user.Email?.ToLower();
            UserAccount? userAccount = _userAccountRepository.GetQuery()
                .SingleOrDefault(u => u.Login.ToLower() == userIdentifier || (userEmail != null && u.Email.ToLower() == userEmail));
            if (userAccount == null)
            {
                return Array.Empty<GroupeIsa.Neos.Application.Permissions.Permission>();
            }

            int[] userRolesIds = _userAccountRoleRepository.GetQuery()
                .Include(e => e.UserRole)
                .Where(e => e.UserAccountId == userAccount.Id)
                .Select(e => e.UserRole.Id)
                .ToArray();
            if (!userRolesIds.Any())
            {
                return Array.Empty<GroupeIsa.Neos.Application.Permissions.Permission>();
            }

            List<GroupeIsa.Neos.Application.Permissions.Permission> userPermissions = new();

            List<Business.Domain.Entities.Permission> allPermissions = _permissionRepository.GetQuery().Where(e => userRolesIds.Contains(e.UserRoleId)).ToList();

            List<FunctionTreeNode> functionTree = _getFunctionTree.Execute();
            foreach (FunctionTreeNode node in functionTree)
            {
                AddPermission(userPermissions, allPermissions, userRolesIds, node, null);
            }

#if DEBUG
            _logger.LogInformation(GetUserPermissionLog(user, userPermissions));
#else
            _logger.LogDebug(GetUserPermissionLog(user, userPermissions));
#endif

            return await Task.FromResult(userPermissions);
        }

        private void AddPermission(
            List<GroupeIsa.Neos.Application.Permissions.Permission> userPermissions,
            List<Business.Domain.Entities.Permission> allPermissions,
            int[] userRolesIds,
            FunctionTreeNode node,
            FunctionTreeNode? parentNode)
        {
            Business.Domain.Entities.Permission[] functionPermissions = allPermissions.Where(e => e.FunctionName == node.Name).ToArray();

            // Récupérer les permissions des fonctions parentes pour les rôles qui n'ont pas de permission sur la fonction
            foreach (int userRoleId in userRolesIds)
            {
                if (!functionPermissions.Any(e => e.UserRoleId == userRoleId))
                {
                    if (parentNode != null)
                    {
                        Business.Domain.Entities.Permission? parentPermission = allPermissions.FirstOrDefault(e => e.FunctionName == parentNode.Name && e.UserRoleId == userRoleId);
                        allPermissions.Add(GetPermissionFromParentFunctionRole(node, userRoleId, parentPermission));
                    }
                    else
                    {
                        allPermissions.Add(GetPermissionFromParentFunctionRole(node, userRoleId, null));
                    }
                }
            }
            functionPermissions = allPermissions.Where(e => e.FunctionName == node.Name).ToArray();
            userPermissions.Add(CreateNeosPermissionFromPermissions(node, functionPermissions));

            // Même traitement pour chaque enfant
            foreach (FunctionTreeNode child in node.Children)
            {
                AddPermission(userPermissions, allPermissions, userRolesIds, child, node);
            }
        }

        /// <summary>
        /// Donne la permission du noeud en fonction de la permission parent et de l'id du rôle
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentPermission"></param>
        /// <param name="userRoleId"></param>
        /// <returns>La permission héritée par le noeud</returns>
        private static Business.Domain.Entities.Permission GetPermissionFromParentFunctionRole(
            FunctionTreeNode node,
            int userRoleId,
            Business.Domain.Entities.Permission? parentPermission)
        {
            if (parentPermission != null)
            {
                if (node.FunctionType == FunctionType.CRUD)
                {
                    if (parentPermission.FunctionType == FunctionType.AllowDeny)
                    {
                        return new Business.Domain.Entities.Permission()
                        {
                            FunctionType = node.FunctionType,
                            FunctionName = node.Name,
                            HasAccess = false,
                            HasReadOnlyAccess = parentPermission.HasAccess,
                            HasCreationAccess = parentPermission.HasAccess,
                            HasUpdateAccess = parentPermission.HasAccess,
                            HasDeleteAccess = parentPermission.HasAccess,
                            UserRoleId = userRoleId
                        };
                    }
                    else
                    {
                        return new Business.Domain.Entities.Permission()
                        {
                            FunctionType = node.FunctionType,
                            FunctionName = node.Name,
                            HasAccess = false,
                            HasReadOnlyAccess = parentPermission.HasReadOnlyAccess,
                            HasCreationAccess = parentPermission.HasCreationAccess,
                            HasUpdateAccess = parentPermission.HasUpdateAccess,
                            HasDeleteAccess = parentPermission.HasDeleteAccess,
                            UserRoleId = userRoleId
                        };
                    }
                }
                else
                {
                    if (parentPermission.FunctionType == FunctionType.AllowDeny)
                    {
                        return new Business.Domain.Entities.Permission()
                        {
                            FunctionType = node.FunctionType,
                            FunctionName = node.Name,
                            HasAccess = parentPermission.HasAccess,
                            HasReadOnlyAccess = false,
                            HasCreationAccess = false,
                            HasUpdateAccess = false,
                            HasDeleteAccess = false,
                            UserRoleId = userRoleId
                        };
                    }
                    else
                    {
                        return new Business.Domain.Entities.Permission()
                        {
                            FunctionType = node.FunctionType,
                            FunctionName = node.Name,
                            HasAccess = parentPermission.HasReadOnlyAccess && parentPermission.HasCreationAccess && parentPermission.HasUpdateAccess && parentPermission.HasDeleteAccess,
                            HasReadOnlyAccess = false,
                            HasCreationAccess = false,
                            HasUpdateAccess = false,
                            HasDeleteAccess = false,
                            UserRoleId = userRoleId
                        };
                    }
                }
            }

            return new Business.Domain.Entities.Permission()
            {
                FunctionType = node.FunctionType,
                FunctionName = node.Name,
                HasAccess = false,
                HasReadOnlyAccess = false,
                HasCreationAccess = false,
                HasUpdateAccess = false,
                HasDeleteAccess = false,
                UserRoleId = userRoleId
            };
        }

        /// <summary>
        /// Créer la permission Neos correspondant au permissions de l'utilisateur sur la fonction
        /// </summary>
        /// <param name="node"></param>
        /// <param name="functionPermissions"></param>
        /// <returns>La permission Neos</returns>
        private static GroupeIsa.Neos.Application.Permissions.Permission CreateNeosPermissionFromPermissions(
            FunctionTreeNode node,
            Business.Domain.Entities.Permission[] functionPermissions)
        {
            if (node.FunctionType == FunctionType.CRUD)
            {
                return new CrudPermission(
                    node.Name,
                    functionPermissions.Any(e => e.FunctionType == FunctionType.CRUD && e.HasCreationAccess),
                    functionPermissions.Any(e => e.FunctionType == FunctionType.CRUD && e.HasReadOnlyAccess),
                    functionPermissions.Any(e => e.FunctionType == FunctionType.CRUD && e.HasUpdateAccess),
                    functionPermissions.Any(e => e.FunctionType == FunctionType.CRUD && e.HasDeleteAccess)
                );
            }
            else
            {
                return new AllowDenyPermission(node.Name, functionPermissions.Any(e => e.FunctionType == FunctionType.AllowDeny && e.HasAccess));
            }
        }

        private static PermissionDefaultBehavior GetDefaultBehavior(IConfiguration configuration)
        {
            string? value = configuration["PermissionDefaultBehavior"];
            if (value != null && Enum.TryParse(value, true, out PermissionDefaultBehavior defaultBehavior))
            {
                return defaultBehavior;
            }

            return PermissionDefaultBehavior.Default;
        }

        private static string GetPermissionsLog(IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission> permissions)
        {
            StringBuilder sb = new();
            sb.AppendLine($"{permissions.Count()} permissions used for the following functions : ");

            foreach (GroupeIsa.Neos.Application.Permissions.Permission permission in permissions)
            {
                if (permission is CrudPermission crudPermission)
                {
                    StringBuilder crudPermissionStringBuilder = new();

                    if (crudPermission.Create)
                    {
                        crudPermissionStringBuilder.Append('C');
                    }

                    if (crudPermission.Read)
                    {
                        crudPermissionStringBuilder.Append('R');
                    }

                    if (crudPermission.Update)
                    {
                        crudPermissionStringBuilder.Append('U');
                    }

                    if (crudPermission.Delete)
                    {
                        crudPermissionStringBuilder.Append('D');
                    }

                    sb.AppendLine($"- {crudPermission.FunctionName} => {crudPermissionStringBuilder.ToString()}");
                }
                else if (permission is AllowDenyPermission adp)
                {
                    sb.AppendLine($"- {adp.FunctionName} => {(adp.Allow ? "Allow" : "Deny")}");
                }
            }

            return sb.ToString();
        }

        private static string GetUserPermissionLog(IUserInfo user, List<GroupeIsa.Neos.Application.Permissions.Permission> userPermissions)
        {
            string message = $"Retrieved {userPermissions.Count} permissions retrieved for the following user : {user.Identifier}";
            if (user.Email != null)
            {
                message += $" ({user.Email})";
            }

            return message;
        }
    }
}