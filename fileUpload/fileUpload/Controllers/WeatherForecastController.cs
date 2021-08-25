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
using System.Web;

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

        // --- link generator --- //

        const string httpMethod = "GET";
        const string host = "s3.jp-tok.cloud-object-storage.appdomain.cloud";
        const string region = "";
        const string endpoint = "https://" + host;
        const string bucket = "dotnet-icos";
        const string objectKey = "Samplepdf.pdf";
        int expiration = 86400;  // time in seconds



        //-- Add Amazon Object storage here --//

        const string bucketName = "dotnet-icos";

        public static AmazonS3Config S3Config = new AmazonS3Config
        {
            // ServiceURL = "https://s3.private.jp-tok.cloud-object-storage.appdomain.cloud"
            ServiceURL = "https://s3.jp-tok.cloud-object-storage.appdomain.cloud"
        };

        const string accessKeyId = "b07bdb7f374648da93727ec216015387";
        const string secretAccessKey = "564b2e8ab4d2dcc11343ee3d8350a92d0e27ddab7400e5e0";
        const string keyName = "Coffee Bag 13.jpg";
        public static IAmazonS3 s3Client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment IWebHostEnvironment)
        {
            _logger = logger;
            _environment = IWebHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var rng = new Random();
            int ret = 0;
            Console.WriteLine("Get file");


            using (AmazonS3Client s3Client = new AmazonS3Client(accessKeyId , secretAccessKey ,S3Config))
            {


                // create object request object
                GetObjectRequest requestDownload = new GetObjectRequest
                { 
                BucketName = bucketName, 
                Key = keyName
                };

                // download physical file
                using (GetObjectResponse response = await s3Client.GetObjectAsync(requestDownload))
                {
                    string dest = Path.Combine(_environment.WebRootPath, "") + $@"\{keyName}";
                    using (Stream responseStream = response.ResponseStream)
                    using (FileStream outFile = System.IO.File.Create(dest))
                    {
                        responseStream.CopyTo(outFile);
                    }
                }

            }
            Console.WriteLine("Filedownloaded, {0}", keyName);
            return Ok(new { response = ret });
            // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = DateTime.Now.AddDays(index),
            //     TemperatureC = rng.Next(-20, 55),
            //     Summary = Summaries[rng.Next(Summaries.Length)]
            // })
            // .ToArray();
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

                    FileInfo tempFile = new FileInfo(fullPath);
                    tempFile.Delete();

                }

            }
            Console.WriteLine("Done");
            return Ok(new { response = ret });
            // return;
        }

        [HttpGet("GetLink")]
        public ActionResult GetLink()
        {
            int ret = 0;
            var time = DateTime.UtcNow;
            // string timestamp = time.ToString("yyyyMMddHHmmss") + "Z";
            // string datestamp = time.ToString("yyyyMMdd");
            const string  timestamp = "20210825T043429Z";
            const string  datestamp = "20210825";


            var standardizedQuerystring = "X-Amz-Algorithm=AWS4-HMAC-SHA256" +
                "&X-Amz-Credential=" + System.Web.HttpUtility.UrlEncode(accessKeyId + "/" + datestamp + "/" + region + "/s3/aws4_request") +
                "&X-Amz-Date=" + timestamp +
                "&X-Amz-Expires=" + expiration.ToString() +
                "&X-Amz-SignedHeaders=host";

            var standardizedResource = "/" + bucket + "/" + objectKey;

            var payloadHash = "UNSIGNED-PAYLOAD";
            var standardizedHeaders = "host:" + host;
            var signedHeaders = "host";

            var standardizedRequest = httpMethod + "\n" +
                standardizedResource + "\n" +
                standardizedQuerystring + "\n" +
                standardizedHeaders + "\n" +
                "\n" +
                signedHeaders + "\n" +
                payloadHash;

            // assemble string-to-sign
            string hashingAlgorithm = "AWS4-HMAC-SHA256";
            string credentialScope = datestamp + "/" + region + "/" + "s3" + "/" + "aws4_request";
            // string sts = hashingAlgorithm + "\n" +
            //     timestamp + "\n" +
            //     credentialScope + "\n" +
            //     Crypto.hashHex(standardizedRequest);

            string sts = hashingAlgorithm + "\n" +
                timestamp + "\n" +
                credentialScope + "\n" + "8442242f88793e7a7f5586cf592fd72966599c8489461803ce987b27f648745a";
                // Crypto.hashHex(standardizedRequest);


            // generate the signature
            byte[] signatureKey = Crypto.createSignatureKey(secretAccessKey, datestamp, region, "s3");

            // string signature = Crypto.hmacHex(signatureKey, sts);
            // string signature = Crypto.hmacHex(BitConverter.ToString(signatureKey).Replace("-", string.Empty).ToLower(), sts);        
            string signature = Crypto.hmacHex(signatureKey, sts);

            // create and send the request
            // the 'requests' package autmatically adds the required 'host' header
            string requestUrl = endpoint + "/" +
                bucket + "/" +
                objectKey + "?" +
                standardizedQuerystring +
                "&X-Amz-Signature=" +
                signature;


            Console.WriteLine("Get Link");


            // byte[] tempHash = Crypto.hash(secretAccessKey, "test");
            // Console.WriteLine("Hash value {0}, {1}, {2}", BitConverter.ToString(tempHash).Replace("-", string.Empty).ToLower() , timestamp, datestamp);
            Console.WriteLine("Request URL = {0}", requestUrl);
            Console.WriteLine("signatureKey = {0}", BitConverter.ToString(signatureKey).Replace("-", string.Empty).ToLower());
            Console.WriteLine("signature = {0}", signature);
             Console.WriteLine("standardizedQuerystring = {0}", standardizedQuerystring);
            Console.WriteLine("standardizedRequest = {0} \n {1}", standardizedRequest, Crypto.hashHex(standardizedRequest));
            Console.WriteLine("sts = {0}", sts);
            Console.WriteLine("\n\nhasHex = {0}", Crypto.hashHex("test"));
            // Console.WriteLine("\n\nhmacHex = {0}", Crypto.hmacHex(secretAccessKey, "test"));


            // Console.WriteLine("\n\nhasHex = {0}", Crypto.hash(secretAccessKey, "test"));
            return Ok(new { response = ret });
        }

    }
}
