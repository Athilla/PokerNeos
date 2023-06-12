using System.Collections.Generic;
using System.Linq;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application.Methods
{
    /// <summary>
    /// Represents GetAssignedFeatureChildrenTreeNode method.
    /// </summary>
    public class GetAssignedFeatureChildrenTreeNode : IGetAssignedFeatureChildrenTreeNode
    {
        private readonly IUserAccountUserRoleRepository _userAccountUserRoleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGetFunctionTree _getFunctionTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAssignedFeaturesTreeNodeChildren"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetAssignedFeatureChildrenTreeNode(IUserAccountUserRoleRepository userAccountUserRoleRepository,
            IPermissionRepository permissionRepository,
            IGetFunctionTree getFunctionTree)
        {
            _userAccountUserRoleRepository = userAccountUserRoleRepository;
            _permissionRepository = permissionRepository;
            _getFunctionTree = getFunctionTree;
        }

        /// <inheritdoc/>
        public List<TreeNodeFeature> Execute(int userId)
        {
            int[] IdAssignedRoles = _userAccountUserRoleRepository.GetQuery().Where(e => e.UserAccountId == userId).Select(e => e.UserRoleId).ToArray();
            List<TreeNodeFeature> result = new();
            List<FunctionTreeNode> functions = _getFunctionTree.Execute();

            foreach (FunctionTreeNode node in functions)
            {
                var createdNode = SetTreeNode(node, IdAssignedRoles, null);
                if (createdNode != null)
                {
                    SetTreeNodeIcon(createdNode);
                    result.Add(createdNode);
                }
            }

            return result;
        }

        /// <summary>
        /// Build node from function and permission
        /// </summary>
        /// <param name="node"></param>
        /// <param name="idAssignedRoles"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private TreeNodeFeature? SetTreeNode(FunctionTreeNode node, int[] idAssignedRoles, int? parentId)
        {
            var permissions = _permissionRepository
                .GetQuery()
                .Where(e => idAssignedRoles.Contains(e.UserRoleId) && e.FunctionName == node.Name)
                .AsEnumerable()
                .GroupBy(e => e.FunctionName);

            TreeNodeFeature? treeNodeFeature = null;

            foreach (IGrouping<string, Permission> group in permissions)
            {
                if (treeNodeFeature == null)
                {
                    treeNodeFeature = new()
                    {
                        Name = node.Name,
                        Caption = node.Caption,
                        FeatureHasNoPermission = false,
                        Id = group.First().Id,
                        Code = node.Name,
                        ParentFeatureId = parentId,
                        Children = new List<TreeNodeFeature>() { },
                        HasCRUDAccess = group.First().FunctionType == FunctionType.CRUD,
                        HasAccess = group.First().HasAccess,
                        HasReadOnlyAccess = group.First().HasReadOnlyAccess,
                        HasCreationAccess = group.First().HasCreationAccess,
                        HasUpdateAccess = group.First().HasUpdateAccess,
                        HasDeleteAccess = group.First().HasDeleteAccess
                    };

                    foreach (var child in node.Children)
                    {
                        var createdChild = SetTreeNode(child, idAssignedRoles, group.First().Id);
                        if (createdChild != null)
                        {
                            treeNodeFeature.Children.Add(createdChild);
                        }
                    }
                }

                AssignPermissions(group, treeNodeFeature);
                SetTreeNodeIcon(treeNodeFeature);

            }

            return treeNodeFeature;
        }

        /// <summary>
        /// Assigns permissions to given node
        /// </summary>
        /// <param name="group"></param>
        /// <param name="n"></param>
        private static void AssignPermissions(IGrouping<string, Permission> group, TreeNodeFeature node)
        {
            //merge of permissions granted for the function
            for (int i = 1; i < group.Count(); i++)
            {
                node.HasAccess = group.ElementAt(i).HasAccess || node.HasAccess;
                node.HasReadOnlyAccess = group.ElementAt(i).HasReadOnlyAccess || node.HasReadOnlyAccess;
                node.HasCreationAccess = group.ElementAt(i).HasCreationAccess || node.HasCreationAccess;
                node.HasUpdateAccess = group.ElementAt(i).HasUpdateAccess || node.HasUpdateAccess;
                node.HasDeleteAccess = group.ElementAt(i).HasDeleteAccess || node.HasDeleteAccess;
            }
        }

        /// <summary>
        /// Sets an icon for given node
        /// </summary>
        /// <param name="node"></param>
        private static void SetTreeNodeIcon(TreeNodeFeature node)
        {
            if (node.HasAccess || (node.HasCreationAccess && node.HasDeleteAccess && node.HasReadOnlyAccess && node.HasUpdateAccess))
            {
                node.Icon = "🟢";
            }
            else if (node.HasCreationAccess || node.HasDeleteAccess || node.HasReadOnlyAccess || node.HasUpdateAccess)
            {
                node.Icon = "🟡";
            }
            else
            {
                node.Icon = "🔴";
            }
        }
    }
}