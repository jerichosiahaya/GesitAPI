using GesitAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        // json serialize setting
        JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        [HttpGet(nameof(GesitRha))]
        public IActionResult GesitRha(string npp, string password)
        {
            // DI from Helpers
            Authentication auth = new Authentication();
            var token = auth.GesitAuth(npp, password);

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

        [HttpGet(nameof(Progo))]
        public IActionResult Progo(string kategori)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);
            var request = new RestRequest("progodev/api/project?kategori="+kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
                var pdm = obj["data"].Where(jt => (string)jt["Divisi"] == "PDM").ToList();
                //return Ok(obj["data"][10]);
                return Ok(pdm[0]);
            }
        }


    }
}
