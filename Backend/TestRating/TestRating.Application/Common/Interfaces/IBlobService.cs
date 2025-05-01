using TestRating.Application.Contacts.File;

namespace TestRating.Application.Common.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadBlob(FileModel file, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> UploadBlobs(string pathFolder, List<FileModel> files, CancellationToken cancellationToken = default);

        Task<string?> GetBlobUrl(string path, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetBlobFolder(string path, CancellationToken cancellationToken = default);

        Task DeleteBlob(string path, CancellationToken cancellationToken = default);

        Task DeleteBlobFolder(string path, CancellationToken cancellationToken = default);
    }
}
