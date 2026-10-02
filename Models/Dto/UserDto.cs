namespace Niwahana_backend.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LinkedInLink { get; set; } = string.Empty;
        public string StaffId { get; set; } = string.Empty;
    }
}