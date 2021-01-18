namespace Glasswall.IdentityManagementService.Common.Store
{
    public interface IFileStoreOptions
    {
        string RootPath { get; }
        IEncryptionHandler EncryptionHandler { get; }
        byte[] EncryptionSecret { get; }
    }
}