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

        public Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            return InternalAuthenticateAsync(username, password, cancellationToken);
        }

        public async IAsyncEnumerable<User> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var filePath in _fileStore.SearchAsync("", new UserMetadataSearchStrategy(),
                cancellationToken)) yield return await InternalDownloadAsync(filePath, cancellationToken);
        }

        public Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentException("Value must not be empty", nameof(id));

            return InternalDownloadAsync($"{id}.json", cancellationToken);
        }

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Value is required", nameof(user.Username));

            return InternalCreateAsync(user, cancellationToken);
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return InternalUpdateAsync(user, cancellationToken);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) throw new ArgumentException("Value must not be empty", nameof(id));
            return _fileStore.DeleteAsync($"{id}.json", cancellationToken);
        }

        public Task UpdatePasswordAsync(User user, string password, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            (user.PasswordSalt, user.PasswordHash) = SaltAndHashPassword(password);

            return InternalUploadAsync(user, cancellationToken);
        }

        private async Task<User> InternalCreateAsync(User user, CancellationToken cancellationToken)
        {
            await foreach (var otherUser in GetAllAsync(cancellationToken))
                if (otherUser.Id != user.Id && otherUser.Username == user.Username)
                    return otherUser;

            var (passwordSalt, passwordHash) = SaltAndHashPassword(RandomPassword(DefaultPasswordLength));

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Status = UserStatus.Active;

            await InternalUploadAsync(user, cancellationToken);
            return user;
        }

        private async Task InternalUpdateAsync(User userParam, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userParam.Id, cancellationToken);

            if (user == null) throw new UserNotFoundException(userParam.Id);

            await foreach (var otherUser in GetAllAsync(cancellationToken))
                if (otherUser.Id != userParam.Id && otherUser.Username == userParam.Username)
                    throw new UsernameAlreadyTakenException(otherUser.Id, userParam.Username);

            user.Username = userParam.Username;

            if (!string.IsNullOrWhiteSpace(userParam.FirstName)) user.FirstName = userParam.FirstName;
            if (!string.IsNullOrWhiteSpace(userParam.LastName)) user.LastName = userParam.LastName;
            if (!string.IsNullOrWhiteSpace(userParam.Email)) user.LastName = userParam.Email;

            user.Status = userParam.Status;

            await InternalUploadAsync(user, cancellationToken);
        }

        private async Task<User> InternalDownloadAsync(string path, CancellationToken ct)
        {
            if (!await _fileStore.ExistsAsync(path, ct)) return null;
            await using var file = await _fileStore.ReadAsync(path, ct);
            var fileString = Encoding.UTF8.GetString(file.ToArray());
            return JsonConvert.DeserializeObject<User>(fileString);
        }

        private async Task InternalUploadAsync(User user, CancellationToken ct)
        {
            var path = $"{user.Id}.json";
            await _fileStore.WriteAsync(path, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)), ct);
        }

        private async Task<User> InternalAuthenticateAsync(string username, string password,
            CancellationToken cancellationToken)
        {
            await foreach (var user in GetAllAsync(cancellationToken))
                if (username == user.Username && PasswordMatches(password, user.PasswordHash, user.PasswordSalt))
                    return user;

            return null;
        }

        private static (byte[], byte[]) SaltAndHashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private static bool PasswordMatches(string password, IReadOnlyList<byte> storedHash, byte[] storedSalt)
        {
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