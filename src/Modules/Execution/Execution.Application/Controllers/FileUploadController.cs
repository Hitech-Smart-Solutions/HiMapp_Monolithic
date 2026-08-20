using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Himapp.Execution.Application.Controllers
{
    [ApiController]
    [Route("v1/upload")]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string _region;

        public FileUploadController(IWebHostEnvironment environment,IConfiguration configuration,IAmazonS3 s3Client)
        {
            _environment = environment;
            _configuration = configuration;
            _s3Client = s3Client;
            _region = _configuration["AWS:Region"] ?? "ap-south-1";
        }

        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file was uploaded." });

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest(new { message = "Only image files are allowed." });

            using var memStream = new MemoryStream();
            await file.CopyToAsync(memStream);
            memStream.Position = 0;

            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string bucketName = _configuration["AWS:BucketName"]; // or from IOptions

            try
            {
                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(memStream, bucketName, storedFileName);
            }
            catch (AmazonS3Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            // Build the public URL (if bucket is public or you want the object URL)
            // Option 1: Use the bucket's public URL (if public-read)
            var fileUrl = $"https://{bucketName}.s3.{_region}.amazonaws.com/{storedFileName}";

            // Option 2: Generate a pre‑signed URL (if private bucket) – see below

            return Ok(new
            {
                url = fileUrl,
                fileName = storedFileName
            });
        }
    }
}