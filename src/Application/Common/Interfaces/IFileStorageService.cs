namespace Application.Common.Interfaces;

/// <summary>
/// File storage service interface.
/// Abstracts file storage operations for credential uploads, etc.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Save a file to storage and return the relative path.
    /// </summary>
    /// <param name="fileStream">The file stream to save.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="subFolder">Sub-folder path (e.g., "credentials/{userId}").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Relative file path for storage in DB.</returns>
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string subFolder,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from storage.
    /// </summary>
    /// <param name="relativePath">The relative path of the file to delete.</param>
    /// <returns>True if file was deleted, false if not found.</returns>
    bool DeleteFile(string relativePath);

    /// <summary>
    /// Check if a file exists.
    /// </summary>
    /// <param name="relativePath">The relative path of the file.</param>
    /// <returns>True if the file exists.</returns>
    bool FileExists(string relativePath);
}
