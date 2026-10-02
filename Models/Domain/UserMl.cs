namespace Niwahana_backend.Models.Domain
{
    public class UserMl
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LinkedInLink { get; set; } = string.Empty;
        
    }
}