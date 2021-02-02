using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;

namespace Glasswall.IdentityManagementService.Common.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
        IAsyncEnumerable<User> GetAllAsync(CancellationToken cancellationToken);
        Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<UserEditOperationState> UpdatePasswordAsync(User user, string password, CancellationToken cancellationToken);
        Task<UserEditOperationState> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<UserEditOperationState> CreateAsync(User user, CancellationToken cancellationToken);
        Task<UserEditOperationState> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }

    public class UserEditOperationState
    {
        public UserEditOperationState(User user = null, params UserWriteError[] errors)
        {
            User = user;
            var errorList = errors ?? Enumerable.Empty<UserWriteError>();
            Errors = errorList?.GroupBy(f => f.ErrorKey).ToDictionary(f => f.Key, f => f.ToArray());
        }

        public User User { get; }

        public IDictionary<string, UserWriteError[]> Errors;

    }

    [ExcludeFromCodeCoverage]
    public class UserWriteError
    {
        public UserWriteError(string errorKey, string errorType, string message)
        {
            ErrorKey = errorKey;
            ErrorType = errorType;
            ErrorMessage = message;
        }

        public string ErrorKey { get; }
        public string ErrorType { get; }
        public string ErrorMessage { get; }
    }
}