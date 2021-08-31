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
    public class RhaEvidenceController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IRhaevidence _rha;
        public RhaEvidenceController(IRhaevidence rhaEvidence, IWebHostEnvironment hostingEnvironment)
        {
            _rha = rhaEvidence;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/<RhaEvidenceController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RhaEvidenceController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        // POST api/<RhaEvidenceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RhaEvidenceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RhaEvidenceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
