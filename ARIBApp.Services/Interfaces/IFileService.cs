using Microsoft.AspNetCore.Http;

namespace ARIBApp.Core.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveImageAsync(IFormFile file, string folder = "employees");
    void DeleteImage(string imagePath);
    string GetDefaultImagePath();
}