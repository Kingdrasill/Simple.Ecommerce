using Microsoft.AspNetCore.Http;

namespace Simple.Ecommerce.Contracts.UserPhotoContracts
{
    public class UserPhotoUploadRequest
    {
        public int Id { get; set; }
        public bool Compress { get; set; } = true;
        public bool Deletable { get; set; } = true;
        public IFormFile File { get; set; } = null;
    }
}
