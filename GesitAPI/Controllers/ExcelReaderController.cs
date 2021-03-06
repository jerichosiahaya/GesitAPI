using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelReaderController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ExcelReaderController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Get([Required] IFormFile file)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "ExcelReader";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            Directory.CreateDirectory(target);
            var filePath = Path.Combine(target, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var result = reader.AsDataSet(conf);

                    if (result != null)
                    {
                        var obj = result.Tables[0];
                        var sheetName = obj.TableName;
                        var objCount = obj.Rows.Count;
                        return Ok(new { status = true, count = objCount, sheet_name = sheetName, data = obj.Rows });
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
    }
}
