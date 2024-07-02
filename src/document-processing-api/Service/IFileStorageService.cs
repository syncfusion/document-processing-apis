namespace OfficeToPdf.API.Service
{
    /// <summary>
    /// Hanlde file storage
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file, string fileName, string folderName);
        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file);
        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileExtension"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(Stream file, string fileExtension, string? folderName);
        /// <summary>
        /// Download the requested file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        Task<Stream> DownloadFile(string? fileName, string? folderName);
        /// <summary>
        /// Delete the entire folder
        /// </summary>
        /// <param name="folderName"></param>
        void DeleteFolder(string folderName);
        /// <summary>
        /// Get root folder
        /// </summary>
        /// <returns></returns>
        string GetRootFolder();
    }
}
