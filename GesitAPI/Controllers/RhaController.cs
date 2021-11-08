using AutoMapper;
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
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GesitAPI.Dtos.RhaDto;

// Author: Jericho Siahaya
// Created: August 2021

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

        List<string> allowedFileExtensionsXlsx = new List<string>() {  "xls","xlsx","XLS","XLSX",  };

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
                return BadRequest(new { status = "Fail", message = "Libur nih, gak ada kerjaan.."});
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
        
        // DEPRECATED
        // delete only one rha that doesnt have any foreign key to subrha
        [Obsolete]
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

        // update rha
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

        // delete all (rha -> subrha -> subrhaevidences -> subrhaimages -> tindaklanjuts -> tindaklanjutsevidences)
        [HttpDelete("DeleteAll/{id}")]
        public async Task<IActionResult> DeleteAll(string id)
        {
           var results = await _rha.GetById(id);
           if (results == null)
            {
                return BadRequest(new { status = "Error", message = "There is no such a file" });
            } else
            {
                await _rha.DeleteAll(id.ToString());
                return Ok(new { status = true, message = $"Successfully delete the RHA file with id: {id}" });
            }
               
        }

        // upload rha with subrha
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file, [FromForm] RhaDto rhafile)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "Rha";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            Directory.CreateDirectory(target);


            if (file.Length <= 0)
            {
                return BadRequest(new { status = "Error", message = "File is empty" });
            }
            else if (file.Length > 10000000)
            {
                return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
            }

            string s = file.FileName;
            int checkLastIndex = s.LastIndexOf('.');
            string lhs = checkLastIndex < 0 ? s : s.Substring(0, checkLastIndex), rhs = checkLastIndex < 0 ? "" : s.Substring(checkLastIndex + 1);
            if (!allowedFileExtensionsXlsx.Any(a => a.Equals(rhs)))
            {
                return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
            }
            var filePath = Path.Combine(target, file.FileName);

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

                Rha insertData = new Rha();
                insertData.FileType = file.ContentType;
                insertData.FileSize = file.Length;
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
                    await file.CopyToAsync(stream);
                    await _rha.Insert(insertData);
                    await _db.SaveChangesAsync();
                }

                // insert subrha
                int rhaId = insertData.Id;
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = System.IO.File.Open(newFilePath, FileMode.Open, FileAccess.Read))
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

                        // check if documents is null
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                DataTable obj = result.Tables[0];
                                var sheetName = obj.TableName;
                                var objCount = obj.Rows.Count;
                                var colCount = obj.Columns.Count;

                                // sementara di-comment karena ada hidden di file excel
                                // handling error
                                //if (colCount != 12)
                                //    return BadRequest(new { status = false, message = "You're not using the correct template" });

                                // check RHA first
                                var checkRHA = await _rha.GetById(rhaId.ToString());
                                if (checkRHA == null)
                                {
                                    await _rha.Delete(rhaId.ToString());
                                    await _db.SaveChangesAsync();
                                    return BadRequest(new { status = false, message = "No Rha found" });
                                }
                                else
                                {
                                    var jatuhTempoRHA = checkRHA.StatusJt;
                                    float compareDate = 0;
                                    List<SubRha> dataResponse = new List<SubRha>();
                                    for (int i = 0; i < objCount; i++)
                                    {

                                        // compare date
                                        var dateNow = DateTime.Today.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        string tglOnSubRHA = obj.Rows[i][9].ToString();
                                        string mergedTglandJthTempo = tglOnSubRHA + " " + jatuhTempoRHA;
                                        DateTime d1 = DateTime.ParseExact(dateNow, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        DateTime d2 = DateTime.ParseExact(mergedTglandJthTempo, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        compareDate = DateTime.Compare(d1, d2);

                                        // generate Jatuh Tempo
                                        var rha = new SubRha();
                                        if (compareDate < 0)
                                        {
                                            rha.StatusJatuhTempo = "Belum Jatuh Tempo";
                                        }
                                        else
                                        {
                                            rha.StatusJatuhTempo = "Sudah Jatuh Tempo";
                                        }

                                        rha.UicLama = obj.Rows[i][0].ToString();
                                        rha.DivisiBaru = obj.Rows[i][1].ToString();
                                        rha.UicBaru = obj.Rows[i][2].ToString();
                                        rha.NamaAudit = obj.Rows[i][3].ToString();
                                        rha.Lokasi = obj.Rows[i][4].ToString();

                                        if (obj.Rows[i][5].ToString() != "")
                                            rha.Nomor = Convert.ToInt32(obj.Rows[i][5]);

                                        rha.Masalah = obj.Rows[i][6].ToString();
                                        rha.Pendapat = obj.Rows[i][7].ToString();
                                        rha.Status = obj.Rows[i][8].ToString();

                                        if (obj.Rows[i][9].ToString() != "")
                                            rha.JatuhTempo = Convert.ToInt32(obj.Rows[i][9]);

                                        if (obj.Rows[i][10].ToString() != "")
                                            rha.TahunTemuan = Convert.ToInt32(obj.Rows[i][10]);


                                        //rha.OpenClose = obj.Rows[i][11].ToString();
                                        rha.Assign = obj.Rows[i][11].ToString();
                                        rha.RhaId = rhaId;
                                        rha.CreatedAt = DateTime.Now;
                                        rha.UpdatedAt = DateTime.Now;
                                        rha.OpenClose = "Open"; // auto open
                                        dataResponse.Add(rha);
                                        _db.SubRhas.Add(rha);

                                    }
                                    await _db.SaveChangesAsync();

                                    // automapper
                                    var config = new MapperConfiguration(cfg =>
                                    cfg.CreateMap<SubRha, SubRhaDto>()
                                    );
                                    var mapper = new Mapper(config);
                                    List<SubRhaDto> resultData = mapper.Map<List<SubRha>, List<SubRhaDto>>(dataResponse);
                                }
                            }
                            catch (DbUpdateException dbEx)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(new { status = "Error", message = dbEx.Message, logtime = DateTime.Now });
                            }
                            catch (Exception ex)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(new { status = "Error", message = ex.Message, logtime = DateTime.Now });
                            }
                            return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_name = newfileName, file_size = file.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
                        }
                        else
                        {
                            await _rha.Delete(rhaId.ToString());
                            await _db.SaveChangesAsync();
                            return NotFound(new { status = "Fail", message = "Document is empty" });
                        }
                    }
                }
                //return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_name = newfileName, file_size = file.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
            }
            else
            {
                // TO DO 
                // using AutoMapper
                Rha insertData = new Rha();
                insertData.FileType = file.ContentType;
                insertData.FileSize = file.Length;
                insertData.FileName = file.FileName;
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
                    await file.CopyToAsync(stream);
                    await _rha.Insert(insertData);
                    await _db.SaveChangesAsync();
                }

                int rhaId = insertData.Id;
                // insert subrha
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

                        // check if the document is empty
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                DataTable obj = result.Tables[0];
                                var sheetName = obj.TableName;
                                var objCount = obj.Rows.Count;
                                var colCount = obj.Columns.Count;

                                // sementara di-comment karena ada hidden di file excel
                                // handling error
                                //if (colCount != 12)
                                //    return BadRequest(new { status = false, message = "You're not using the correct template" });

                                // check RHA first
                                var checkRHA = await _rha.GetById(rhaId.ToString());

                                var jatuhTempoRHA = checkRHA.StatusJt;
                                float compareDate = 0;
                                List<SubRha> dataResponse = new List<SubRha>();
                                for (int i = 0; i < objCount; i++)
                                {
                                    if (Convert.ToInt32(obj.Rows[i][9]) <= 0)
                                    {
                                        await _rha.Delete(rhaId.ToString());
                                        await _db.SaveChangesAsync();
                                        return BadRequest(new { status = false, message = "Invalid date time" });
                                    }
                                    else
                                    {
                                        // compare date
                                        var dateNow = DateTime.Today.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        string tglOnSubRHA = obj.Rows[i][9].ToString();
                                        string mergedTglandJthTempo = tglOnSubRHA + " " + jatuhTempoRHA;
                                        DateTime d1 = DateTime.ParseExact(dateNow, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        DateTime d2 = DateTime.ParseExact(mergedTglandJthTempo, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                        compareDate = DateTime.Compare(d1, d2);

                                        // generate Jatuh Tempo
                                        var rha = new SubRha();
                                        if (compareDate < 0)
                                        {
                                            rha.StatusJatuhTempo = "Belum Jatuh Tempo";
                                        }
                                        else
                                        {
                                            rha.StatusJatuhTempo = "Sudah Jatuh Tempo";
                                        }

                                        rha.UicLama = obj.Rows[i][0].ToString();
                                        rha.DivisiBaru = obj.Rows[i][1].ToString();
                                        rha.UicBaru = obj.Rows[i][2].ToString();
                                        rha.NamaAudit = obj.Rows[i][3].ToString();
                                        rha.Lokasi = obj.Rows[i][4].ToString();

                                        if (obj.Rows[i][5].ToString() != "")
                                            rha.Nomor = Convert.ToInt32(obj.Rows[i][5]);

                                        rha.Masalah = obj.Rows[i][6].ToString();
                                        rha.Pendapat = obj.Rows[i][7].ToString();
                                        rha.Status = obj.Rows[i][8].ToString();

                                        if (obj.Rows[i][9].ToString() != "")
                                            rha.JatuhTempo = Convert.ToInt32(obj.Rows[i][9]);

                                        if (obj.Rows[i][10].ToString() != "")
                                            rha.TahunTemuan = Convert.ToInt32(obj.Rows[i][10]);


                                        //rha.OpenClose = obj.Rows[i][11].ToString();
                                        rha.Assign = obj.Rows[i][11].ToString();
                                        rha.RhaId = rhaId;
                                        rha.CreatedAt = DateTime.Now;
                                        rha.UpdatedAt = DateTime.Now;
                                        rha.OpenClose = "Open"; // auto open
                                        dataResponse.Add(rha);
                                        _db.SubRhas.Add(rha);
                                    }
                                }
                                await _db.SaveChangesAsync();

                                // automapper
                                var config = new MapperConfiguration(cfg =>
                                cfg.CreateMap<SubRha, SubRhaDto>()
                                );
                                var mapper = new Mapper(config);
                                List<SubRhaDto> resultData = mapper.Map<List<SubRha>, List<SubRhaDto>>(dataResponse);
                            }
                            catch (DbUpdateException dbEx)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(new { status = "Error", message = dbEx.Message, logtime = DateTime.Now });
                            }
                            catch (Exception ex)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(new { status = "Error", message = ex.Message, logtime = DateTime.Now });
                            }
                            return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_size = file.Length, file_path = filePath, logtime = DateTime.Now });
                        }
                        else
                        {
                            await _rha.Delete(rhaId.ToString());
                            await _db.SaveChangesAsync();
                            return NotFound(new { status = "Fail", message = "Document is empty"});
                        }
                    }
                }
                //return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_size = file.Length, file_path = filePath, logtime = DateTime.Now });
            }
        }

        // POST & upload excel
        [Obsolete]
        [HttpPost(nameof(UploadOnlyRha))]
        public async Task<IActionResult> UploadOnlyRha([Required] IFormFile formFile, [FromForm] RhaDto rhafile)
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

    }
}
