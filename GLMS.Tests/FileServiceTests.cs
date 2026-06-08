using System.IO;
using System.Text;
using System.Threading.Tasks;
using GLMS.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GLMS.Tests
{
    public class FileServiceTests
    {
        private IFormFile CreateFakeFile(string fileName, string content = "fake content")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        private FileService GetService()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            return new FileService(mockEnv.Object);
        }

        [Fact]
        public void IsValidPdf_WithPdfFile_ReturnsTrue()
        {
            var service = GetService();
            var file = CreateFakeFile("contract.pdf");
            Assert.True(service.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WithExeFile_ReturnsFalse()
        {
            var service = GetService();
            var file = CreateFakeFile("virus.exe");
            Assert.False(service.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WithDocxFile_ReturnsFalse()
        {
            var service = GetService();
            var file = CreateFakeFile("document.docx");
            Assert.False(service.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WithNullFile_ReturnsFalse()
        {
            var service = GetService();
            Assert.False(service.IsValidPdf(null!));
        }

        [Fact]
        public async Task SaveContractFileAsync_WithExeFile_ThrowsException()
        {
            var service = GetService();
            var file = CreateFakeFile("virus.exe");
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.SaveContractFileAsync(file));
        }
    }
}