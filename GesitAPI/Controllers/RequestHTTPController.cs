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

        public class MyItem
        {
            public decimal? status_completed { get; set; }
            public string name { get; set; }
            public string address { get; set; }
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
            var request = new RestRequest("jerichosiahaya/ba36e2a9ba40d24c0f530eb2f57dcad2/raw/bcc1b39504039facff6fbd7000e40ee57ed59b58/dummydata.json");
            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<Root>(response.Content);
            //var total = 0;
            foreach (var item in result.data)
            {
                var total = 0;
                total = item.GetType()
                    .GetProperties()
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));
                
                item.status_completed = (total-1)/3m;
            }
            return Ok(result);
        }

        //var result = obj["data"].Where(jt => (string)jt["name"] == "John Doe").ToList();
        //[HttpGet(nameof(Progo))]
        //public IActionResult Progo(string kategori, string divisi)
        //{
        //    var client = new RestClient("http://35.219.107.102/");
        //    client.UseNewtonsoftJson(DefaultSettings);
        //    // string kategori added to the URL scheme
        //    var request = new RestRequest("progodev/api/project?kategori="+kategori);
        //    request.AddHeader("progo-key", "progo123");
        //    var response = client.Execute(request);
        //    JObject obj = JObject.Parse(response.Content);
        //    if (!obj.ContainsKey("status"))
        //    {
        //        return NotFound(response.StatusCode);
        //    }
        //    else
        //    {
        //        //JObject obj = JObject.Parse(response.Content);
        //        // string divisi added as parameter of the where clause
        //        var pdm = obj["data"].Where(jt => (string)jt["AIPId"] == divisi).ToList();
        //        if (pdm.Count == 0)
        //            return NoContent();
        //        //return Ok(obj["data"][10]);
        //        return Ok(new { message = "tes", data = pdm });
        //    }
        //}

        //[HttpGet(nameof(ProgoDokumen))]
        //public IActionResult ProgoDokumen(string aipId)
        //{

        //    var client = new RestClient("http://35.219.107.102/");
        //    client.UseNewtonsoftJson(DefaultSettings);
        //    // string kategori added to the URL scheme
        //    var request = new RestRequest("progodev/api/dokumen?AIPId=" + aipId);
        //    request.AddHeader("progo-key", "progo123");
        //    var response = client.Execute(request);
        //    if (response.Content == null)
        //    {
        //        return NotFound(response.StatusCode);
        //    }
        //    else
        //    {
        //        JObject obj = JObject.Parse(response.Content);
        //        return Ok(obj);
        //    }
        //}

        //[HttpGet(nameof(ProgoDivisi))]
        //public IActionResult ProgoDivisi(string kategori)
        //{

        //    var client = new RestClient("http://35.219.107.102/");
        //    client.UseNewtonsoftJson(DefaultSettings);
        //    // string kategori added to the URL scheme
        //    var request = new RestRequest("progodev/api/project?kategori=" + kategori);
        //    request.AddHeader("progo-key", "progo123");
        //    var response = client.Execute(request);
        //    if (response.Content == null)
        //    {
        //        return NotFound(response.StatusCode);
        //    }
        //    else
        //    {
        //        JObject obj = JObject.Parse(response.Content);
        //        var listDivisi = obj["data"];
        //        List<string> div = new List<string>();
        //        for (int i = 0; i < listDivisi.Count(); i++)
        //        {
        //            var a = obj["data"][i]["Divisi"];
        //            div.Add(a.ToString());
        //        }
        //        return Ok(div.Distinct());
        //    }
        //}

        //[HttpGet(nameof(ProgoMerged))]
        //public IActionResult ProgoMerged(string kategori, string aipId)
        //{
        //    var client = new RestClient("http://35.219.107.102/");
        //    client.UseNewtonsoftJson(DefaultSettings);

        //    var request = new RestRequest("progodev/api/project?kategori=" + kategori);
        //    var request2 = new RestRequest("progodev/api/dokumen?AIPId=" + aipId);
        //    request.AddHeader("progo-key", "progo123");
        //    request2.AddHeader("progo-key", "progo123");

        //    var response = client.Execute(request);
        //    var response2 = client.Execute(request2);

        //    JObject obj = JObject.Parse(response.Content);
        //    JObject obj2 = JObject.Parse(response2.Content);

        //    if (!obj.ContainsKey("status"))
        //    {
        //        return NotFound(response.StatusCode);
        //    }
        //    else
        //    {
        //        var noDok = obj["data"].Where(jt => (string)jt["AIPId"] == aipId).ToList();
        //        var dok = obj2["data"];

        //        // perhitungan status
        //        var a = obj["data"].Where(jt => (string)jt["AIPId"] == aipId).Select(s => new 
        //        {
        //            b1 = (string)s["ProjectCategory"], 
        //            b2 = (string)s["JenisPengembangan"],
        //            b3 = (string)s["Pengembang"],
        //            b4 = (string)s["EksImplementasi"],
        //            b5 = (string)s["NamaProject"],
        //            b6 = (string)s["NamaAIP"],
        //            b7 = (string)s["StrategicImportance"],
        //            b8 = (string)s["PPJTIPihakTerkait"]
        //        }).ToList();

        //        List<string> vs = new List<string>();
        //        a.ForEach(o =>
        //        {
        //            string k1 = o.b1.ToString();
        //            string k2 = o.b2.ToString();
        //            string k3 = o.b3.ToString();
        //            string k4 = o.b4.ToString();
        //            string k5 = o.b5.ToString();
        //            string k6 = o.b6.ToString();
        //            string k7 = o.b7.ToString();
        //            string k8 = o.b8.ToString();
        //            vs.Add(k1);
        //            vs.Add(k2);
        //            vs.Add(k3);
        //            vs.Add(k4);
        //            vs.Add(k5);
        //            vs.Add(k6);
        //            vs.Add(k7);
        //            vs.Add(k8);
        //        });
        //        vs = vs.Where(o => o != "" && o != null).ToList();

        //        // pembagian
        //        int countItems = vs.Count();
        //        decimal statusResult = countItems / 8m;

        //        // new json info
        //        var info = new
        //        {
        //            percentage_completed = statusResult,
        //            completed = countItems,
        //            uncompleted = 8 - countItems
        //        };

        //        return Ok(new { info = info, data = noDok, dokumen = dok });
        //    }
        //}
    }
}
