using Microsoft.AspNetCore.Http;

namespace Simple.Ecommerce.Contracts.ProductPhotoContracts
{
    public class ProductPhotoUploadRequest
    {
        public int ProductId { get; set; }
        public bool Compress { get; set; } = true;
        public bool Deletable { get; set; } = true;
        public IFormFile File { get; set; } = null;
    }
}
