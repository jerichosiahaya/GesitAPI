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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubRhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private GesitDbContext _db;
        private ISubRha _subRha;
        private IRha _rha;
        private ISubRhaImage _subRhaImage;
        public SubRhaController(GesitDbContext db, ISubRha subRha, IRha rha, ISubRhaImage subRhaImage, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _subRha = subRha;
            _subRhaImage = subRhaImage;
            _rha = rha;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "jpeg", "png", "xlsx", "JPG", "JPEG", "PNG", "XLSX" };

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _subRha.GetAll();
            return Ok(new { count = results.Count(), data = results });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _subRha.GetById(id);
            return Ok(new { data = results });
        }

        [HttpGet("GetByRhaId/{rhaId}")]
        public async Task<IActionResult> GetByRhaID(string rhaId)
        {
            var results = await _subRha.GetByRhaID(rhaId);
            var files = results.ToList();
            if (files.Count == 0)
                return Ok(new { status = "null", message = "Empty data" });

            return Ok(new { data = files });
        }

        // GET assign with RHA (utama)
        [HttpGet("GetByAssign/{assign}")]
        public async Task<IActionResult> GetByAssign(string assign)
        {
            var results = await _subRha.GetByAssign(assign);
            return Ok(new { data = results });
        }

        [HttpGet("GetByRhaIDandAssign/{rhaId}/{assign}")]
        public async Task<IActionResult> GetByRhaIDandAssign(string rhaId, string assign)
        {
            var results = await _subRha.GetByRhaIDandAssign(rhaId, assign);
            List<SubRhaViewImageDto> resultData = new List<SubRhaViewImageDto>();
            string webPath = "http://35.219.8.90:90/";
            var viewLInk = webPath + "api/SubRhaImage/GetById/";
            foreach (var o in results)
            {
                SubRhaViewImageDto tempData = new SubRhaViewImageDto();
                tempData.Id = o.Id;
                tempData.RhaId = o.RhaId;
                tempData.DivisiBaru = o.DivisiBaru;
                tempData.UicBaru = o.UicBaru;
                tempData.NamaAudit = o.NamaAudit;
                tempData.Lokasi = o.Lokasi;
                tempData.Nomor = o.Nomor;
                tempData.Masalah = o.Masalah;
                tempData.Pendapat = o.Pendapat;
                tempData.Status = o.Status;
                tempData.JatuhTempo = o.JatuhTempo;
                tempData.TahunTemuan = o.TahunTemuan;
                tempData.Assign = o.Assign;
                tempData.UicLama = o.UicLama;
                tempData.OpenClose = o.OpenClose;
                tempData.UsulClose = o.UsulClose;
                tempData.StatusJatuhTempo = o.StatusJatuhTempo;
                //tempData.SubRhaevidences = o.SubRhaevidences;
                //tempData.TindakLanjuts = o.TindakLanjuts;

                foreach (var i in o.SubRhaimages)
                {
                    SubRhaImageDto vData = new SubRhaImageDto();
                    vData.Id = i.Id;
                    vData.FileName = i.FileName;
                    vData.FileSize = i.FileSize;
                    vData.FileType = i.FileType;
                    vData.CreatedAt = i.CreatedAt;
                    vData.ViewImage = viewLInk + i.Id;
                    tempData.SubRhaImages.Add(vData);
                }

                foreach (var v in o.SubRhaevidences)
                {
                    SubRhaEvidenceDto vData = new SubRhaEvidenceDto();
                    vData.Id = v.Id;
                    vData.SubRhaId = v.SubRhaId;
                    vData.FileName = v.FileName;
                    vData.Notes = v.Notes;
                    vData.CreatedAt = v.CreatedAt;
                    vData.UpdatedAt = v.UpdatedAt;
                    tempData.SubRhaevidences.Add(vData);
                }

                foreach (var s in o.TindakLanjuts)
                {
                    TindakLanjutDto vData = new TindakLanjutDto();
                    vData.Id = s.Id;
                    vData.SubRhaId = s.SubRhaId;
                    vData.Notes = s.Notes;

                    // add to new list object first
                    // then loop through the TindakLanjutEvidence objects
                    List<TindakLanjutEvidenceDto> vDataList = new List<TindakLanjutEvidenceDto>();
                    foreach (var p in s.TindakLanjutEvidences)
                    {
                        vDataList.Add(new TindakLanjutEvidenceDto
                        {
                            Id = p.Id,
                            FileName = p.FileName,
                            FilePath = p.FilePath,
                            FileType = p.FileType,
                            CreatedAt = p.CreatedAt
                        });
                    }
                    vData.TindakLanjutEvidences = vDataList;

                    tempData.TindakLanjuts.Add(vData);
                }

                // add to dto list
                resultData.Add(tempData);
            }
            return Ok(resultData);
        }

        // insert sub rha from excel file
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file, [FromForm] int rhaId)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "SubRha";
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
                                return BadRequest();

                            var jatuhTempoRHA = checkRHA.StatusJt;
                            float compareDate = 0;
                            List<SubRha> dataResponse = new List<SubRha>();
                            for (int i = 0; i < objCount; i++)
                            {
                                if (Convert.ToInt32(obj.Rows[i][9]) <= 0)
                                    return BadRequest(new { status = false, message = "Invalid date time" });

                                // compare date
                                var dateNow = DateTime.Today.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                string tglOnSubRHA = obj.Rows[i][9].ToString();
                                string mergedTglandJthTempo = tglOnSubRHA + " " + jatuhTempoRHA;
                                DateTime d1 = DateTime.ParseExact(dateNow, "dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                DateTime d2 = DateTime.ParseExact(mergedTglandJthTempo, "dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
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

                                // default value to Open
                                //if (obj.Rows[i][11].ToString() == "" || obj.Rows[i][11].ToString() == null)
                                //{
                                //    rha.OpenClose = "Open";
                                //} else
                                //{
                                //    rha.OpenClose = obj.Rows[i][11].ToString();
                                //}

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

                            return Ok(new { status = true, count = objCount, column_count = colCount, sheet_name = sheetName, data = resultData });
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
        }

        // insert sub rha from form
        [HttpPost(nameof(SingleUpload))]
        public async Task<IActionResult> SingleUpload([Required][FromForm] SubRhaInsert subRhaInsert)
        {
            int rhaId = subRhaInsert.RhaId;
            // check rha id first
            var checkRHA = await _rha.GetById(rhaId.ToString());
            if (checkRHA != null)
            {
                var jatuhTempoRHA = checkRHA.StatusJt;
                
                // compare date
                var dateNow = DateTime.Today.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                string tglOnSubRHA = subRhaInsert.JatuhTempo.ToString();
                string mergedTglandJthTempo = tglOnSubRHA + " " + jatuhTempoRHA;
                DateTime d1 = DateTime.ParseExact(dateNow, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                DateTime d2 = DateTime.ParseExact(mergedTglandJthTempo, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                float compareDate = DateTime.Compare(d1, d2);

                
                SubRha subRha = new SubRha();

                // generate Jatuh Tempo
                if (compareDate < 0)
                {
                    subRha.StatusJatuhTempo = "Belum Jatuh Tempo";
                }
                else
                {
                    subRha.StatusJatuhTempo = "Sudah Jatuh Tempo";
                }

                subRha.JatuhTempo = subRhaInsert.JatuhTempo;
                subRha.Lokasi = subRhaInsert.Lokasi;
                subRha.Masalah = subRhaInsert.Masalah;
                subRha.NamaAudit = subRhaInsert.NamaAudit;
                subRha.Nomor = subRhaInsert.Nomor;
                subRha.OpenClose = "Open";
                subRha.Pendapat = subRhaInsert.Pendapat;
                subRha.RhaId = rhaId;
                subRha.DivisiBaru = subRhaInsert.DivisiBaru;
                subRha.UicLama = subRhaInsert.UicLama;
                subRha.UicBaru = subRhaInsert.UicBaru;
                subRha.Status = subRhaInsert.Status;
                subRha.TahunTemuan = subRhaInsert.TahunTemuan;
                subRha.Assign = subRhaInsert.Assign;
                subRha.CreatedAt = DateTime.Now;
                subRha.UpdatedAt = DateTime.Now;
                await _subRha.Insert(subRha);

                return Ok(new { status = "Success", data = subRhaInsert});
            } 
            else
            {
                return BadRequest(new { status = "Error", message = $"There is no RHA with id: {rhaId}" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SubRhaDto subrha)
        {
            try
            {
                SubRha updateData = new SubRha();
                updateData.DivisiBaru = subrha.DivisiBaru;
                updateData.UicLama = subrha.UicLama;
                updateData.UicBaru = subrha.UicBaru;
                updateData.NamaAudit = subrha.NamaAudit;
                updateData.Lokasi = subrha.Lokasi;
                updateData.Nomor = subrha.Nomor;
                updateData.Masalah = subrha.Masalah;
                updateData.Pendapat = subrha.Pendapat;
                updateData.Status = subrha.Status;
                updateData.JatuhTempo = subrha.JatuhTempo;
                updateData.TahunTemuan = subrha.TahunTemuan;
                updateData.Assign = subrha.Assign;
                updateData.UsulClose = subrha.UsulClose;
                updateData.OpenClose = subrha.OpenClose;
                await _subRha.Update(subrha.Id.ToString(), updateData);
                return Ok($"Data {subrha.Id} berhasil diupdate!");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPut("UpdateUsulClose/{id}")]
        public async Task<IActionResult> UpdateUsulClose(string id, string usulClose)
        {
            try
            {
                var result = await _subRha.GetById(id);
                if (result != null)
                {
                    result.UsulClose = usulClose;
                    result.UpdatedAt = DateTime.Now;
                    await _db.SaveChangesAsync();
                    return Ok($"Data {id} successfully updated!");
                }
                else
                {
                    throw new Exception($"Data {id} not found");
                }
            }
            catch (DbUpdateException DbEx)
            {

                throw new Exception(DbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // delete sub rha
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var results = await _subRha.GetById(id);
            if (results == null)
            {
                return BadRequest(new { status = "Error", message = "There is no such a file" });
            }
            else
            {
                await _subRha.Delete(id.ToString());
                return Ok(new { status = true, message = $"Successfully delete the RHA file with id: {id}" });
            }
        }

        // HIDDEN CONTROLLER BELOW
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut(nameof(UpdateStatusJatuhTempo))]
        public async Task<IActionResult> UpdateStatusJatuhTempo()
        {
            try
            {
                var subRhaGetAll = await _subRha.GetAllTracking();
                var dateNow = DateTime.Today.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                int compareDate = 0;
                foreach (var o in subRhaGetAll)
                {
                    var rhaGetById = await _rha.GetById(o.RhaId.ToString());
                    var jatuhTempoRha = rhaGetById.StatusJt;
                    var jatuhTempoSubRha = o.JatuhTempo.ToString();
                    var jatuhTempoMerged = jatuhTempoSubRha + " " + jatuhTempoRha;
                    DateTime d1 = DateTime.ParseExact(dateNow, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                    DateTime d2 = DateTime.ParseExact(jatuhTempoMerged, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                    compareDate = DateTime.Compare(d1, d2);
                    if (compareDate < 0)
                    {
                        o.StatusJatuhTempo = "Belum Jatuh Tempo";
                    }
                    else
                    {
                        o.StatusJatuhTempo = "Sudah Jatuh Tempo";
                    }
                }
                await _db.SaveChangesAsync();
                return Ok();
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut(nameof(UpdateStatusJatuhTempoBySubRha))]
        public async Task<IActionResult> UpdateStatusJatuhTempoBySubRha(int SubRhaId)
        {
            var subRha = await _subRha.GetById(SubRhaId.ToString());
            var rhaGetById = await _rha.GetById(subRha.RhaId.ToString());
            var dateNow = DateTime.Today.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            int compareDate = 0;

            var jatuhTempoRha = rhaGetById.StatusJt;
            var jatuhTempoSubRha = subRha.JatuhTempo.ToString();
            var jatuhTempoMerged = jatuhTempoSubRha + " " + jatuhTempoRha;
            DateTime d1 = DateTime.ParseExact(dateNow, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            DateTime d2 = DateTime.ParseExact(jatuhTempoMerged, "d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            compareDate = DateTime.Compare(d1, d2);

            if (compareDate < 0)
            {
                subRha.StatusJatuhTempo = "Belum Jatuh Tempo";
            }
            else
            {
                subRha.StatusJatuhTempo = "Sudah Jatuh Tempo";
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost(nameof(UploadWithRha))]
        public async Task<IActionResult> UploadWithRha([Required] IFormFile file, [FromForm] RhaDto rhafile)
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
            if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
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

                        if (result != null)
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
                                throw new Exception(dbEx.Message);
                            }
                            catch (Exception ex)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(ex.Message);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
                return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_name = newfileName, file_size = file.Length, file_path = newFilePath, logtime = DateTime.Now, duplicated_filenames = arrDuplicatedNames.ToList() });
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

                        if (result != null)
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

                                // to do hapus if statement

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
                                throw new Exception(dbEx.Message);
                            }
                            catch (Exception ex)
                            {
                                await _rha.Delete(rhaId.ToString());
                                await _db.SaveChangesAsync();
                                return BadRequest(ex.Message);
                            }
                        }
                        else
                        {
                            await _rha.Delete(rhaId.ToString());
                            await _db.SaveChangesAsync();
                            return NotFound();
                        }
                    }
                }
                return Ok(new { status = "Success", message = "File successfully uploaded", id = insertData.Id, file_size = file.Length, file_path = filePath, logtime = DateTime.Now });
            }
        }

    }
}
