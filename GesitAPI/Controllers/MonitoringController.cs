using GesitAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GesitAPI.Dtos.MonitoringDto;
using static GesitAPI.Dtos.ReportingDto;
using static GesitAPI.Dtos.ResponseMonitoring;

// Author: Jericho Cristofel Siahaya
// Created: 2021-09-16

namespace GesitAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        [HttpGet("{kategori}")]
        public IActionResult MonitoringGovernanceProject(string kategori)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Monitoring>(response.Content);

            if (result.data.Count <= 0)
                return NoContent();
            foreach (var item in result.data)
            {
                var total = 0;
                var completedCount = 0;
                var uncompletedCount = 0;
                decimal percentageCompleted = 0;
                if (item.Pengembang == "Inhouse" || item.Pengembang == "InHouse")
                {
                    total = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "EstimasiBiayaCapex"
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP"
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));

                    uncompletedCount = total;
                    completedCount = 9 - uncompletedCount;
                    percentageCompleted = completedCount / 9m;
                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
                else
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
                    && x.Name != "status_completed" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP" 
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)) || v is "0"); // delete this if capex/opex 0 is not counted as null

                    uncompletedCount = total;
                    completedCount = 11 - uncompletedCount;
                    percentageCompleted = completedCount / 11m;
                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
            }

            var groupByDivision = result.data.Where(x => x.Divisi != null)
                .GroupBy(o => o.Divisi, (d, r) => new ResponseMonitoring()
                {
                    Division = d,
                    Data = r.Select(x => new Project() { 
                        AIPId = x.AIPId,
                        PercentageCompleted = x.PercentageCompleted,
                        NamaAIP = x.NamaAIP,
                        ProjectId = x.ProjectId,
                        statusAIP = x.statusAIP,
                        StatusProject = x.StatusProject
                    }).ToList()
                }).ToList();

            foreach (var groupItem in groupByDivision)
            {
                float totalProject = groupItem.Data.Count();
                float totalCompleted = groupItem.Data.Where(x => x.PercentageCompleted == 1).Count();
                float totalUncomplete = groupItem.Data.Where(x => x.PercentageCompleted < 1).Count();
                int totalCompletedProgo = groupItem.Data.Where(x => x.statusAIP is "RTP / Production / PIR" or "Cancel / Pending" or "RTP/Production/PIR" or "Cancel/Pending").Count(); // on check kalau ada kesalahan
                int totalUncompleteProgo = groupItem.Data.Where(x => x.statusAIP is not "RTP / Production / PIR" or "Cancel / Pending" or "RTP/Production/PIR" or "Cancel/Pending").Count(); // on check kalau ada kesalahan

                string statusAIP = groupItem.Data.Select(x => x.statusAIP).ToString();
                groupItem.CompletedPercentage = totalCompleted / totalProject;
                //groupItem.Completed = Convert.ToInt32(totalCompleted);
                //groupItem.Uncomplete = Convert.ToInt32(totalUncomplete);
                groupItem.TotalProject = Convert.ToInt32(totalProject);

                groupItem.Status.Add(new StatusInfoMonitoring()
                {
                    CompletedFromPercentage = Convert.ToInt32(totalCompleted),
                    UncompleteFromPercentage = Convert.ToInt32(totalUncomplete),
                    CompletedFromProgo = totalCompletedProgo,
                    UncompleteFromProgo = totalUncompleteProgo
                });
            }

            var json = JsonConvert.SerializeObject(groupByDivision, Formatting.Indented);
            return Ok(json);
        }

        [HttpGet("{kategori}/{divisi}")]
        public IActionResult MonitoringGovernanceProjectByDivisi(string kategori, string divisi)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Monitoring>(response.Content);

            //var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            if (result.data.Count <= 0)
                return NoContent();
            foreach (var item in result.data)
            {
                var total = 0;
                var completedCount = 0;
                var uncompletedCount = 0;
                decimal percentageCompleted = 0;
                if (item.Pengembang == "Inhouse" || item.Pengembang == "InHouse")
                {
                    total = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "EstimasiBiayaCapex"
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP"
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));

                    uncompletedCount = total;
                    completedCount = 9 - uncompletedCount;
                    percentageCompleted = completedCount / 9m;

                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
                else
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
                    && x.Name != "status_completed" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP"
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)) || v is "0"); // delete this if capex/opex 0 is not counted as null

                    uncompletedCount = total;
                    completedCount = 11 - uncompletedCount;
                    percentageCompleted = completedCount / 11m;

                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
            }

            var groupByDivision = result.data.Where(x => x.Divisi == divisi)
                .GroupBy(o => o.Divisi, (d, r) => new ResponseMonitoring()
                {
                    Division = d,
                    Data = r.Select(x => new Project()
                    {
                        AIPId = x.AIPId,
                        PercentageCompleted = x.PercentageCompleted,
                        NamaAIP = x.NamaAIP,
                        ProjectId = x.ProjectId,
                        statusAIP = x.statusAIP,
                        StatusProject = x.StatusProject
                    }).ToList()

                })
                .ToList();

            foreach (var groupItem in groupByDivision)
            {
                float totalProject = groupItem.Data.Count();
                float totalCompleted = groupItem.Data.Where(x => x.PercentageCompleted == 1).Count();
                float totalUncomplete = groupItem.Data.Where(x => x.PercentageCompleted < 1).Count();
                int totalCompletedProgo = groupItem.Data.Where(x => x.statusAIP is "RTP / Production / PIR" or "Cancel / Pending").Count(); // on check kalau ada kesalahan
                //int totalUncompleteProgo = groupItem.Data.Where(x => x.statusAIP is not "Cancel / Pending").Count(); // on check kalau ada kesalahan
                int totalUncompleteProgo = groupItem.Data.Where(x => x.statusAIP is not "RTP / Production / PIR" || x.statusAIP is not "Cancel / Pending").Count();

                groupItem.CompletedPercentage = totalCompleted / totalProject;
                groupItem.TotalProject = Convert.ToInt32(totalProject);
                groupItem.Status.Add(new StatusInfoMonitoring()
                {
                    CompletedFromPercentage = Convert.ToInt32(totalCompleted),
                    UncompleteFromPercentage = Convert.ToInt32(totalUncomplete),
                    CompletedFromProgo = totalCompletedProgo,
                    UncompleteFromProgo = Convert.ToInt32(totalProject) - totalCompletedProgo // TO DO: get count dari linq, jangan dikurang
                });
            }
            
            return Ok(groupByDivision);
        }

        [HttpGet("StatusAll/{kategori}")]
        public IActionResult StatusAll(string kategori)
        {
            var client = new RestClient("http://35.219.107.102/");
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori=" + kategori);
            request.AddHeader("progo-key", "progo123");
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<Monitoring>(response.Content);

            if (result.data.Count <= 0)
                return NoContent();
            foreach (var item in result.data)
            {
                var total = 0;
                var completedCount = 0;
                var uncompletedCount = 0;
                decimal percentageCompleted = 0;
                if (item.Pengembang == "Inhouse" || item.Pengembang == "InHouse")
                {
                    total = item.GetType()
                    .GetProperties()
                    .Where(x => x.Name != "EstimasiBiayaCapex"
                    && x.Name != "EstimasiBiayaOpex" && x.Name != "NamaLOB"
                    && x.Name != "ProjectId" && x.Name != "Durasi"
                    && x.Name != "ProjectValue" && x.Name != "ProjectBudget"
                    && x.Name != "Divisi" && x.Name != "LOB"
                    && x.Name != "Squad" && x.Name != "NamaSquad"
                    && x.Name != "TahunCreate" && x.Name != "PeriodeAIP"
                    && x.Name != "AplikasiTerdampak" && x.Name != "LokasiDRC" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP"
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)));

                    uncompletedCount = total;
                    completedCount = 9 - uncompletedCount;
                    percentageCompleted = completedCount / 9m;
                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
                else
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
                    && x.Name != "status_completed" && x.Name != "AIPId"
                    && x.Name != "StatusInfo" && x.Name != "statusAIP"
                    && x.Name != "PercentageCompeted" && x.Name != "StatusProject")
                    .Select(x => x.GetValue(item, null))
                    .Count(v => v is null || (v is string a && string.IsNullOrWhiteSpace(a)) || v is "0"); // delete this if capex/opex 0 is not counted as null

                    uncompletedCount = total;
                    completedCount = 11 - uncompletedCount;
                    percentageCompleted = completedCount / 11m;
                    item.PercentageCompleted = percentageCompleted;

                    if (item.statusAIP == "RTP / Production / PIR" || item.statusAIP == "Cancel / Pending" || item.statusAIP == "RTP/Production/PIR" || item.statusAIP == "Cancel/Pending")
                    {
                        item.StatusProject = "Completed";
                    }
                    else
                    {
                        item.StatusProject = "Uncomplete";
                    }

                }
            }

            var responseData = result.data.Where(x => x.Divisi != null).ToList();
            var projectCount = responseData.Count();
            var StatusCompletedCountProgo = responseData.Where(x => x.StatusProject is "Completed").Count();
            var StatusUncompleteCountProgo = responseData.Where(x => x.StatusProject is "Uncomplete").Count();
            var StatusCompletedCountPercentage = responseData.Where(x => x.PercentageCompleted == 1).Count();
            var StatusUncompleteCountPercentage = responseData.Where(x => x.PercentageCompleted < 1).Count();

            ResponseMonitoringStatusAll resultStatus = new ResponseMonitoringStatusAll()
            {
                ProjectCount = projectCount,
                CompletedCountFromProgo = StatusCompletedCountProgo,
                UncompleteCountFromProgo = StatusUncompleteCountProgo,
                CompletedCountFromPercentage = StatusCompletedCountPercentage,
                UncompleteCountFromPercentage = StatusUncompleteCountPercentage
            };

            return Ok(resultStatus);
        }
    }
}
