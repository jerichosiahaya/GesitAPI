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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IRha _rha;
        private GesitDbContext _db;
        public RhaController(GesitDbContext db, IRha rha, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _rha = rha;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };

        // GET: api/<RhaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _rha.GetAll();
            var files = results.ToList();
            return Ok(new { count = files.Count(), data = files });
        }

        // GET api/<RhaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _rha.GetById(id);
            return Ok(new { data = results });
        }

        // POST & Upload Excel
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] Rha rhafile)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "Rha";
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

                rhafile.FileType = formFile.ContentType;
                rhafile.FileSize = formFile.Length;

                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _rha.CountExistingFileNameRha(lhs); // using DI from data access layer
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
                    rhafile.FileName = newfileName;
                    rhafile.FilePath = newFilePath;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(rhafile);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = rhafile.Id, file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    rhafile.FileName = formFile.FileName;
                    rhafile.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(rhafile);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = rhafile.Id, file_size = formFile.Length, file_path = filePath, logtime = DateTime.Now });
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

        // POST & Read Excel content
        [HttpPost(nameof(UploadExcel))]
        public async Task<IActionResult> UploadExcel([Required] IFormFile formFile, [FromForm] Rha rhafile)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "Rha";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            Directory.CreateDirectory(target);
            try
            {
                // uploaded file length
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

                rhafile.FileType = formFile.ContentType;
                rhafile.FileSize = formFile.Length;

                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _rha.CountExistingFileNameRha(lhs); // using DI from data access layer
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
                    rhafile.FileName = newfileName;
                    rhafile.FilePath = newFilePath;

                    // upload file first
                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(rhafile);
                    }

                    // read file to get the contents
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
                                try
                                {
                                    var obj = result.Tables[0];
                                    var sheetName = obj.TableName;
                                    var objCount = obj.Rows.Count;

                                    for (int fCounter = 0; i < objCount; i++)
                                    {
                                        var rha = new SubRha(); // DI from Models
                                        rha.DivisiBaru = obj.Rows[fCounter][0].ToString();
                                        rha.UicBaru = obj.Rows[fCounter][1].ToString();
                                        rha.NamaAudit = obj.Rows[fCounter][2].ToString();
                                        rha.Lokasi = obj.Rows[fCounter][3].ToString();
                                        rha.Nomor = Convert.ToInt32(obj.Rows[fCounter][4]);
                                        rha.Masalah = obj.Rows[fCounter][5].ToString();
                                        rha.Pendapat = obj.Rows[fCounter][6].ToString();
                                        rha.Status = obj.Rows[fCounter][7].ToString(); ;
                                        rha.TahunTemuan = Convert.ToInt32(obj.Rows[fCounter][9]);
                                        rha.Assign = obj.Rows[fCounter][10].ToString();
                                        rha.RhaId = rhafile.Id;
                                        _db.SubRhas.Add(rha);
                                    }
                                    await _db.SaveChangesAsync();
                                    return Ok(new { status = true, file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList(), count = objCount, sheet_name = sheetName, data = obj });
                                }
                                catch (DbUpdateException dbEx)
                                {
                                    throw new Exception(dbEx.Message);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                    //return Ok(new { status = "Success", message = "File successfully uploaded", file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    rhafile.FileName = formFile.FileName;
                    rhafile.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(rhafile);
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
