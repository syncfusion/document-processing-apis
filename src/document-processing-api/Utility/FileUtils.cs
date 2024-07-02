namespace DocumentProcessing.API.Utility
{
    internal class FileUtils
    {
        internal static string GenerateUniqueFileName(string originalFileName)
        {
            // Generate a timestamp string using the current date and time
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Extract the file extension from the original file name
            string fileExtension = Path.GetExtension(originalFileName);

            // Generate the unique file name by appending the timestamp to the original file name
            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{timeStamp}{fileExtension}";

            return uniqueFileName;
        }
    }
}
