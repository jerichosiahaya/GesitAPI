﻿using AutoMapper;
using ExcelDataReader;
using GesitAPI.Data;
using GesitAPI.Dtos;
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
using static GesitAPI.Dtos.RhaDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IRha _rha;
        private ISubRha _subRha;
        private GesitDbContext _db;
        public RhaController(GesitDbContext db, IRha rha, ISubRha subRha, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _rha = rha;
            _subRha = subRha;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "jpeg", "png", "doc", "docx", "xls", 
            "xlsx", "pdf", "csv", "txt", "zip", "rar", "JPG", "JPEG", "PNG", "DOC", "DOCX", "XLS", 
            "XLSX", "PDF", "CSV", "TXT", "ZIP", "RAR"  };

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _rha.GetAll();
            List<RhaDto> resultData = new List<RhaDto>();

            foreach (var o in results)
            {
                var countSubRha = o.SubRhas.Count;
                var countSubRhaOpen = o.SubRhas.Where(p => p.OpenClose == "Open").Count();
                var countSubRhaClosed = o.SubRhas.Where(p => p.OpenClose == "Closed").Count();
                float completedPercentage = (float)countSubRhaClosed / (float)countSubRha;

                resultData.Add(new RhaDto
                {
                    Id = o.Id,
                    FileName = o.FileName,
                    Kondisi = o.Kondisi,
                    Rekomendasi = o.Rekomendasi,
                    SubKondisi = o.SubKondisi,
                    TargetDate = o.TargetDate,
                    Assign = o.Assign,
                    CreatedBy = o.CreatedBy,
                    StatusJt = o.StatusJt,
                    DirSekor = o.DirSekor,
                    Uic = o.Uic,
                    StatusTemuan = o.StatusTemuan,
                    StatusInfo = new List<StatusInfoRha>()
                    {
                        new StatusInfoRha()
                        {
                            CountSubRha = countSubRha,
                            CountSubRHAClosed = countSubRhaClosed,
                            CountSubRHAOpen = countSubRhaOpen,
                            StatusCompletedPercentage = completedPercentage
                        }
                    }
                });
            }
            return Ok(resultData);
        }

        // to do: pindahkan ke folder dto
        public class ResponseStatusRha
        {
            public int CountRha { get; set; }
            public int CompletedRha { get; set; }
            public int UncompleteRha { get; set; }
            public float PercentageCompletedRha { get; set; }
            public float PercentageUncompleteRha { get; set; }
        }

        [HttpGet(nameof(GetStatusRha))]
        public async Task<IActionResult> GetStatusRha()
        {
            var results = await _rha.GetAll();
            List<RhaDto> resultData = new List<RhaDto>();

            foreach (var o in results)
            {
                var countSubRha = o.SubRhas.Count;
                var countSubRhaOpen = o.SubRhas.Where(p => p.OpenClose == "Open").Count();
                var countSubRhaClosed = o.SubRhas.Where(p => p.OpenClose == "Closed").Count();
                float completedPercentage = (float)countSubRhaClosed / (float)countSubRha;

                resultData.Add(new RhaDto
                {
                    Id = o.Id,
                    FileName = o.FileName,
                    Kondisi = o.Kondisi,
                    Rekomendasi = o.Rekomendasi,
                    SubKondisi = o.SubKondisi,
                    TargetDate = o.TargetDate,
                    Assign = o.Assign,
                    CreatedBy = o.CreatedBy,
                    StatusJt = o.StatusJt,
                    DirSekor = o.DirSekor,
                    Uic = o.Uic,
                    StatusTemuan = o.StatusTemuan,
                    StatusInfo = new List<StatusInfoRha>()
                    {
                        new StatusInfoRha()
                        {
                            CountSubRha = countSubRha,
                            CountSubRHAClosed = countSubRhaClosed,
                            CountSubRHAOpen = countSubRhaOpen,
                            StatusCompletedPercentage = completedPercentage
                        }
                    }
                });
            }

            
            var countRha = resultData.Count();
            var completedRha = resultData.Where(p => p.StatusInfo.Any(o => o.StatusCompletedPercentage == 100)).Count();
            float resultPercentageCompleted = (float)completedRha / (float)countRha;
            ResponseStatusRha responseData = new ResponseStatusRha()
            {
                CountRha = countRha,
                CompletedRha = completedRha,
                UncompleteRha = countRha - completedRha,
                PercentageCompletedRha = resultPercentageCompleted,
                PercentageUncompleteRha = 100-resultPercentageCompleted
            };

            return Ok(responseData);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _rha.GetById(id);
            return Ok(new { data = results });
        }

        [HttpGet("GetBySubRhaAssign/{assign}")]
        public async Task<IActionResult> GetBySubRhaAssign(string assign)
        {
            // check sub rha by assign first
            var check = await _subRha.GetByAssign(assign);
            var checkCount = check.Count();
            if (checkCount == 0)
            {
                return NoContent();
            } else
            {

                //var result = await _db.Rhas.Join(_db.SubRhas, o => o.Id, p => p.RhaId, (o, p) => new { Rha = o, Sub = p })
                //    .Where(i => i.Sub.Assign == assign).Distinct().AsNoTracking().ToListAsync();

                //var result = await _db.Rhas.Select(x => new { Rha = x, Things = x.SubRhas })
                //    .Where(p=>p.Things.a)
                //    .Select(o=>o.Rha)
                //    .AsNoTracking().ToListAsync();

                //var result =  _db.Rhas.SelectMany(r => r.SubRhas, (r, s) => new { r = r, s = s })
                //             .Where(temp0 => (temp0.s.Assign == assign))
                //             .Select(temp0 => temp0.r)
                //             .Distinct();

                var result = _rha.GetSubRHAByAssign(assign);
                List<RhaDto> resultData = new List<RhaDto>();

                foreach (var o in result)
                {
                    var subRhaData = _db.SubRhas.Where(p => p.RhaId == o.Id).ToList(); //  && p.Assign == assign
                    var countSubRha = subRhaData.Count;
                    var countSubRhaOpen = subRhaData.Where(o => o.OpenClose == "Open").Count();
                    var countSubRhaClosed = subRhaData.Where(p => p.OpenClose == "Closed").Count();
                    float completedPercentage = (float)countSubRhaOpen / (float)countSubRha;

                    resultData.Add(new RhaDto
                    {
                        Id = o.Id,
                        FileName = o.FileName,
                        Kondisi = o.Kondisi,
                        Rekomendasi = o.Rekomendasi,
                        SubKondisi = o.SubKondisi,
                        TargetDate = o.TargetDate,
                        Assign = o.Assign,
                        CreatedBy = o.CreatedBy,
                        StatusJt = o.StatusJt,
                        DirSekor = o.DirSekor,
                        Uic = o.Uic,
                        StatusTemuan = o.StatusTemuan,
                        StatusInfo = new List<StatusInfoRha>()
                        {
                            new StatusInfoRha()
                            {
                                CountSubRha = countSubRha,
                                CountSubRHAClosed = countSubRhaClosed,
                                CountSubRHAOpen = countSubRhaOpen,
                                StatusCompletedPercentage = completedPercentage
                            }
                        }
                    });
                }
                return Ok(resultData);
            }
        }

        // download template risalah hasil audit
        [HttpGet(nameof(DownloadTemplate))]
        public async Task<IActionResult> DownloadTemplate()
        {
            var subDirectory1 = "UploadedFiles";
            var subDirectory2 = "Templates";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory1, subDirectory2);
            var fileName = "template_risalah_hasil_audit.xlsx";
            var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var filePath = Path.Combine(target, fileName);
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            byte[] arr = memory.ToArray();
            memory.Position = 0;
            return File(memory, fileType, fileName);
        }

        // download RHA file (excel)
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> DownloadSingleFile(int id)
        {
            var results = await _rha.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });

            var path = results.FilePath;
            var fileName = results.FileName;
            var fileType = results.FileType;
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            byte[] arr = memory.ToArray();
            memory.Position = 0;
            return File(memory, fileType, fileName);
        }

        // POST & upload excel
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] RhaDto rhafile)
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
                else if (formFile.Length > 10000000)
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

                    // TO DO 
                    // using AutoMapper
                    Rha insertData = new Rha();
                    insertData.FileType = formFile.ContentType;
                    insertData.FileSize = formFile.Length;
                    insertData.FileName = newfileName;
                    insertData.FilePath = newFilePath;
                    insertData.Kondisi = rhafile.Kondisi;
                    insertData.Rekomendasi = rhafile.Rekomendasi;
                    insertData.StatusJt = rhafile.StatusJt;
                    insertData.StatusTemuan = rhafile.StatusTemuan;
                    insertData.SubKondisi = rhafile.SubKondisi;
                    insertData.TargetDate = rhafile.TargetDate;
                    insertData.Uic = rhafile.Uic;
                    insertData.Assign = rhafile.Assign;
                    insertData.CreatedBy = rhafile.CreatedBy;
                    insertData.DirSekor = rhafile.DirSekor;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(insertData);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    // TO DO 
                    // using AutoMapper
                    Rha insertData = new Rha();
                    insertData.FileType = formFile.ContentType;
                    insertData.FileSize = formFile.Length;
                    insertData.FileName = formFile.FileName;
                    insertData.FilePath = filePath;
                    insertData.Kondisi = rhafile.Kondisi;
                    insertData.Rekomendasi = rhafile.Rekomendasi;
                    insertData.StatusJt = rhafile.StatusJt;
                    insertData.StatusTemuan = rhafile.StatusTemuan;
                    insertData.SubKondisi = rhafile.SubKondisi;
                    insertData.TargetDate = rhafile.TargetDate;
                    insertData.Uic = rhafile.Uic;
                    insertData.DirSekor = rhafile.DirSekor;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await _rha.Insert(insertData);
                    }
                    return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_size = formFile.Length, file_path = filePath, logtime = DateTime.Now });
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

        // delete RHA (only use this for error handling if sub RHA error)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var results = await _rha.GetById(id);
                if (results == null)
                    return BadRequest(new { status = "Error", message = "There is no such a file" });

                var filePath = results.FilePath;
                System.IO.File.Delete(filePath);
                await _rha.Delete(id.ToString());
                return Ok(new { status = true, message = $"Successfully delete the RHA file with id: {id}"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] RhaDto rha)
        {
            try
            {
                Rha updateData = new Rha();
                updateData.DirSekor = rha.DirSekor;
                updateData.Kondisi = rha.Kondisi;
                updateData.Rekomendasi = rha.Rekomendasi;
                updateData.StatusJt = rha.StatusJt;
                updateData.StatusTemuan = rha.StatusTemuan;
                updateData.SubKondisi = rha.SubKondisi;
                updateData.TargetDate = rha.TargetDate;
                updateData.Uic = rha.Uic;
                updateData.Assign = rha.Assign;
                updateData.CreatedBy = rha.CreatedBy;
                await _rha.Update(id.ToString(), updateData);
                return Ok($"Data {updateData.Id} berhasil diupdate!");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        // POST & Read Excel content
        //[HttpPost(nameof(UploadExcel))]
        //public async Task<IActionResult> UploadExcel([Required] IFormFile formFile, [FromForm] Rha rhafile)
        //{
        //    var subDirectory = "UploadedFiles";
        //    var subDirectory2 = "Rha";
        //    var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
        //    Directory.CreateDirectory(target);
        //    try
        //    {
        //        if (formFile.Length <= 0)
        //        {
        //            return BadRequest(new { status = "Error", message = "File is empty" });
        //        }
        //        else if (formFile.Length > 2000000)
        //        {
        //            return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
        //        }
        //        string s = formFile.FileName;
        //        int i = s.LastIndexOf('.');
        //        string lhs = i < 0 ? s : s.Substring(0, i), rhs = i < 0 ? "" : s.Substring(i + 1);
        //        if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
        //        {
        //            return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
        //        }
        //        var filePath = Path.Combine(target, formFile.FileName);
        //        rhafile.FileType = formFile.ContentType;
        //        rhafile.FileSize = formFile.Length;
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            var duplicateNames = await _rha.CountExistingFileNameRha(lhs);
        //            var countDuplicateNames = duplicateNames.Count();
        //            var value = countDuplicateNames + 1;
        //            var listduplicateNames = duplicateNames.ToList();
        //            List<string> arrDuplicatedNames = new List<string>();
        //            listduplicateNames.ForEach(file =>
        //            {
        //                var dupNames = file.FileName;
        //                arrDuplicatedNames.Add(dupNames);
        //            });
        //            var newfileName = String.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(filePath), value, Path.GetExtension(filePath));
        //            var newFilePath = Path.Combine(target, newfileName);
        //            rhafile.FileName = newfileName;
        //            rhafile.FilePath = newFilePath;
        //            using (var stream = new FileStream(newFilePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }

        //            // read file to get the contents and then insert to another table
        //            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //            using (var stream2 = System.IO.File.Open(newFilePath, FileMode.Open, FileAccess.Read))
        //            {
        //                using (var reader = ExcelReaderFactory.CreateReader(stream2))
        //                {
        //                    var conf = new ExcelDataSetConfiguration
        //                    {
        //                        UseColumnDataType = true,
        //                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
        //                        {
        //                            UseHeaderRow = true
        //                        }
        //                    };

        //                    var result = reader.AsDataSet(conf);

        //                    if (result != null)
        //                    {
        //                        try
        //                        {
        //                            var obj = result.Tables[0];
        //                            var sheetName = obj.TableName;
        //                            var objCount = obj.Rows.Count;
        //                            for (int fCounter = 0; i < objCount; fCounter++)
        //                            {
        //                                rhafile.SubRhas.Add(new SubRha()
        //                                {
        //                                    RhaId = rhafile.Id,
        //                                    DivisiBaru = obj.Rows[fCounter][0].ToString(),
        //                                    UicBaru = obj.Rows[fCounter][1].ToString(),
        //                                    NamaAudit = obj.Rows[fCounter][2].ToString(),
        //                                    Lokasi = obj.Rows[fCounter][3].ToString(),
        //                                    Nomor = Convert.ToInt32(obj.Rows[fCounter][4]),
        //                                    Masalah = obj.Rows[fCounter][5].ToString(),
        //                                    Pendapat = obj.Rows[fCounter][6].ToString(),
        //                                    Status = obj.Rows[fCounter][7].ToString(),
        //                                    TahunTemuan = Convert.ToInt32(obj.Rows[fCounter][9]),
        //                                    Assign = obj.Rows[fCounter][10].ToString()
        //                                });
        //                            }
        //                            _db.Rhas.Add(rhafile);
        //                            await _db.SaveChangesAsync();
        //                            return Ok(new { status = true, file_name = newfileName, file_size = formFile.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList(), count = objCount, sheet_name = sheetName, data = obj });
        //                        }
        //                        catch (DbUpdateException dbEx)
        //                        {
        //                            throw new Exception(dbEx.Message);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            throw new Exception(ex.Message);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return NotFound();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            rhafile.FileName = formFile.FileName;
        //            rhafile.FilePath = filePath;
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //            using (var stream2 = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
        //            {
        //                using (var reader = ExcelReaderFactory.CreateReader(stream2))
        //                {
        //                    var conf = new ExcelDataSetConfiguration
        //                    {
        //                        UseColumnDataType = true,
        //                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
        //                        {
        //                            UseHeaderRow = true
        //                        }
        //                    };

        //                    var result = reader.AsDataSet(conf);

        //                    if (result != null)
        //                    {
        //                        try
        //                        {
        //                            var obj = result.Tables[0];
        //                            var sheetName = obj.TableName;
        //                            var objCount = obj.Rows.Count;
        //                            for (int fCounter = 0; i < objCount; fCounter++)
        //                            {
        //                                rhafile.SubRhas.Add(new SubRha()
        //                                {
        //                                    RhaId = rhafile.Id,
        //                                    DivisiBaru = obj.Rows[fCounter][0].ToString(),
        //                                    UicBaru = obj.Rows[fCounter][1].ToString(),
        //                                    NamaAudit = obj.Rows[fCounter][2].ToString(),
        //                                    Lokasi = obj.Rows[fCounter][3].ToString(),
        //                                    Nomor = Convert.ToInt32(obj.Rows[fCounter][4]),
        //                                    Masalah = obj.Rows[fCounter][5].ToString(),
        //                                    Pendapat = obj.Rows[fCounter][6].ToString(),
        //                                    Status = obj.Rows[fCounter][7].ToString(),
        //                                    TahunTemuan = Convert.ToInt32(obj.Rows[fCounter][9]),
        //                                    Assign = obj.Rows[fCounter][10].ToString()
        //                                });
        //                            }
        //                            _db.Rhas.Add(rhafile);
        //                            await _db.SaveChangesAsync();
        //                            return Ok(new { status = true, file_name = formFile.FileName, file_size = formFile.Length, file_path = filePath, logtime = DateTime.Now, count = objCount, sheet_name = sheetName, data = obj });
        //                        }
        //                        catch (DbUpdateException dbEx)
        //                        {
        //                            throw new Exception(dbEx.Message);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            throw new Exception(ex.Message);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return NotFound();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        throw new Exception(dbEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
