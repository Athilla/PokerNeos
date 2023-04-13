using GroupeIsa.Neos.Application.Permissions;
using System;
using System.Linq;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Enums;

namespace Transversals.Business.UserPermissions.Application.Methods
{
    /// <summary>
    /// Represents GetFunctionTree method.
    /// </summary>
    public class GetFunctionTree : IGetFunctionTree
    {
        private readonly IFunctionLoader _functionLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFunctionTree" /> class.
        /// </summary>
        /// <param name="functionLoader">The function loader.</param>
        public GetFunctionTree(IFunctionLoader functionLoader)
        {
            _functionLoader = functionLoader;
        }

        /// <inheritdoc/>
        public System.Collections.Generic.List<Transversals.Business.Application.Abstractions.DataObjects.FunctionTreeNode> Execute()
        {
            IFunction[] functions = _functionLoader.GetFunctions();
            return functions.Select(MapToFunctionTreeNode).ToList();
        }

        private static FunctionTreeNode MapToFunctionTreeNode(IFunction function)
        {
            return new()
            {
                Name = function.Name,
                Caption = function.Caption,
                FunctionType = function.PermissionType == PermissionType.CRUD ? FunctionType.CRUD : FunctionType.AllowDeny,
                Children = function.Children?.Select(MapToFunctionTreeNode).ToArray() ?? Array.Empty<FunctionTreeNode>(),
            };
        }
    }
}