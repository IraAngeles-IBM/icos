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


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment IWebHostEnvironment)
        {
            _logger = logger;
            _environment = IWebHostEnvironment;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("UploadAttachment")]
        public ActionResult UploadAttachment(IFormFile formfile)
        {
            int ret = 0;
            var file = formfile;

            if (file.Length > 0)
            {
                foreach (var f in Request.Form.Files)
                {
                    var fileName = f.FileName;
                    var folder = "";
                    var directory = Path.Combine(_environment.WebRootPath, folder);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var fullPath = Path.Combine(_environment.WebRootPath, folder) + $@"\{fileName}";

                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        f.CopyTo(fs);
                        fs.Flush();
                    }

                    //-- Add Amazon Object storage here --//

                    //-- End Amazon Object storage here --//
                }

            }

            return Ok(new { response = ret });
        }
    }
}
