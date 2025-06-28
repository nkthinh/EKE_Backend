using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Firebase
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, long userId);
        Task<bool> DeleteImageAsync(string imageUrl);
        //Task<string> pdateProfileImageUrlAsync(Stream imageStream, string fileName, string folder);
    }

}
