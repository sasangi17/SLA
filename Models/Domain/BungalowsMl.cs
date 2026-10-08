namespace Niwahana_backend.Models.Domain
{
    public class BungalowsMl
    {
        public int BungalowId { get; set; }
        public string BungalowName { get; set; } = string.Empty;
        public string BungalowCode { get; set; } = string.Empty;
        public string BungalowLocation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CreateUser { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateDateTime { get; set; }
    }
}
