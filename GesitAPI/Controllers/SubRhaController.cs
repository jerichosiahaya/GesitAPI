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
    public class SubRhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private GesitDbContext _db;
        private ISubRha _subRha;
        public SubRhaController(GesitDbContext db, ISubRha subRha, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _subRha = subRha;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/<SubRhaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _subRha.GetAll();
            var files = results.ToList();
            return Ok(new { count = files.Count(), data = files });
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
                            if (colCount != 11)
                                return BadRequest(new { status = false, message = "You're not using the correct template" });

                            for (int i = 0; i < objCount; i++)
                            {
                                var rha = new SubRha(); // DI from Models
                                rha.DivisiBaru = obj.Rows[i][0].ToString();
                                rha.UicBaru = obj.Rows[i][1].ToString();
                                rha.NamaAudit = obj.Rows[i][2].ToString();
                                rha.Lokasi = obj.Rows[i][3].ToString();
                                rha.Nomor = Convert.ToInt32(obj.Rows[i][4]);
                                rha.Masalah = obj.Rows[i][5].ToString();
                                rha.Pendapat = obj.Rows[i][6].ToString();
                                rha.Status = obj.Rows[i][7].ToString();
                                rha.JatuhTempo = Convert.ToDateTime(obj.Rows[i][8]);
                                rha.TahunTemuan = Convert.ToInt32(obj.Rows[i][9]);
                                rha.Assign = obj.Rows[i][10].ToString();
                                rha.RhaId = id;
                                _db.SubRhas.Add(rha);
                            }
                            await _db.SaveChangesAsync();
                            return Ok(new { status = true, count = objCount, column_count = colCount, sheet_name = sheetName, data = obj });
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
    }
}
