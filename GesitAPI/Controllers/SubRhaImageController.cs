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
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubRhaImageController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private GesitDbContext _db;
        private ISubRha _subRha;
        private IRha _rha;
        private readonly ISubRhaImage _subRhaImage;
        public SubRhaImageController(GesitDbContext db, ISubRha subRha, IRha rha, ISubRhaImage subRhaImage, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _subRha = subRha;
            _rha = rha;
            _subRhaImage = subRhaImage;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "jpeg", "png", "JPG", "JPEG", "PNG" };

        // upload image
        [HttpPost(nameof(UploadImage))]
        public async Task<IActionResult> UploadImage([Required] IFormFile image, [FromForm] int subRhaId)
        {
            SubRhaimage subRhaImage = new SubRhaimage();
            var subDirectory = "UploadedFiles";
            var subDirectory2 = "SubRhaImages";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory, subDirectory2);
            string webPath = "http://35.219.8.90:90/";
            var viewLInk = webPath + "api/SubRhaImage/GetById/";
            Directory.CreateDirectory(target);
            try
            {
                if (image.Length <= 0)
                {
                    return BadRequest(new { status = "Error", message = "File is empty" });
                }
                else if (image.Length > 15000000)
                {
                    return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
                }

                string s = image.FileName;
                int i = s.LastIndexOf('.');
                string lhs = i < 0 ? s : s.Substring(0, i), rhs = i < 0 ? "" : s.Substring(i + 1);
                if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
                {
                    return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
                }
                var filePath = Path.Combine(target, image.FileName);

                subRhaImage.FileType = image.ContentType;
                subRhaImage.FileSize = image.Length;
                subRhaImage.SubRhaId = subRhaId;

                subRhaImage.CreatedAt = DateTime.Now;
                subRhaImage.UpdatedAt = DateTime.Now;

                if (System.IO.File.Exists(filePath))
                {
                    // query for duplicate names to generate counter
                    var duplicateNames = await _subRhaImage.CountExistingFileNameSubRhaImage(lhs); // using DI from data access layer
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
                    subRhaImage.FileName = newfileName;
                    subRhaImage.FilePath = newFilePath;

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                        await _subRhaImage.Insert(subRhaImage); // todo
                    }

                    // new dto to return the response
                    SubRhaImageDto responseData = new SubRhaImageDto();
                    responseData.Id = subRhaImage.Id;
                    responseData.FileName = image.FileName;
                    responseData.FileSize = image.Length;
                    responseData.FileType = image.ContentType;
                    responseData.CreatedAt = DateTime.Now;
                    responseData.ViewImage = viewLInk + subRhaImage.Id;

                    return Ok(new { status = "Success", message = "File successfully uploaded", data = responseData, duplicated_filenames = arrDuplicatedNames.ToList() });
                }
                else
                {
                    subRhaImage.FileName = image.FileName;
                    subRhaImage.FilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                        await _subRhaImage.Insert(subRhaImage);
                    }

                    // new dto to return the response
                    SubRhaImageDto responseData = new SubRhaImageDto();
                    responseData.Id = subRhaImage.Id;
                    responseData.FileName = image.FileName;
                    responseData.FileSize = image.Length;
                    responseData.FileType = image.ContentType;
                    responseData.CreatedAt = DateTime.Now;
                    responseData.ViewImage = viewLInk + subRhaImage.Id;

                    return Ok(new { status = "Success", message = "File successfully uploaded", data = responseData, logtime = DateTime.Now });
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

        [HttpGet(nameof(ViewImage))]
        public IActionResult ViewImage([Required] int subRhaId)
        {
            var result = _db.SubRhaimages.Where(o => o.SubRhaId == subRhaId).ToList();
            List<SubRhaImageDto> resultData = new List<SubRhaImageDto>();
            
            foreach (var o in result)
            {
                string base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(o.FilePath));
                resultData.Add(new SubRhaImageDto
                {
                    Id = o.Id,
                    FileName = o.FileName,
                    FileSize = o.FileSize,
                    FileType = o.FileType,
                    ViewImage = "data:" + o.FileType + ";base64, " + base64,
                    CreatedAt = o.CreatedAt
                });
            }

            return Ok(resultData);
        }

        [AllowAnonymous]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _subRhaImage.GetById(id.ToString());
            var imageType = result.FileType;
            var imagePath = result.FilePath;
            Byte[] contentImage = System.IO.File.ReadAllBytes(imagePath);
            return File(contentImage, imageType);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _subRhaImage.GetAll();
            List<SubRhaImageDto> resultsData = new List<SubRhaImageDto>();
            string webPath = "http://35.219.8.90:90/";
            var viewLInk = webPath + "api/SubRhaImage/GetById/";
            foreach (var o in results)
            {
                resultsData.Add(new SubRhaImageDto
                {
                    Id = o.Id,
                    FileName = o.FileName,
                    FileType = o.FileType,
                    FileSize = o.FileSize,
                    CreatedAt = o.CreatedAt,
                    ViewImage = viewLInk + o.Id
                });
            }
            return Ok(resultsData);
        }

    }
}
