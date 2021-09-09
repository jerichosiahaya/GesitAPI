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
    public class ProgoController : ControllerBase
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

        [HttpGet(nameof(GetByKategoriandDivisi))]
        public IActionResult GetByKategoriandDivisi(string kategori, string divisi)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);
            // string kategori added to the URL scheme
            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            JObject obj = JObject.Parse(response.Content);
            if (!obj.ContainsKey("status"))
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                //JObject obj = JObject.Parse(response.Content);
                // string divisi added as parameter of the where clause
                var pdm = obj["data"].Where(jt => (string)jt["Divisi"] == divisi).ToList();
                if (pdm.Count == 0)
                    return NoContent();
                //return Ok(obj["data"][10]);
                return Ok(new { data = pdm });
            }
        }

        [HttpGet(nameof(GetDokumenByAIPid))]
        public IActionResult GetDokumenByAIPid(string aipId)
        {

            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);
            // string kategori added to the URL scheme
            var request = new RestRequest("progodev/api/dokumen?AIPId=" + aipId);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
                return Ok(obj);
            }
        }

        [HttpGet(nameof(GetDivisiByKategori))]
        public IActionResult GetDivisiByKategori(string kategori)
        {

            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);
            // string kategori added to the URL scheme
            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                JObject obj = JObject.Parse(response.Content);
                var listDivisi = obj["data"];
                List<string> div = new List<string>();
                for (int i = 0; i < listDivisi.Count(); i++)
                {
                    var a = obj["data"][i]["Divisi"];
                    div.Add(a.ToString());
                }
                return Ok(div.Distinct());
            }
        }

        [HttpGet(nameof(GetDetailsandDokumen))]
        public IActionResult GetDetailsandDokumen(string kategori, string aipId)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);

            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            var request2 = new RestRequest("progodev/api/dokumen?AIPId=" + aipId);
            request.AddHeader("progo-key", "progo123");
            request2.AddHeader("progo-key", "progo123");

            var response = client.Execute(request);
            var response2 = client.Execute(request2);

            JObject obj = JObject.Parse(response.Content);
            JObject obj2 = JObject.Parse(response2.Content);

            if (!obj.ContainsKey("status"))
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                var noDok = obj["data"].Where(jt => (string)jt["AIPId"] == aipId).ToList();
                var dok = obj2["data"];

                // perhitungan status
                var a = obj["data"].Where(jt => (string)jt["AIPId"] == aipId).Select(s => new
                {
                    b1 = (string)s["ProjectCategory"],
                    b2 = (string)s["JenisPengembangan"],
                    b3 = (string)s["Pengembang"],
                    b4 = (string)s["EksImplementasi"],
                    b5 = (string)s["NamaProject"],
                    b6 = (string)s["NamaAIP"],
                    b7 = (string)s["StrategicImportance"],
                    b8 = (string)s["PPJTIPihakTerkait"]
                }).ToList();

                List<string> vs = new List<string>();
                a.ForEach(o =>
                {
                    string k1 = o.b1.ToString();
                    string k2 = o.b2.ToString();
                    string k3 = o.b3.ToString();
                    string k4 = o.b4.ToString();
                    string k5 = o.b5.ToString();
                    string k6 = o.b6.ToString();
                    string k7 = o.b7.ToString();
                    string k8 = o.b8.ToString();
                    vs.Add(k1);
                    vs.Add(k2);
                    vs.Add(k3);
                    vs.Add(k4);
                    vs.Add(k5);
                    vs.Add(k6);
                    vs.Add(k7);
                    vs.Add(k8);
                });
                vs = vs.Where(o => o != "" && o != null).ToList();

                // pembagian
                int countItems = vs.Count();
                decimal statusResult = countItems / 8m;

                // new json info
                var info = new
                {
                    percentage_completed = statusResult,
                    completed = countItems,
                    uncompleted = 8 - countItems
                };

                return Ok(new { info = info, data = noDok, dokumen = dok });
            }
        }

        [HttpGet]
        public IActionResult Get(string aipId)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson(DefaultSettings);

            var request = new RestRequest("progodev/api/project?kategori=All");
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            JObject obj = JObject.Parse(response.Content);
            

            if (!obj.ContainsKey("status"))
            {
                return NotFound(response.StatusCode);
            }
            else
            {
                var noDok = obj["data"].ToList();

                var request2 = new RestRequest("progodev/api/dokumen?AIPId=" + aipId);
                request2.AddHeader("progo-key", "progo123");
                var response2 = client.Execute(request2);
                JObject obj2 = JObject.Parse(response2.Content);
                var dok = obj2["data"];

                // perhitungan status
                var a = obj["data"].Where(jt => (string)jt["AIPId"] == aipId).Select(s => new
                {
                    b1 = (string)s["ProjectCategory"],
                    b2 = (string)s["JenisPengembangan"],
                    b3 = (string)s["Pengembang"],
                    b4 = (string)s["EksImplementasi"],
                    b5 = (string)s["NamaProject"],
                    b6 = (string)s["NamaAIP"],
                    b7 = (string)s["StrategicImportance"],
                    b8 = (string)s["PPJTIPihakTerkait"]
                }).ToList();

                List<string> vs = new List<string>();
                a.ForEach(o =>
                {
                    string k1 = o.b1.ToString();
                    string k2 = o.b2.ToString();
                    string k3 = o.b3.ToString();
                    string k4 = o.b4.ToString();
                    string k5 = o.b5.ToString();
                    string k6 = o.b6.ToString();
                    string k7 = o.b7.ToString();
                    string k8 = o.b8.ToString();
                    vs.Add(k1);
                    vs.Add(k2);
                    vs.Add(k3);
                    vs.Add(k4);
                    vs.Add(k5);
                    vs.Add(k6);
                    vs.Add(k7);
                    vs.Add(k8);
                });
                vs = vs.Where(o => o != "" && o != null).ToList();

                // pembagian
                int countItems = vs.Count();
                decimal statusResult = countItems / 8m;

                // new json info
                var info = new
                {
                    percentage_completed = statusResult,
                    completed = countItems,
                    uncompleted = 8 - countItems
                };

                return Ok(new { info = info, data = noDok, dokumen = dok });
            }
        }
    }
}
