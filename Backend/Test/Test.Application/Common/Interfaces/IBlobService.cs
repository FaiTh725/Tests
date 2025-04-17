using Test.Application.Contracts.File;

namespace Test.Application.Common.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadBlob(FileModel file, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> UploadBlobs(List<FileModel> files, CancellationToken cancellationToken = default);

        Task<string?> GetBlobUrl(string path, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetBlobFolder(string path, CancellationToken cancellationToken = default);

        Task DeleteBlob(string path, CancellationToken cancellationToken = default);

        Task DeleteBlobFolder(string path, CancellationToken cancellationToken = default);
    }
}
