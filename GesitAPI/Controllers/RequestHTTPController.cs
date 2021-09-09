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

// Author: Jericho Cristofel Siahaya
// Created: 2021-09-09

namespace GesitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestHTTPController : ControllerBase
    {
        // json serialize settings
        // use this settings to change the format of the json output
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
            // this authentication function method located at Helpers folder
            // use this authentication if you have bearer token to be added on the header
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
        public IActionResult Progo(string kategori, string divisi)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);
            // string kategori added to the URL scheme
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
                // string divisi added as parameter of the where clause
                var pdm = obj["data"].Where(jt => (string)jt["Divisi"] == divisi).ToList();
                if (pdm.Count == 0)
                    return NoContent();
                //return Ok(obj["data"][10]);
                return Ok(pdm);
            }
        }


    }
}
