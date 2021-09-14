using GesitAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GesitAPI.Dtos.ReportingDto;

// Author: Jericho Siahaya
// Created: 2021-09-10

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        //public class Root
        //{
        //    public List<MappingReporting> data { get; set; }
        //}

        [HttpGet(nameof(RPTI))]
        public IActionResult RPTI(string kategori)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori="+kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Root>(response.Content);
            if (result.data.Count() <= 0)
                return NoContent();
            foreach (var item in result.data)
            {
                var total = 0;
                var completedCount = 0;
                var uncompletedCount = 0;
                decimal percentageCompleted = 0;
                var statusCompleted = "Uncomplete";
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
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId" && x.Name != "StatusInfo")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));

                    var completedProperties = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "EstimasiBiayaCapex"
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId" 
                    && x.Name != "StatusInfo")
                    .Select(pi => new { Val = (string)pi.GetValue(item), Name = pi.Name })
                    .Where(pi => !string.IsNullOrEmpty(pi.Val))
                    .ToDictionary(pi => pi.Name, pi => pi.Val);

                    var uncompletedProperties = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "EstimasiBiayaCapex"
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" )
                    .Select(pi => new { Val = (string)pi.GetValue(item), Name = pi.Name })
                    .Where(pi => string.IsNullOrEmpty(pi.Val))
                    .ToDictionary(pi => pi.Name, pi => pi.Val);

                    uncompletedCount = total;
                    completedCount = 9- uncompletedCount;
                    percentageCompleted = completedCount / 9m;

                    if (uncompletedCount <= 0)
                        statusCompleted = "Complete";

                    item.StatusInfo.Add(new StatusInfo() { 
                        Status = statusCompleted,
                        CountUncompleted = uncompletedCount,
                        CountCompleted = completedCount,
                        PercentageCompleted = percentageCompleted,
                        Uncompleted = uncompletedProperties,
                        Completed = completedProperties
                    });

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
                    && x.Name != "status_completed" && x.Name != "AIPId" && x.Name != "StatusInfo")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)) || v is "0"); // delete this if capex/opex 0 is not counted as null

                    var completedProperties = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo")
                    .Select(pi => new { Val = (string)pi.GetValue(item), Name = pi.Name })
                    .Where(pi => !string.IsNullOrEmpty(pi.Val) && pi.Val is not "0") // delete this if capex/opex 0 is not counted as null
                    .ToDictionary(pi => pi.Name, pi => pi.Val);

                    var uncompletedProperties = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo")
                    .Select(pi => new { Val = (string)pi.GetValue(item), Name = pi.Name })
                    .Where(v => v.Val is null || (v.Val is string a && string.IsNullOrWhiteSpace(a)) || v.Val is "0") // delete this if capex/opex 0 is not counted as null
                    .ToDictionary(pi => pi.Name, pi => pi.Val);

                    uncompletedCount = total;
                    completedCount = 11 - uncompletedCount;
                    percentageCompleted = completedCount / 11m;

                    if (uncompletedCount <= 0)
                        statusCompleted = "Complete";

                    item.StatusInfo.Add(new StatusInfo()
                    {
                        Status = statusCompleted,
                        CountUncompleted = uncompletedCount,
                        CountCompleted = completedCount,
                        PercentageCompleted = percentageCompleted,
                        Uncompleted = uncompletedProperties,
                        Completed = completedProperties
                    });

                }
            }
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return Ok(json);
        }
    }
}
