using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestHTTPController : ControllerBase
    {
        //GET: api/<RequestHTTPController>
        [HttpGet]
        public IActionResult Get()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com/");
            var request = new RestRequest("todos/1");
            var response = client.Execute(request);
            if (response == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(response.Content);
            }
        }

        // GET api/<RequestHTTPController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RequestHTTPController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RequestHTTPController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        
    }
}
