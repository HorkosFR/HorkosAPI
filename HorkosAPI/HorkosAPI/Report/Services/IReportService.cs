using HorkosAPI.Report.Models;

namespace HorkosAPI.Report.Services
{
    public interface IReportService
    {
        Task<List<Database.Models.Report>> GetReportsAsync();
        Task<Database.Models.Report?> GetReportByIdAsync(Guid id);
        Task<List<Database.Models.Report>> GetNewReportsAsync();
        Task<Database.Models.Report> CreateReportAsync(ReportDTO report);
        Task<bool> UpdateReportAsync(Guid id, ReportDTO report, Guid currentUser);
        Task<bool> DeleteReportAsync(Guid id);
        Task<bool> PostBanReportItem(Guid id);
    }
}
