using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Store;
using Microsoft.Extensions.Logging;

namespace Glasswall.IdentityManagementService.Business.Store
{
    public class FileStore : IFileStore
    {
        public const string EncryptedMarker = "ThisFileIsEncrypted";
        private const int InitializationVectorLength = 16;
        private readonly ILogger<FileStore> _logger;
        private readonly IFileStoreOptions _fileStoreOptions;
        private readonly byte[] _encryptedMarkerBytes = Encoding.UTF8.GetBytes(EncryptedMarker);

        public FileStore(
            ILogger<FileStore> logger,
            IFileStoreOptions fileStoreOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStoreOptions = fileStoreOptions ?? throw new ArgumentNullException(nameof(fileStoreOptions));
        }

        public IAsyncEnumerable<string> SearchAsync(string relativePath, IPathActions pathActions,
            CancellationToken cancellationToken)
        {
            if (pathActions == null) throw new ArgumentNullException(nameof(pathActions));

            return InternalSearchAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), pathActions,
                cancellationToken);
        }

        public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalExistsAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath));
        }

        public Task<MemoryStream> ReadAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalReadAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), cancellationToken);
        }

        public Task WriteAsync(string relativePath, byte[] bytes, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalWriteAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath), bytes, cancellationToken);
        }

        public Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Value must not be null or whitespace", nameof(relativePath));

            return InternalDeleteAsync(Path.Combine(_fileStoreOptions.RootPath, relativePath));
        }

        private static Task InternalDeleteAsync(string fullPath)
        {
            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, true);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        private async Task InternalWriteAsync(string fullPath, byte[] bytes, CancellationToken cancellationToken)
        {
            var dir = Path.GetDirectoryName(fullPath)
                      ?? throw new ArgumentException("A directory was not specified", nameof(fullPath));

            Directory.CreateDirectory(dir);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            if (_fileStoreOptions.EncryptionHandler != null)
                bytes = await GetEncryptedBytes(bytes, cancellationToken);

            await File.WriteAllBytesAsync(fullPath, bytes, cancellationToken);
        }

        private async Task<byte[]> GetEncryptedBytes(byte[] bytes, CancellationToken cancellationToken)
        {
            var saltBytes = EncryptionUtils.GenerateSalt(InitializationVectorLength);

            var data = await _fileStoreOptions.EncryptionHandler.EncryptAsync(
                bytes,
                _fileStoreOptions.EncryptionSecret,
                saltBytes,
                cancellationToken);

            return _encryptedMarkerBytes.Concat(saltBytes).Concat(data).ToArray();
        }

        private async Task<MemoryStream> InternalReadAsync(string path, CancellationToken cancellationToken)
        {
            if (!File.Exists(path))
                return null;

            var ms = new MemoryStream();

            await using (var fs = File.OpenRead(path))
            {
                if (fs.Length == 0) return null;
                await fs.CopyToAsync(ms, (int) fs.Length, cancellationToken);
            }

            if (_fileStoreOptions.EncryptionHandler == null) return ms;

            return await ReadEncrypted(ms, cancellationToken);
        }

        private async Task<MemoryStream> ReadEncrypted(MemoryStream origContent, CancellationToken cancellationToken)
        {
            var fileContents = origContent.ToArray();

            if (_encryptedMarkerBytes.Where((t, i) => fileContents[i] != t).Any()) return origContent;

            var saltBytes = fileContents.Skip(_encryptedMarkerBytes.Length).Take(InitializationVectorLength);

            var data = await _fileStoreOptions.EncryptionHandler.DecryptAsync(
                fileContents.Skip(_encryptedMarkerBytes.Length).Skip(InitializationVectorLength).ToArray(),
                _fileStoreOptions.EncryptionSecret,
                saltBytes.ToArray(),
                cancellationToken);

            return new MemoryStream(data.ToArray());
        }

        private static Task<bool> InternalExistsAsync(string fullPath)
        {
            return Task.FromResult(Directory.Exists(fullPath) || File.Exists(fullPath));
        }

        private async IAsyncEnumerable<string> InternalSearchAsync(
            string directory,
            IPathActions pathActions,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            _logger.LogInformation("Searching relativePath '{0}'", directory);

            var subDirectories = Directory.GetDirectories(directory);
            var subFiles = Directory.GetFiles(directory);

            foreach (var subDirectory in subDirectories)
            {
                var relativePath = Collect(subDirectory);
                var action = pathActions.DecideAction(relativePath);

                switch (action)
                {
                    case PathAction.Recurse:
                        await foreach (var subItem in InternalSearchAsync(subDirectory, pathActions, cancellationToken))
                            yield return subItem;
                        break;
                    case PathAction.Collect:
                        yield return relativePath;
                        break;
                    case PathAction.Break:
                        yield break;
                }
            }

            foreach (var subFile in subFiles)
            {
                var relativePath = Collect(subFile);
                var action = pathActions.DecideAction(relativePath);

                if (action == PathAction.Collect) yield return relativePath;
            }
        }

        private string Collect(string path)
        {
            return path.Replace(_fileStoreOptions.RootPath, "").TrimStart(Path.DirectorySeparatorChar);
        }
    }
}