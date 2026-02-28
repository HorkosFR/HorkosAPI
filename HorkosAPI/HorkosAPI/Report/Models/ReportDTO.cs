namespace HorkosAPI.Report.Models
{
    public class ReportDTO
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; } = "";
        public string Reason { get; set; } = "";
        public string? Comment { get; set; }
        public string? AdminComment { get; set; }
        public string Status { get; set; }
        public Guid? UserId { get; set; }
    }
}
