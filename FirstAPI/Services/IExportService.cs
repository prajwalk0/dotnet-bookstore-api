using FirstAPI.DTO;

namespace FirstAPI.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportToExcelAsync();
        Task<byte[]> ExportToPdfAsync();
    }
}
