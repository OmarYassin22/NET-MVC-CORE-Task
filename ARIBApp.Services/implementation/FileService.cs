using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ARIBApp.Services.implementation.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(IFormFile file, string folder = "employees")
    {
        if (file == null || file.Length == 0)
            return GetDefaultImagePath();

         var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", folder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

         var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

         using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

         return $"/images/{folder}/{fileName}";
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath) || imagePath == GetDefaultImagePath())
            return;

        var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public string GetDefaultImagePath()
    {
        return "/images/default.png";
    }
}