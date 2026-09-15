using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Himapp.Shared.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Himapp.Execution.Application.Controllers
{
    [ApiController]
    [Route("v1/upload")]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly RegionEndpoint region = RegionEndpoint.APSouth1;

        public FileUploadController(IWebHostEnvironment environment,IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _s3Client = new AmazonS3Client(AWSConfiguration.AWSCredentials, region);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            using var memStream = new MemoryStream();
            await file.CopyToAsync(memStream);
            memStream.Position = 0;

            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string originalFileName = file.FileName;

            string bucketName = AWSConfiguration.BucketName;

            try
            {
                var s3Client = new AmazonS3Client(AWSConfiguration.AWSCredentials, region);
                var transferUtility = new TransferUtility(s3Client);

                await transferUtility.UploadAsync(memStream, bucketName, storedFileName);
            }
            catch (AmazonS3Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            // IMPORTANT: return stored + original filename
            return Ok(new
            {
                storedFileName,
                originalFileName
            });
        }
    }
}