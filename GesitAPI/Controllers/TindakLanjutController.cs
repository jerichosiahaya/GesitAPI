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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TindakLanjutController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ITindakLanjut _tindakLanjut;
        public TindakLanjutController(ITindakLanjut tindakLanjut, IWebHostEnvironment hostingEnvironment)
        {
            _tindakLanjut = tindakLanjut;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _tindakLanjut.GetAll();
            var files = results.ToList();
            return Ok(new { count = files.Count(), data = files });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _tindakLanjut.GetById(id);
            return Ok(new { data = results });
        }

        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file, [FromForm] TindakLanjut tindakLanjut)
        {
            var subDirectory1 = "UploadedFiles";
            var subDirectory2 = "TindakLanjut";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory1, subDirectory2);
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
                tindakLanjut.FileType = file.ContentType;
                tindakLanjut.FileSize = file.Length;
                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _tindakLanjut.CountExistingFileNameTindakLanjut(lhs); // using DI from data access layer
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
                    tindakLanjut.FileName = newfileName;
                    tindakLanjut.FilePath = newFilePath;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await _tindakLanjut.Insert(tindakLanjut);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = tindakLanjut.Id, file_name = newfileName, file_size = file.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    tindakLanjut.FileName = file.FileName;
                    tindakLanjut.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await _tindakLanjut.Insert(tindakLanjut);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = tindakLanjut.Id, file_size = file.Length, file_path = filePath, logtime = DateTime.Now });
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

        // download tindak lanjut file
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> DownloadSingleFile(int id)
        {
            var results = await _tindakLanjut.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });

            var path = results.FilePath;
            var fileName = results.FileName;
            var fileType = results.FileType;
            //byte[] arr = File.ReadAllBytes(fileName);
            //var files = Directory.GetFiles(path);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            byte[] arr = memory.ToArray();
            memory.Position = 0;
            return File(memory, fileType, fileName);
            //return Ok(new { data = results });
            //return File.ReadAllBytes(fileName);
        }

        [HttpGet("DownloadMultiple/{rhaId}")]
        public async Task<IActionResult> GetBundleFiles(string rhaId)
        {
            List<byte[]> filesPath = new List<byte[]>();
            var results = await _tindakLanjut.GetByRhaID(rhaId);
            var files = results.ToList();
            if (files.Count == 0)
                return Ok(new { status = "null", message = "Empty data" });

            files.ForEach(file =>
            {
                var fPath = file.FilePath;
                byte[] bytes = Encoding.ASCII.GetBytes(fPath);
                filesPath.Add(bytes);
            });
            return DownloadMultipleFiles(filesPath);
        }

        // This only a function, not controller
        private FileResult DownloadMultipleFiles(List<byte[]> byteArrayList)
        {
            var zipName = $"archive-EvidenceFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in byteArrayList)
                    {
                        string fPath = Encoding.ASCII.GetString(file);
                        var entry = archive.CreateEntry(Path.GetFileName(fPath), CompressionLevel.Fastest);
                        using (var zipStream = entry.Open())
                        {
                            var bytes = System.IO.File.ReadAllBytes(fPath);
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }

    }
}
