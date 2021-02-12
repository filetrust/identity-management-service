using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Services.Exceptions;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Glasswall.IdentityManagementService.Common.Store;
using Newtonsoft.Json;

namespace Glasswall.IdentityManagementService.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IFileStore _fileStore;
        private const int DefaultPasswordLength = 16;

        public UserService(IFileStore fileStore)
        {
            _fileStore = fileStore ?? throw new ArgumentNullException(nameof(fileStore));
        }

        public Task<UserEditOperationState> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("Value is required", nameof(user.Username));
            return InternalCreateAsync(user, cancellationToken);
        }

        public Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            return InternalAuthenticateAsync(username, password, cancellationToken);
        }

        public async IAsyncEnumerable<User> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var filePath in _fileStore.SearchAsync("", new UserMetadataSearchStrategy(), cancellationToken)) 
                yield return await InternalDownloadAsync(filePath, cancellationToken);
        }

        public Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentException("Value must not be empty", nameof(id));
            return InternalDownloadAsync($"{id}.json", cancellationToken);
        }
        
        public Task<UserEditOperationState> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return InternalUpdateAsync(user, cancellationToken);
        }

        public Task<UserEditOperationState> UpdatePasswordAsync(User user, string password, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            return InternalUpdatePasswordAsync(user.Id, password, cancellationToken);
        }

        public Task<UserEditOperationState> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentException("Value must not be empty", nameof(id));
            return InternalDeleteAsync(id, cancellationToken);
        }

        private async Task<UserEditOperationState> InternalCreateAsync(User userParam, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userParam.Id, cancellationToken);
            if (user != null) return new UserEditOperationState(user); //, new UserDataNotUniqueError(userParam.Id, nameof(userParam.Id), userParam.Id.ToString()));

            var dErrors = await GetDuplicationErrors(userParam, cancellationToken);
            if (dErrors.Any()) return new UserEditOperationState(userParam, dErrors);

            var (passwordSalt, passwordHash) = SaltAndHashPassword(RandomPassword(DefaultPasswordLength));

            userParam.PasswordHash = passwordHash;
            userParam.PasswordSalt = passwordSalt;
            userParam.Status = UserStatus.Active;

            await InternalUploadAsync(userParam, cancellationToken);
            return new UserEditOperationState(userParam);
        }

        private async Task<User> InternalDownloadAsync(string path, CancellationToken ct)
        {
            if (!await _fileStore.ExistsAsync(path, ct)) return null;
            await using var file = await _fileStore.ReadAsync(path, ct);
            var fileString = Encoding.UTF8.GetString(file.ToArray());
            return JsonConvert.DeserializeObject<User>(fileString);
        }
        
        private async Task<User> InternalAuthenticateAsync(string username, string password,
            CancellationToken cancellationToken)
        {
            await foreach (var user in GetAllAsync(cancellationToken))
                if (username == user.Username && PasswordMatches(password, user.PasswordHash, user.PasswordSalt))
                    return user;

            return null;
        }
        
        private async Task<UserEditOperationState> InternalUpdateAsync(User userParam, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userParam.Id, cancellationToken);
            if (user == null) return new UserEditOperationState(null, new UserNotFoundError(userParam.Id));

            var errors = await GetDuplicationErrors(userParam, cancellationToken);
            if (errors.Any()) return new UserEditOperationState(userParam, errors);

            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Email = userParam.Email;
            user.Username = userParam.Username;
            user.Status = userParam.Status;

            await InternalUploadAsync(user, cancellationToken);

            return new UserEditOperationState(user);
        }

        private async Task<UserEditOperationState> InternalUpdatePasswordAsync(Guid id, string password, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(id, cancellationToken);

            if (user == null) return new UserEditOperationState(null, new UserNotFoundError(id));

            (user.PasswordSalt, user.PasswordHash) = SaltAndHashPassword(password);

            await InternalUploadAsync(user, cancellationToken);

            return new UserEditOperationState(user);
        }

        private async Task<UserEditOperationState> InternalDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == DefaultUser.DefaultUserId)
            {
                var user = new DefaultUser {PasswordSalt = null, PasswordHash = null, Status = UserStatus.Deactivated};
                await InternalUploadAsync(user, cancellationToken);
            }
            else
            {
                await _fileStore.DeleteAsync($"{id}.json", cancellationToken);
            }

            return new UserEditOperationState();
        }

        private async Task<UserWriteError[]> GetDuplicationErrors(User userParam, CancellationToken cancellationToken)
        {
            var errors = new List<UserWriteError>();

            await foreach (var otherUser in GetAllAsync(cancellationToken))
            {
                if (otherUser.Id == userParam.Id) continue;

                if (otherUser.Username == userParam.Username) errors.Add(new UserDataNotUniqueError(otherUser.Id, nameof(otherUser.Username), userParam.Username));
                if (otherUser.Email == userParam.Email) errors.Add(new UserDataNotUniqueError(otherUser.Id, nameof(otherUser.Email), userParam.Email));
            }

            return errors.ToArray();
        }

        private async Task InternalUploadAsync(User user, CancellationToken ct)
        {
            var path = $"{user.Id}.json";
            await _fileStore.WriteAsync(path, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)), ct);
        }

        private static (byte[], byte[]) SaltAndHashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private static bool PasswordMatches(string password, IReadOnlyList<byte> storedHash, byte[] storedSalt)
        {
            if (storedHash == null) return false;

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return !computedHash.Where((t, i) => t != storedHash[i]).Any();
            }
        }

        private static readonly Random Random = new Random();

        private static string RandomPassword(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789[]!\"£$%^&()_+";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}