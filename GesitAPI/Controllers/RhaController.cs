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

        // POST api/<RhaController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile file)
        {
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "Rha";
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
                        try { 
                            var obj = result.Tables[0];
                            var sheetName = obj.TableName;
                            var objCount = obj.Rows.Count;
                        
                            for (int i=0; i<objCount; i++)
                            {
                                var rha = new Rha(); // DI from Models
                                rha.DivisiBaru = obj.Rows[i][0].ToString();
                                rha.UicBaru = obj.Rows[i][1].ToString();
                                rha.NamaAudit = obj.Rows[i][2].ToString();
                                rha.Lokasi = obj.Rows[i][3].ToString();
                                rha.Nomor = Convert.ToInt32(obj.Rows[i][4]);
                                rha.Masalah = obj.Rows[i][5].ToString();
                                rha.Pendapat = obj.Rows[i][6].ToString();
                                rha.Status = obj.Rows[i][7].ToString();;
                                rha.TahunTemuan = Convert.ToInt32(obj.Rows[i][9]);
                                rha.Assign = obj.Rows[i][10].ToString();
                                _db.Rhas.Add(rha);
                            }
                            await _db.SaveChangesAsync();
                            return Ok(new { status = true, count = objCount, sheet_name = sheetName, data = obj });
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

        // PUT api/<RhaController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<RhaController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
