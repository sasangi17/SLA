namespace Niwahana_backend.Models.Dto
{
    public class BungalowsDto
    {
        public int BungalowId { get; set; }
        public string BungalowName { get; set; } = string.Empty;
        public string BungalowCode { get; set; } = string.Empty;
        public string BungalowLocation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
