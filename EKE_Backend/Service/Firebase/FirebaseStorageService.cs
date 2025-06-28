using Firebase.Storage;
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
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly FirebaseStorage _firebaseStorage;
        private readonly ILogger<FirebaseStorageService> _logger;

        public FirebaseStorageService(IConfiguration configuration, ILogger<FirebaseStorageService> logger)
        {
            var bucket = configuration["Firebase:StorageBucket"];
            _firebaseStorage = new FirebaseStorage(bucket);
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, long userId)
        {
            try
            {
                var fileName = $"{folder}/{userId}_{Guid.NewGuid()}_{file.FileName}";

                using var stream = file.OpenReadStream();
                var downloadUrl = await _firebaseStorage
                    .Child(fileName)
                    .PutAsync(stream);

                return downloadUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Firebase");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return true;

                // Extract file path from Firebase URL
                var uri = new Uri(imageUrl);
                var pathSegments = uri.Segments;
                var fileName = Uri.UnescapeDataString(pathSegments.Last().Split('?')[0]);

                await _firebaseStorage.Child(fileName).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Firebase: {ImageUrl}", imageUrl);
                return false;
            }
        }
    }
}
