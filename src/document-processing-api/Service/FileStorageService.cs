
namespace OfficeToPdf.API.Service
{
    /// <summary>
    /// Hanlde file storage
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private string rootPath = "../FileData/";


        /// <summary>
        /// Downaload the requested file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<Stream?> DownloadFile(string fileName, string folder)
        {
            string filePath = rootPath + folder + "/" + fileName;
            if (System.IO.File.Exists(filePath))
            {
                FileStream docStream = new FileStream(filePath, FileMode.Open);
                return docStream;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string folder = Guid.NewGuid().ToString();
            string filePath = rootPath + folder;
            // Check if the folder does not exist
            if (!Directory.Exists(filePath))
            {
                // Create the folder
                Directory.CreateDirectory(filePath);
            }
            string fileExtension = Path.GetExtension(file.FileName);
            string inputFile = folder + fileExtension;
            //Get input file
            FileStream docStream = new FileStream(filePath + "/" + inputFile, FileMode.CreateNew);
            await file.CopyToAsync(docStream);
            docStream.Position = 0;
            docStream.Close();
            return inputFile;
        }

        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(IFormFile file, string fileName, string folderName)
        {
            string folder = folderName;
            string filePath = rootPath + folder;
            // Check if the folder does not exist
            if (!Directory.Exists(filePath))
            {
                // Create the folder
                Directory.CreateDirectory(filePath);
            }

            //Get input file
            FileStream docStream = new FileStream(filePath + "/" + fileName, FileMode.CreateNew);
            await file.CopyToAsync(docStream);
            docStream.Position = 0;
            docStream.Close();
            return fileName;
        }

        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            string folder = folderName;
            string filePath = rootPath + folder;
            // Check if the folder does not exist
            if (!Directory.Exists(filePath))
            {
                // Create the folder
                Directory.CreateDirectory(filePath);
            }
            string fileExtension = Path.GetExtension(file.FileName);
            string inputFile = Guid.NewGuid().ToString() + fileExtension;
            //Get input file
            FileStream docStream = new FileStream(filePath + "/" + inputFile, FileMode.CreateNew);
            await file.CopyToAsync(docStream);
            docStream.Position = 0;
            docStream.Close();
            return inputFile;
        }

        /// <summary>
        /// Upload the file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileExtension"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(Stream file, string fileExtension, string folder)
        {
            string filePath = rootPath + folder;

            if (!Directory.Exists(filePath))
            {
                // Create the folder
                Directory.CreateDirectory(filePath);
            }
            string inputFile = Guid.NewGuid().ToString() + fileExtension;
            //Get input file
            FileStream docStream = new FileStream(filePath + "/" + inputFile, FileMode.CreateNew);
            await file.CopyToAsync(docStream);
            docStream.Position = 0;
            docStream.Close();
            return inputFile;
        }

        /// <summary>
        /// Delete entire folder
        /// </summary>
        /// <param name="folderName"></param>
        public void DeleteFolder(string folderName)
        {
            string filePath = rootPath + folderName;
            if (Directory.Exists(filePath))
            {
                string[] files = Directory.GetFiles(filePath);
                foreach (string file in files)
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                // Delete the root directory
                Directory.Delete(filePath);
            }
        }

        /// <summary>
        /// Get root path of the storage
        /// </summary>
        /// <returns></returns>
        public string GetRootFolder()
        {
            return rootPath;
        }
    }
}
