namespace Niwahana_backend.Models.Domain
{
    public class ProfileImageMl
    {
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ContentType { get; set; }
    }
}