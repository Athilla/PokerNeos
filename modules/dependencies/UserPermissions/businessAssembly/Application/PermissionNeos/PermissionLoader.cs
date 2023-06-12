using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Application.Permissions;
using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.Logging;
using GroupeIsa.Neos.Shared.MultiTenant;
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

        public PermissionLoader(IUserInfoAccessor user,
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
        }
        public PermissionDefaultBehavior DefaultBehavior
        {
            get { return PermissionDefaultBehavior.Default; }
        }
        /// <summary>
        /// Build and return a Neos permission list from User roles
        /// </summary>
        /// <param name="functionNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission>> GetPermissionsAsync(string[] functionNames)
        {
            string userEmail = _user?.Email?.ToLower() ?? string.Empty;
            var permissionsInCache = await _coreMemoryCache.GetOrCreateAsync(MemoryCacheCategories.Permissions.ToString(), userEmail, () => GetPermissionsAsync(userEmail));
            _logger.LogInformation($"PermissionLoader FunctionName : {string.Join(',', functionNames)}");
#if DEBUG
            _logger.LogInformation($"Reading from cache :");
            LogPermissions(permissionsInCache);
#endif
            List<GroupeIsa.Neos.Application.Permissions.Permission> permissions = new();
            foreach (var functionName in functionNames)
            {
                if (permissionsInCache.Any(x => x.FunctionName == functionName))
                    permissions.Add(permissionsInCache.First(x => x.FunctionName == functionName));
            }
#if DEBUG
            _logger.LogInformation($"return : :");
            LogPermissions(permissions);
#endif
            return permissions;
        }
        private async Task<IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission>> GetPermissionsAsync(string userEmail)
        {
            List<Transversals.Business.Application.Abstractions.DataObjects.FunctionTreeNode> functionTree = _getFunctionTree.Execute();
            List<GroupeIsa.Neos.Application.Permissions.Permission> permissions = new();
            UserAccount? userAccount = _userAccountRepository.GetQuery().SingleOrDefault(u => u.Email == userEmail);
            if (userAccount != null)
            {
                List<int> userRolesIds = _userAccountRoleRepository.GetQuery()
                                            .Include(e => e.UserRole)
                                            .Where(e => e.UserAccountId == userAccount.Id)
                                            .Select(e => e.UserRole.Id).ToList();
                List<Business.Domain.Entities.Permission> permissionsRoleFunction = _permissionRepository.GetQuery().Where(e => userRolesIds.Contains(e.UserRoleId)).ToList();
                // On ajoute les permissions pour chaque fonction de l'arbre
                foreach (var node in functionTree)
                {
                    GetNodePermission(node, null, permissions, permissionsRoleFunction, userRolesIds);
                }
            }
            return await Task.FromResult(permissions);
        }
#if DEBUG
        private void LogPermissions(IEnumerable<GroupeIsa.Neos.Application.Permissions.Permission> permissions)
        {
            foreach (var perm in permissions)
            {
                if (perm is AllowDenyPermission adp)
                {
                    _logger.LogInformation($" {adp.FunctionName} => {adp.Allow}");
                }
                else if (perm is CrudPermission crud)
                {
                    _logger.LogInformation($" {crud.FunctionName} => {crud.Create}-{crud.Read}-{crud.Update}-{crud.Delete}");
                }
            }
        }
#endif
        /// <summary>
        /// Crée la permission sur une fonction par rapport aux rôles de l'utilisateur et des accès sur la fonction parent.
        /// Puis ajoute  la permission du noeud dans la liste des permissions
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentCrudPermission"></param>
        /// <param name="parentAllowDenyPermission"></param>
        /// <param name="permissions"></param>
        /// <param name="permissionsRoleFunctionGroup"></param>
        private void GetNodePermission(FunctionTreeNode node,
            FunctionTreeNode? parentNode,
            List<GroupeIsa.Neos.Application.Permissions.Permission> permissions,
            List<Business.Domain.Entities.Permission> userPermissions,
            List<int> userRolesIds)
        {
            Business.Domain.Entities.Permission[] functionPermissions = userPermissions.Where(e => e.FunctionName == node.Name).ToArray();
            //Récupérer les permissions des fonctions parentes pour les rôles qui n'ont pas de permission sur la fonction
            foreach (int userRoleId in userRolesIds)
            {
                if (!functionPermissions.Any(e => e.UserRoleId == userRoleId))
                {
                    if (parentNode != null)
                    {
                        Business.Domain.Entities.Permission? parentPermission = userPermissions.FirstOrDefault(e => e.FunctionName == parentNode.Name && e.UserRoleId == userRoleId);
                        userPermissions.Add(GetPermissionFromParentFunctionRole(node, parentPermission, userRoleId));
                    }
                    else
                    {
                        userPermissions.Add(GetPermissionFromParentFunctionRole(node, null, userRoleId));
                    }
                }
            }
            functionPermissions = userPermissions.Where(e => e.FunctionName == node.Name).ToArray();
            permissions.Add(GetNeosPermissionForFunction(node, functionPermissions));
            //même traitement pour chaque enfant
            foreach (var child in node.Children)
            {
                GetNodePermission(child, node, permissions, userPermissions, userRolesIds);
            }
        }

        /// <summary>
        /// Donne la permission du noeud en fonction de la permission parent et de l'id du rôle
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentPermission"></param>
        /// <param name="userRoleId"></param>
        /// <returns>La permission héritée par le noeud</returns>
        private static Business.Domain.Entities.Permission GetPermissionFromParentFunctionRole(FunctionTreeNode node,
            Business.Domain.Entities.Permission? parentPermission,
            int userRoleId
            )
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
        private static GroupeIsa.Neos.Application.Permissions.Permission GetNeosPermissionForFunction(FunctionTreeNode node,
            Business.Domain.Entities.Permission[] functionPermissions
            )
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
    }
}