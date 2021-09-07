using ExcelDataReader;
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
    public class TindakLanjutEvidenceController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ITindakLanjutEvidence _tindakLanjutEvidence;
        private GesitDbContext _db;
        public TindakLanjutEvidenceController(GesitDbContext db, ITindakLanjutEvidence tindakLanjutEvidence, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _tindakLanjutEvidence = tindakLanjutEvidence;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };


        // GET: api/<TindakLanjutEvidenceController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _tindakLanjutEvidence.GetAll();
            var files = results.ToList();
            var filesCount = results.Count();
            if (filesCount == 0)
                return NoContent();
            return Ok(new { count = files.Count(), data = files });
        }

        // GET api/<TindakLanjutEvidenceController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _tindakLanjutEvidence.GetById(id);
            return Ok(new { data = results });
        }

        [HttpGet("GetByTindakLanjutID/{tlId}")]
        public async Task<IActionResult> GetByTindakLanjutID(string tlId)
        {
            var results = await _tindakLanjutEvidence.GetByTindakLanjutID(tlId);
            var files = results.ToList();
            if (files.Count == 0)
                return NotFound(new { status = "null", message = "Empty data" });
            return Ok(new { data = files });
        }

        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file, [FromForm] int tindakLanjutID)
        {
            TindakLanjutEvidence tlEvidence = new TindakLanjutEvidence();
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "TindakLanjutEvidence";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            Directory.CreateDirectory(target);
            try
            {
                if (file.Length <= 0)
                {
                    return BadRequest(new { status = "Error", message = "File is empty" });
                }
                else if (file.Length > 2000000)
                {
                    return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
                }

                string s = file.FileName;
                int i = s.LastIndexOf('.');
                string lhs = i < 0 ? s : s.Substring(0, i), rhs = i < 0 ? "" : s.Substring(i + 1);
                if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
                {
                    return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
                }
                var filePath = Path.Combine(target, file.FileName);

                tlEvidence.FileType = file.ContentType;
                tlEvidence.FileSize = file.Length;
                tlEvidence.TindaklanjutId = tindakLanjutID;

                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _tindakLanjutEvidence.CountExistingFileNameTLEvidence(lhs); // using DI from data access layer
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
                    tlEvidence.FileName = newfileName;
                    tlEvidence.FilePath = newFilePath;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await _tindakLanjutEvidence.Insert(tlEvidence);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", file_name = newfileName, file_size = file.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    tlEvidence.FileName = file.FileName;
                    tlEvidence.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await _tindakLanjutEvidence.Insert(tlEvidence);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", file_size = file.Length, file_path = filePath, logtime = DateTime.Now });
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


        //// PUT api/<TindakLanjutEvidenceController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<TindakLanjutEvidenceController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
