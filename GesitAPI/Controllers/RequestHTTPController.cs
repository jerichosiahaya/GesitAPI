using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
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
        public IActionResult Get(IRestResponse Data)
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com/todos/1");
            //client.Authenticator = new HttpBasicAuthenticator("username", "password");
            var request = new RestRequest("statuses/home_timeline.json", DataFormat.Json);
            var response = client.Get(request);
            if (response == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Data = response);
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
