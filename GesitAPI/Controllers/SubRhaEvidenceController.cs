using GesitAPI.Data;
using GesitAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubRhaEvidenceController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ISubRhaEvidence _subRhaEvidence;
        private GesitDbContext _db;
        public SubRhaEvidenceController(GesitDbContext db, ISubRhaEvidence subRhaEvidence, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _subRhaEvidence = subRhaEvidence;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };

        // GET: api/<SubRhaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _subRhaEvidence.GetAll();
            var files = results.ToList();
            return Ok(new { count = files.Count(), data = files });
        }

        // GET api/<SubRhaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _subRhaEvidence.GetById(id);
            return Ok(new { data = results });
        }

        // POST & Upload Excel file
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] SubRhaevidence subRhaEvidencefile)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "SubRhaEvidence";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            Directory.CreateDirectory(target);
            try
            {
                if (formFile.Length <= 0)
                {
                    return BadRequest(new { status = "Error", message = "File is empty" });
                }
                else if (formFile.Length > 2000000)
                {
                    return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
                }

                string s = formFile.FileName;
                int i = s.LastIndexOf('.');
                string lhs = i < 0 ? s : s.Substring(0, i), rhs = i < 0 ? "" : s.Substring(i + 1);
                if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
                {
                    return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
                }
                var filePath = Path.Combine(target, formFile.FileName);

                subRhaEvidencefile.FileType = formFile.ContentType;
                subRhaEvidencefile.FileSize = formFile.Length;

                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _subRhaEvidence.CountExistingFileNameSubRhaEvidence(lhs); // using DI from data access layer
                    var countDuplicateNames = duplicateNames.Count();
                    var value = countDuplicateNames + 1;

                    // getting duplicated name into array
                    var listduplicateNames = duplicateNames.ToList();
                    List<string> arrDuplicatedNames = new List<string>();
                    listduplicateNames.ForEach(file =>
                    {
                        var dupNames = file.FileName;
                        arrDuplicatedNames.Add(dupNames);
                    });

                    // generating new file name
                    var newfileName = String.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(filePath), value, Path.GetExtension(filePath));
                    var newFilePath = Path.Combine(target, newfileName);
                    subRhaEvidencefile.FileName = newfileName;
                    subRhaEvidencefile.FilePath = newFilePath;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _subRhaEvidence.Insert(subRhaEvidencefile);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    subRhaEvidencefile.FileName = formFile.FileName;
                    subRhaEvidencefile.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _subRhaEvidence.Insert(subRhaEvidencefile);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", file_size = formFile.Length, file_path = filePath, logtime = DateTime.Now });
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
