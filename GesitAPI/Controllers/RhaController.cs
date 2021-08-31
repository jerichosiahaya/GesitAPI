using GesitAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RhaController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IRha _rha;
        public RhaController(IRha rha, IWebHostEnvironment hostingEnvironment)
        {
            _rha = rha;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/<RhaController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _rha.GetAll();
            var files = results.ToList();
            return Ok(new { rha_count = files.Count(), data = files });
        }

        // GET api/<RhaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var results = await _rha.GetById(id);
            return Ok(new { data = results });
        }

        // POST api/<RhaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
