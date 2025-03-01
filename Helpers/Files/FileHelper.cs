using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Files
{
    public class FileHelper
    {
        private readonly string _profileImagePath;

        public FileHelper() {}
        
        public FileHelper(string webRootPath)
        {
            _profileImagePath = Path.Combine(webRootPath, "profile_images");

            // Ensure directory exists
            if (!Directory.Exists(_profileImagePath))
            {
                Directory.CreateDirectory(_profileImagePath);
            }
        }

        public async Task<string?> UploadProfileImage(IFormFile? imageFile, string? existingFileName)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return existingFileName; // No new file uploaded, keep existing
            }

            // Delete existing image if available
            if (!string.IsNullOrEmpty(existingFileName))
            {
                string existingFilePath = Path.Combine(_profileImagePath, existingFileName);
                if (File.Exists(existingFilePath))
                {
                    File.Delete(existingFilePath);
                }
            }

            // Generate new file name (UUID + extension)
            string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            string newFilePath = Path.Combine(_profileImagePath, newFileName);

            // Save the new image
            await using var stream = new FileStream(newFilePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return newFileName; // Return new file name to store in the database
        }


        public void Test()
        {
            FileSystemInfo fileInfo = new FileInfo("");
        }

        static void ShowWindowsDirectoryInfo()
        {
            // Dump directory information. If you are not on Windows, plug in another directory
            DirectoryInfo dir = new DirectoryInfo($@"C{Path.VolumeSeparatorChar}{Path.
            DirectorySeparatorChar}Windows");
            Console.WriteLine("***** Directory Info *****");
            Console.WriteLine("FullName: {0}", dir.FullName);
            Console.WriteLine("Name: {0}", dir.Name);
            Console.WriteLine("Parent: {0}", dir.Parent);
            Console.WriteLine("Creation: {0}", dir.CreationTime);
            Console.WriteLine("Attributes: {0}", dir.Attributes);
            Console.WriteLine("Root: {0}", dir.Root);
            Console.WriteLine("**************************\n");
        }
    }
}