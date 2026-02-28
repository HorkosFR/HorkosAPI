using HorkosAPI.Report.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorkosAPI.Database.Models;

[Table("Reports")]
public class Report
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TargetType { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Reason { get; set; } = string.Empty;

    public string? Comment { get; set; }
    public string? AdminComment { get; set; }
    public string Status { get; set; } = "New";

    public Guid? UserId { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public Report() { }
    public Report(ReportDTO report)
    {
        Id = Guid.NewGuid();
        TargetId = report.TargetId;
        TargetType = report.TargetType;
        Reason = report.Reason;
        Comment = report.Comment;
        UserId = report.UserId;
        Status = "New";
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        LastUpdatedBy = report.UserId;
    }
}
