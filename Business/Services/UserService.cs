using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Glasswall.IdentityManagementService.Common.Store;

namespace Glasswall.IdentityManagementService.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IFileStore _fileStore;

        public UserService(IFileStore fileStore)
        {
            _fileStore = fileStore ?? throw new ArgumentNullException(nameof(fileStore));
        }

        public Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            return InternalAuthenticateAsync(username, password, cancellationToken);
        }

        public async IAsyncEnumerable<User> GetAllAsync([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await foreach (var filePath in _fileStore.SearchAsync("", new CollectTopLevelPaths(), cancellationToken))
            {
                yield return await InternalDownloadAsync(filePath, cancellationToken);
            }
        }

        public Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return InternalDownloadAsync($"{id}.json", cancellationToken);
        }

        public Task<User> CreateAsync(User user, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value is required", nameof(password));
            if (user.Username == null) throw new ArgumentNullException(nameof(user.Username));
            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("Value is required", nameof(user.Username));
            
            return InternalCreateAsync(user, password, cancellationToken);
        }

        public async Task UpdateAsync(User userParam, string password, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userParam.Id, cancellationToken);

            if (user == null) throw new ApplicationException("User not found");

            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                await foreach (var otherUser in GetAllAsync(cancellationToken))
                {
                    if (otherUser.Id != userParam.Id && otherUser.Username == userParam.Username)
                        throw new ApplicationException("Username " + userParam.Username + " is already taken");
                }

                user.Username = userParam.Username;
            }

            if (!string.IsNullOrWhiteSpace(userParam.FirstName)) user.FirstName = userParam.FirstName;
            if (!string.IsNullOrWhiteSpace(userParam.LastName)) user.LastName = userParam.LastName;
            
            if (!string.IsNullOrWhiteSpace(password))
            {
                var (salt, hash) = SaltAndHashPassword(password);

                if (hash != user.PasswordHash)
                {
                    user.PasswordHash = hash;
                    user.PasswordSalt = salt;
                }
            }

            await InternalUploadAsync(user, cancellationToken);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            return _fileStore.DeleteAsync($"{id}.json", cancellationToken);
        }

        private async Task<User> InternalCreateAsync(User user, string password, CancellationToken cancellationToken)
        {
            await foreach (var otherUser in GetAllAsync(cancellationToken))
            {
                if (otherUser.Id != user.Id && otherUser.Username == user.Username)
                    throw new ApplicationException("Username " + user.Username + " is already taken");
            }

            var (passwordSalt, passwordHash) = SaltAndHashPassword(password);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await InternalUploadAsync(user, cancellationToken);
            return user;
        }

        private async Task<User> InternalDownloadAsync(string path, CancellationToken ct)
        {            
            if (!await _fileStore.ExistsAsync(path, ct)) return null;            
            using var file = await _fileStore.ReadAsync(path, ct);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(file.ToArray()));
        }

        private async Task InternalUploadAsync(User user, CancellationToken ct)
        {
            var path = $"{user.Id}.json";
            await _fileStore.WriteAsync(path, Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(user)), ct);
        }

        private async Task<User> InternalAuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            await foreach (var user in GetAllAsync(cancellationToken))
            {
                if (username == user.Username && PasswordMatches(password, user.PasswordHash, user.PasswordSalt))
                    return user;
            }

            return null;
        }

        private static (byte[], byte[]) SaltAndHashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private static bool PasswordMatches(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash.Length != 64) throw new InvalidOperationException("Invalid length of password hash (64 bytes expected).");
            if (storedSalt.Length != 128) throw new InvalidOperationException("Invalid length of password salt (128 bytes expected).");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}