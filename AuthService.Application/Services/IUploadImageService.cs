using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Services;

public interface IUploadImageService
{
    Task<string> UploadImageAsync(IFormFile file);

    bool DeleteImage(string url);
}