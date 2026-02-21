using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Services;
using HorkosAPI.Report.Models;

namespace HorkosAPI.Report.Services
{
    public class ReportService : IReportService
    {
        private readonly DatabaseContext _context;

        public ReportService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Database.Models.Report>> GetReportsAsync() =>
            await _context.Reports
                          .OrderByDescending(r => r.CreatedAt)
                          .ToListAsync();

        public async Task<Database.Models.Report?> GetReportByIdAsync(Guid id) =>
            await _context.Reports
                          .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<Database.Models.Report>> GetNewReportsAsync() =>
            await _context.Reports
                          .Where(r => r.Status.Equals("New"))
                          .OrderByDescending(r => r.CreatedAt)
                          .Take(10)
                          .ToListAsync();

        public async Task<Database.Models.Report> CreateReportAsync(ReportDTO reportDto)
        {
            var report = new Database.Models.Report
            {
                TargetId = reportDto.TargetId,
                TargetType = reportDto.TargetType,
                Reason = reportDto.Reason,
                Comment = reportDto.Comment,
                UserId = reportDto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> UpdateReportAsync(Guid id, ReportDTO updatedReport, Guid currentUser)
        {
            var existing = await _context.Reports.FindAsync(id);
            if (existing == null) return false;

            existing.TargetId = updatedReport.TargetId;
            existing.TargetType = updatedReport.TargetType;
            existing.Reason = updatedReport.Reason;
            existing.AdminComment = updatedReport.AdminComment;
            existing.Status = updatedReport.Status;
            existing.Comment = updatedReport.Comment;
            existing.UserId = updatedReport.UserId;
            existing.LastUpdatedBy = currentUser;
            existing.LastUpdatedAt = DateTime.UtcNow;

            _context.Reports.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PostBanReportItem(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;

            if (report.TargetType == "group")
            {
                var g = await _context.Groups.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.Groups.Update(g);
            }
            if (report.TargetType == "fact")
            {
                var g = await _context.Facts.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.Facts.Update(g);
            }
            if (report.TargetType == "entity")
            {
                var g = await _context.Entities.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.Entities.Update(g);
            }
            if (report.TargetType == "comment")
            {
                var g = await _context.Comments.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.Comments.Update(g);
            }
            if (report.TargetType == "factcheck")
            {
                var g = await _context.FactChecks.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.FactChecks.Update(g);
            }
            if (report.TargetType == "source")
            {
                var g = await _context.Sources.FindAsync(report.TargetId);
                g.IsVisible = false;
                _context.Sources.Update(g);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReportAsync(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
