using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Text;
using System.Data;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace fileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWebHostEnvironment _environment;

        //-- Add Amazon Object storage here --//

        const string bucketName = "dotnet-icos";

        public static AmazonS3Config S3Config = new AmazonS3Config
        {
            // ServiceURL = "https://s3.private.jp-tok.cloud-object-storage.appdomain.cloud"
            ServiceURL = "https://s3.jp-tok.cloud-object-storage.appdomain.cloud"
        };

        const string accessKeyId = "b07bdb7f374648da93727ec216015387";
        const string secretAccessKey = "564b2e8ab4d2dcc11343ee3d8350a92d0e27ddab7400e5e0";
        public static IAmazonS3 s3Client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment IWebHostEnvironment)
        {
            _logger = logger;
            _environment = IWebHostEnvironment;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            Console.WriteLine("Get Weather Forecast");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("UploadAttachment")]
        public async Task<ActionResult> UploadAttachment(IFormFile formfile)
        {
            int ret = 0;
            var file = formfile;

            

            if (file.Length > 0)
            {
                foreach (var f in Request.Form.Files)
                {
                    
                    var fileName = f.FileName;
                    var folder = "";
                    // var directory = Path.Combine(_environment.WebRootPath, folder);
                    // var directory = Path.Combine(folder);

                    // if (!Directory.Exists(directory))
                    //     Directory.CreateDirectory(directory);

                    var fullPath = Path.Combine(_environment.WebRootPath, folder) + $@"\{fileName}";
                    // var fullPath = Path.Combine(folder) + $@"\{fileName}";
                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        f.CopyTo(fs);
                        fs.Flush();
                    }


                    //-- Add Amazon Object storage here --//


                    using (AmazonS3Client s3Client = new AmazonS3Client(accessKeyId , secretAccessKey ,S3Config))
                    {
                        Console.WriteLine("Writing an object");
                        PutObjectRequest objectRequest = new PutObjectRequest()
                        {

                            // FilePath = "/Users/isaias/Documents/Projects/Developer_Advocate_Group/CODE_PATTERNS/icos-ira/fileUpload/fileUpload/test.png",
                            // ContentBody = "this is a test",
                            FilePath = fullPath,
                            Key = fileName,
                            // BucketName = bucketName,
                            BucketName = "dotnet-icos",
                            // Key = "test.png"
                        };

                        
                        await s3Client.PutObjectAsync(objectRequest);

                    }

                    //-- End Amazon Object storage here --//



                }

            }
            Console.WriteLine("Done");
            return Ok(new { response = ret });
            // return;
        }
    }
}
