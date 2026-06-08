namespace GLMS.Web.Services
{
    public interface IFileService
    {
        Task<string> SaveContractFileAsync(IFormFile file);
        bool IsValidPdf(IFormFile file);
    }
}