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
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE2MzEwMTEyNjcsImV4cCI6MTYzMTYxNjA2NiwiaWF0IjoxNjMxMDExMjY3fQ.1Lmq1dMOAcuU3qNqJ2cm-be-sRJAULu288kzGHpojac";
            var client = new RestClient("http://35.219.8.90:90/");
            var request = new RestRequest("api/rha");
            request.AddHeader("authorization", "Bearer " + token);
            var response = client.Execute(request);
            if (response == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new { statusCode = response.StatusCode, data = response.Content });
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
