namespace GLMS.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension == ".pdf";
        }

        public async Task<string> SaveContractFileAsync(IFormFile file)
        {
            if (!IsValidPdf(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // Save to wwwroot/uploads/contracts/
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "contracts");
            Directory.CreateDirectory(uploadsFolder);

            // UUID filename to prevent overwrites
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return relative path for storage in DB
            return Path.Combine("uploads", "contracts", uniqueFileName);
        }
    }
}