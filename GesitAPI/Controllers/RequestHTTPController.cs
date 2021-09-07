using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using RestSharp.Serializers.NewtonsoftJson;
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
            // json serializer setting
            JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE2MzEwMTEyNjcsImV4cCI6MTYzMTYxNjA2NiwiaWF0IjoxNjMxMDExMjY3fQ.1Lmq1dMOAcuU3qNqJ2cm-be-sRJAULu288kzGHpojac";
            var client = new RestClient("http://35.219.8.90:90/");
            client.UseNewtonsoftJson(DefaultSettings);
            var request = new RestRequest("api/rha");
            request.AddHeader("authorization", "Bearer " + token);
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                //string jsonString = JsonSerializer.Serialize(response.Content);
                return Ok(response.Content);
            }
        }
    }
}
