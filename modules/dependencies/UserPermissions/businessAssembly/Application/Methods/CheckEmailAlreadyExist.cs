using System;
using System.Linq;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application.Methods
{
    /// <summary>
    /// Represents CheckEmailAlreadyExist method.
    /// </summary>
    public class CheckEmailAlreadyExist : ICheckEmailAlreadyExist
    {
        private readonly IUserAccountRepository _userAccountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckEmailAlreadyExist" /> class.
        /// </summary>
        /// <param name="userAccountRepository">The user account repository.</param>
        public CheckEmailAlreadyExist(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        public bool Execute(string? email)
        {
            email = email?.ToLower();
            return !string.IsNullOrEmpty(email) && _userAccountRepository.GetQuery().Any(u => u.Email.ToLower() == email);
        }
    }
}
