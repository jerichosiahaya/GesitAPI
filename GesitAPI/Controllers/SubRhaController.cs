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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubRhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private GesitDbContext _db;
        private ISubRha _subRha;
        private IRha _rha;
        public SubRhaController(GesitDbContext db, ISubRha subRha, IRha rha, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _subRha = subRha;
            _rha = rha;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/<SubRhaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _subRha.GetAll();

            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<SubRha, SubRhaDto>();
            //});
            //IMapper iMapper = config.CreateMapper();
            //List<SubRhaDto> listSubRhaDto = iMapper.Map<List<SubRha>, List<SubRhaDto>>(results);

            return Ok(new { count = results.Count(), data = results });
        }

        // GET api/<SubRhaController>/5
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

        // GET sub rha by rha id and assign
        [HttpGet("GetByRhaIDandAssign/{rhaId}/{assign}")]
        public async Task<IActionResult> GetByRhaIDandAssign(string rhaId, string assign)
        {
            var results = await _subRha.GetByRhaIDandAssign(rhaId, assign);
            return Ok(new { data = results });
        }

        // POST Upload Excel
        // TO DO jatuh_tempo, fix duplicated file names
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file, [FromForm] int id)
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
                            var obj = result.Tables[0];
                            var sheetName = obj.TableName;
                            var objCount = obj.Rows.Count;
                            var colCount = obj.Columns.Count;

                            // handling error
                            if (colCount != 13)
                                return BadRequest(new { status = false, message = "You're not using the correct template" });

                            // check RHA first
                            var checkRHA = await _rha.GetById(id.ToString());
                            if (checkRHA == null)
                                return BadRequest();

                            var jatuhTempoRHA = checkRHA.StatusJt;
                            string tes = null;
                            float compareDate = 0;
                            for (int i = 0; i < objCount; i++)
                            {
                                var dateNow = DateTime.Today.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                string tglOnSubRHA = obj.Rows[i][9].ToString();
                                string mergedTglandJthTempo = tglOnSubRHA + " " +jatuhTempoRHA;
                                tes = mergedTglandJthTempo;

                                DateTime d1 = DateTime.ParseExact(dateNow, "dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                DateTime d2 = DateTime.ParseExact(mergedTglandJthTempo, "dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                                compareDate = DateTime.Compare(d1,d2);

                                var rha = new SubRha(); // DI from Models

                                if (compareDate < 0) {
                                    rha.StatusJatuhTempo = "Belum Jatuh Tempo";
                                } else
                                {
                                    rha.StatusJatuhTempo = "Sudah Jatuh Tempo";
                                }
                                   
                                rha.UicLama = obj.Rows[i][0].ToString();
                                rha.DivisiBaru = obj.Rows[i][1].ToString();
                                rha.UicBaru = obj.Rows[i][2].ToString();
                                rha.NamaAudit = obj.Rows[i][3].ToString();
                                rha.Lokasi = obj.Rows[i][4].ToString();
                                rha.Nomor = Convert.ToInt32(obj.Rows[i][5]);
                                rha.Masalah = obj.Rows[i][6].ToString();
                                rha.Pendapat = obj.Rows[i][7].ToString();
                                rha.Status = obj.Rows[i][8].ToString();
                                rha.JatuhTempo = Convert.ToInt32(obj.Rows[i][9]);
                                rha.TahunTemuan = Convert.ToInt32(obj.Rows[i][10]);
                                rha.OpenClose = obj.Rows[i][11].ToString();
                                rha.Assign = obj.Rows[i][12].ToString();
                                rha.RhaId = id;
                                _db.SubRhas.Add(rha);
                            }
                            await _db.SaveChangesAsync();
                            //System.IO.File.Delete(filePath);
                            return Ok(new { status = true, dt_now = DateTime.Today.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), dt_docs = tes,
                                date_range = compareDate, count = objCount, column_count = colCount, sheet_name = sheetName, data = obj });
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

        [HttpPut("UpdateUsulClose/{id}")]
        public async Task<IActionResult> UpdateUsulClose(string id, string usulClose)
        {
            try
            {
                var result = await _subRha.GetById(id);
                if (result != null)
                {
                    result.UsulClose = usulClose;
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
    }
}
