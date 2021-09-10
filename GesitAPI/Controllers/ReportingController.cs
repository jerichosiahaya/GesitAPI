using GesitAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Author: Jericho Siahaya
// Created: 2021-09-10

namespace GesitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        public class Root
        {
            public List<MappingReporting> data { get; set; }
        }

        [HttpGet(nameof(RPTI))]
        public IActionResult RPTI()
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori=RBB");
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Root>(response.Content);
            // to do
            // cek dulu datanya kosong atau tidak


            foreach (var item in result.data)
            {
                var total = 0;
                if (item.Pengembang == "Inhouse" || item.Pengembang == "InHouse")
                {
                    total = item.GetType()
                    .GetProperties()
                    .Where(x=> x.Name != "EstimasiBiayaCapex" 
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB" 
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" 
                    && x.Name != "status_completed" && x.Name != "AIPId" && x.Name != "info")
                    .Select(x => x.GetValue(item, null))
                    .Count(/*v => v is null || (v is string a && string.IsNullOrWhiteSpace(a))*/);

                    // to do
                    // hitung percentage

                    item.StatusInfo.Add(new StatusInfo() { StatusCompleted = 1 });

                } else
                {
                    total = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC"
                    && x.Name != "status_completed" && x.Name != "AIPId" && x.Name != "info")
                    .Select(x => x.GetValue(item, null))
                    .Count(/*v => v is null || (v is string a && string.IsNullOrWhiteSpace(a))*/);
                    
                    // to do
                    // hitung percentage

                    item.StatusInfo.Add(new StatusInfo() { StatusCompleted = 1 });

                }
            }
            var json = JsonConvert.SerializeObject(result);
            return Ok(json);
        }
    }
}
