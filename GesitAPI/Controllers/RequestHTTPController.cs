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
using System.Globalization;
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

        // 

        public class Response
        {
            public Response()
            {
                data = new List<ResponseData>();
            }
            public string division { get; set; }
            public List<ResponseData> data { get; set; }
        }


        public class ResponseData
        {
            public string name { get; set; }
            public string address { get; set; }
            public int married { get; set; }
            public string division { get; set; }
            public int? age { get; set; }
        }

        // 1

        public class MyItem
        {
            public string name { get; set; }
            public string address { get; set; }
            public int married { get; set; }
            public string division { get; set; }
            public int? age { get; set; }
        }

        public class Root
        {
            public List<MyItem> data { get; set; }
        }

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
                JObject obj = JObject.Parse(response.Content);
                var result = obj["data"];
                return Ok(new { message = "main API is api/rha", data = result});
            }
        }

        [HttpGet(nameof(test))]
        public IActionResult test()
        {
            var client = new RestClient("https://gist.githubusercontent.com/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("jerichosiahaya/ba36e2a9ba40d24c0f530eb2f57dcad2/raw/28b84a4cb332ef95e9bea28403002910069b4481/dummydata.json");
            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<Root>(response.Content);

            var groupByDivision = result.data.Where(x => x.division != null)
                .GroupBy(e => e.division, (d, r) => new Response()
                  {
                      division = d,
                      data =  r.Select(x => new ResponseData() { name = x.name, address = x.address }).ToList()

                })
                  .ToList();

            //List<object> vList = new List<object>();
            //foreach (var group in groupByDivision)
            //{
            //    vList.Add(group.ToList());
            //}

            //foreach (var item in result.data)
            //{
            //    var total = 0;
            //    total = item.GetType()
            //        .GetProperties()
            //        .Select(x => x.GetValue(item, null))
            //        .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));
            //    item.info.Add(new Info() { status = total });
            //}

            return Ok(groupByDivision);
        }
    }
}
