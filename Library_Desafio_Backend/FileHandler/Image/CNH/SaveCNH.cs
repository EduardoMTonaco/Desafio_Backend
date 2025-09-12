using Library_Desafio_Backend.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.FileHandler.Image.CNH
{
    public class SaveCNH
    {
        private string _fileFolder;
        private string _fileCNHFolder;
        public SaveCNH()
        {
            _fileFolder = "FILES";
            _fileCNHFolder = "CNH";
        }

        public string SaveImage(string base64Image, string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                {
                    throw new ArgumentException("No images send");
                }
                string base64Data = base64Image.Split(',').Last();
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                string imageType = GetImageType(imageBytes);
                if (imageType != "image/png" && imageType != "image/bmp")
                {
                    throw new ArgumentException("Invalid Extension");
                }
                string uploadsFolder = CreateFolder(Id);
                string fileName = new RandomToken().CreateRandomKey(50) + GetFileExtension(imageType);
                string filePath = Path.Combine(uploadsFolder, fileName);

                File.WriteAllBytes(filePath, imageBytes);

                return fileName;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string GetImageType(byte[] imageBytes)
        {
            if (imageBytes.Length < 10)
            {
                throw new ArgumentException("Invalid image");
            }
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
            {
                return "image/png";
            }
            else if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
            {
                return "image/bmp";
            }
            throw new ArgumentException("Invalid image");
        }
        private string GetFileExtension(string imageType)
        {
            switch (imageType)
            {
                case "image/png":
                    return ".png";
                case "image/bmp":
                    return ".bmp";
                default:
                    throw new Exception();
            }
        }
        private string CreateFolder(string Id)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), _fileFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            uploadsFolder = Path.Combine(uploadsFolder, _fileCNHFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            uploadsFolder = Path.Combine(uploadsFolder, Id);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            return uploadsFolder;
        }
    }
}
