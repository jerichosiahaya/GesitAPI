using AutoMapper;
using GesitAPI.Data;
using GesitAPI.Dtos;
using GesitAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GesitAPI.Dtos.NotificationDto;
using static GesitAPI.Dtos.ProgoDocumentDto;
using static GesitAPI.Dtos.ProgoProjectDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INotification _notification;
        private GesitDbContext _db;
        public NotificationController(INotification notification, IConfiguration config, GesitDbContext db)
        {
            _notification = notification;
            _config = config;
            _db = db;
        }

        // GET: api/<NotificationsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            List<NotificationView> resultData = new List<NotificationView>();
            NotificationView tempData = new NotificationView();
            var result = await _notification.GetAll();

            foreach (var o in result)
            {
                //tempData.Id = o.Id;
                //tempData.ProjectCategory = o.ProjectCategory;
                //tempData.ProjectDocument = o.ProjectDocument;
                //tempData.ProjectId = o.ProjectId;
                //tempData.ProjectTitle = o.ProjectTitle;
                //tempData.Status = o.Status;
                //tempData.TargetDate = o.TargetDate.ToString("yyyy-MM-dd");

                resultData.Add(new NotificationView
                {
                    Id = o.Id,
                    ProjectCategory = o.ProjectCategory,
                    ProjectDocument = o.ProjectDocument,
                    ProjectId = o.ProjectId,
                    ProjectTitle = o.ProjectTitle,
                    Status = o.Status,
                    TargetDate = o.TargetDate.ToString("yyyy-MM-dd")
                });
            }

            return Ok(resultData);
        }

        [HttpGet(nameof(GetActive))]
        public async Task<IActionResult> GetActive()
        {

            List<NotificationView> resultData = new List<NotificationView>();
            NotificationView tempData = new NotificationView();
            var result = await _notification.GetActive();

            foreach (var o in result)
            {
                resultData.Add(new NotificationView
                {
                    Id = o.Id,
                    ProjectCategory = o.ProjectCategory,
                    ProjectDocument = o.ProjectDocument,
                    ProjectId = o.ProjectId,
                    ProjectTitle = o.ProjectTitle,
                    Status = o.Status,
                    TargetDate = o.TargetDate.ToString("yyyy-MM-dd")
                });
            }

            return Ok(resultData);
        }

        [HttpGet(nameof(GetNotActive))]
        public async Task<IActionResult> GetNotActive()
        {

            List<NotificationView> resultData = new List<NotificationView>();
            NotificationView tempData = new NotificationView();
            var result = await _notification.GetNotActive();

            foreach (var o in result)
            {
                resultData.Add(new NotificationView
                {
                    Id = o.Id,
                    ProjectCategory = o.ProjectCategory,
                    ProjectDocument = o.ProjectDocument,
                    ProjectId = o.ProjectId,
                    ProjectTitle = o.ProjectTitle,
                    Status = o.Status,
                    TargetDate = o.TargetDate.ToString("yyyy-MM-dd")
                });
            }

            return Ok(resultData);
        }

        // GET api/<NotificationsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            NotificationView resultData = new NotificationView();
            var result = await _notification.GetById(id.ToString());

            resultData.Id = result.Id;
            resultData.ProjectCategory = result.ProjectCategory;
            resultData.ProjectDocument = result.ProjectDocument;
            resultData.ProjectId = result.ProjectId;
            resultData.ProjectTitle = result.ProjectTitle;
            resultData.Status = result.Status;
            resultData.TargetDate = result.TargetDate.ToString("yyyy-MM-dd");

            return Ok(resultData);
        }

        [HttpGet("GetNotificationByProjectId/{projectId}")]
        public async Task<IActionResult> GetNotificationByProjectId(string projectId)
        {
            var result = await _notification.GetNotificationByProjectId(projectId);
            if (result == null)
            {
                return NotFound();
            } else
            {
                List<NotificationView> resultData = new List<NotificationView>();
                NotificationView tempData = new NotificationView();

                foreach (var o in result)
                {
                    resultData.Add(new NotificationView
                    {
                        Id = o.Id,
                        ProjectCategory = o.ProjectCategory,
                        ProjectDocument = o.ProjectDocument,
                        ProjectId = o.ProjectId,
                        ProjectTitle = o.ProjectTitle,
                        Status = o.Status,
                        TargetDate = o.TargetDate.ToString("yyyy-MM-dd")
                    });
                }

                return Ok(resultData);
            }
        }

        // POST api/<NotificationsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificationInsert notification)
        {
            try
            {
                var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<NotificationInsert, Notification>()
                );

                var mapper = new Mapper(config);
                var insertData = mapper.Map<Notification>(notification);

                await _notification.Insert(insertData);
                return Ok($"Data {insertData.Id} berhasil ditambahkan!");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // PUT api/<NotificationsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Notification notification)
        {
            try
            {
                await _notification.Update(id.ToString(), notification);
                return Ok($"Data {notification.Id} berhasil diupdate!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // HIDDEN CONTROLLER BELOW (MOVE TO CONSOLE APP)

        // PROGO UPDATE
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(nameof(UpdateStatusNotification))]
        public async Task<IActionResult> UpdateStatusNotification()
        {
            var requestUrl = _config.GetValue<string>("ServerSettings:Progo:Url");
            var apiKey = _config.GetValue<string>("ServerSettings:Progo:ProgoKey");

            var client = new RestClient(requestUrl);
            client.UseNewtonsoftJson();
            var request = new RestRequest("progodev/api/project?kategori=RBB");
            request.AddHeader("progo-key", apiKey);
            var response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<RootProgoProject>(response.Content);
            if (result.data.Count <= 0)
                return NoContent();

            var dateTimeNow = DateTime.Now;

            foreach (var o in result.data)
            {
                ProgoProject progoProject = new ProgoProject();
                
                // check the aip id
                var countCheckAIP = _db.ProgoProjects.Where(p => p.AipId == o.AIPId).Count();
                
                // insert if empty
                if (countCheckAIP == 0)
                {
                    progoProject.AipId = o.AIPId;
                    progoProject.NamaAip = o.NamaAIP;
                    progoProject.ProjectId = o.ProjectId;
                    progoProject.NamaProject = o.NamaProject;
                    progoProject.ProjectBudget = Convert.ToInt32(o.ProjectBudget);
                    progoProject.ProjectDemandValue = Convert.ToInt64(o.ProjectDemandValue);
                    progoProject.StrategicImportance = o.StrategicImportance;
                    progoProject.Durasi = Convert.ToInt32(o.Durasi);
                    progoProject.EksImplementasi = o.EksImplementasi;
                    progoProject.Divisi = o.Divisi;
                    progoProject.Lob = o.LOB;
                    progoProject.NamaLob = o.NamaLOB;
                    progoProject.Squad = o.Squad;
                    progoProject.NamaSquad = o.NamaSquad;
                    progoProject.TahunCreate = Convert.ToInt32(o.TahunCreate);
                    progoProject.PeriodeAip = Convert.ToInt32(o.PeriodeAIP);
                    progoProject.AplikasiTerdampak = o.AplikasiTerdampak;
                    progoProject.ProjectCategory = o.ProjectCategory;
                    progoProject.JenisPengembangan = o.JenisPengembangan;
                    progoProject.Pengembang = o.Pengembang;
                    progoProject.PpjtiPihakTerkait = o.PPJTIPihakTerkait;
                    progoProject.LokasiDc = o.LokasiDC;
                    progoProject.LokasiDrc = o.LokasiDRC;
                    progoProject.EstimasiBiayaCapex = Convert.ToInt32(o.EstimasiBiayaCapex);
                    progoProject.EstimasiBiayaOpex = Convert.ToInt32(o.EstimasiBiayaOpex);
                    progoProject.StatusAip = o.statusAIP;
                    _db.ProgoProjects.Add(progoProject);
                }
                // update if exists
                else if (countCheckAIP == 1) 
                { 
                    var dataProject = _db.ProgoProjects.Where(p => p.AipId == o.AIPId).FirstOrDefault();
                    dataProject.NamaAip = o.NamaAIP;
                    dataProject.ProjectId = o.ProjectId;
                    dataProject.NamaProject = o.NamaProject;
                    dataProject.ProjectBudget = Convert.ToInt32(o.ProjectBudget);
                    dataProject.ProjectDemandValue = Convert.ToInt64(o.ProjectDemandValue);
                    dataProject.StrategicImportance = o.StrategicImportance;
                    dataProject.Durasi = Convert.ToInt32(o.Durasi);
                    dataProject.EksImplementasi = o.EksImplementasi;
                    dataProject.Divisi = o.Divisi;
                    dataProject.Lob = o.LOB;
                    dataProject.NamaLob = o.NamaLOB;
                    dataProject.Squad = o.Squad;
                    dataProject.NamaSquad = o.NamaSquad;
                    dataProject.TahunCreate = Convert.ToInt32(o.TahunCreate);
                    dataProject.PeriodeAip = Convert.ToInt32(o.PeriodeAIP);
                    dataProject.AplikasiTerdampak = o.AplikasiTerdampak;
                    dataProject.ProjectCategory = o.ProjectCategory;
                    dataProject.JenisPengembangan = o.JenisPengembangan;
                    dataProject.Pengembang = o.Pengembang;
                    dataProject.PpjtiPihakTerkait = o.PPJTIPihakTerkait;
                    dataProject.LokasiDc = o.LokasiDC;
                    dataProject.LokasiDrc = o.LokasiDRC;
                    dataProject.EstimasiBiayaCapex = Convert.ToInt32(o.EstimasiBiayaCapex);
                    dataProject.EstimasiBiayaOpex = Convert.ToInt32(o.EstimasiBiayaOpex);
                    dataProject.StatusAip = o.statusAIP;
                    await _db.SaveChangesAsync();
                }

                // get progo documents
                var requestDocuments = new RestRequest("progodev/api/dokumen?AIPId=" + o.AIPId + "-" + dateTimeNow.Year);
                requestDocuments.AddHeader("progo-key", apiKey);
                var responseDocuments = client.Execute(requestDocuments);
                var resultDocuments = JsonConvert.DeserializeObject<RootProgoDocument>(responseDocuments.Content);

                if (resultDocuments.data.Count > 0)
                {
                    foreach (var i in resultDocuments.data)
                    {
                        ProgoDocument progoDocument = new ProgoDocument();

                        // check file name first
                        var countCheckFileName = _db.ProgoDocuments.Where(p => p.NamaFile.Equals(i.NamaFile)).Count();

                        if (countCheckFileName == 0)
                        {
                            progoDocument.AipId = o.AIPId;
                            progoDocument.JenisDokumen = i.JenisDokumen;
                            progoDocument.TaskId = i.TaskId;
                            progoDocument.Tahun = dateTimeNow.Year;
                            progoDocument.NamaFile = i.NamaFile;
                            progoDocument.UrlDownloadFile = i.UrlDownloadFile;
                            _db.ProgoDocuments.Add(progoDocument);
                        }                      
                    }
                }
            }
            await _db.SaveChangesAsync();

            // comparing to notification table
            var dataNotification = _db.Notifications.Where(o => o.Status == 0).ToList();
            foreach (var p in dataNotification)
            {
                if (p.ProjectDocument.Equals("Requirement"))
                {
                    var checkReq = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Memo Requirement") && o.AipId == p.ProjectId).Count();
                    if (checkReq >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Cost & Benefit Analysis"))
                {
                    var checkCost = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Cost & Benefit  Analysis") && o.AipId == p.ProjectId).Count();
                    if (checkCost >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Severity Sistem"))
                {
                    var checkSev = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Severity Sistem") && o.AipId == p.ProjectId).Count();
                    if (checkSev >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Bussiness Impact Analysis"))
                {
                    var checkBus = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Bussiness Impact Analysis") && o.AipId == p.ProjectId).Count();
                    if (checkBus >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Izin / Lapor Regulator"))
                {
                    var checkKajian = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Kajian untuk ijin/lapor regulatori") && o.AipId == p.ProjectId).Count();
                    if (checkKajian >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Budgeting Capex / Opex"))
                {
                    var checkAnggaran = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Anggaran atau Ijin Prinsip (Capex/Opex)") && o.AipId == p.ProjectId).Count();
                    if (checkAnggaran >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Arsitektur / Topologi"))
                {
                    var checkArsi = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Arsitektur atau topologi(AAD)") && o.AipId == p.ProjectId).Count();
                    if (checkArsi >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Risk"))
                {
                    var checkRisk = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Asement Risk ") && o.AipId == p.ProjectId).Count();
                    if (checkRisk >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Target Implementasi"))
                {
                    var checkTarget = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.EksImplementasi).FirstOrDefault();
                    if (checkTarget != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Kategori Project"))
                {
                    var checkProject = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.ProjectCategory).FirstOrDefault();
                    if (checkProject != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Sistem / App Impact"))
                {
                    var checkAppTerdampak = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.AplikasiTerdampak).FirstOrDefault();
                    if (checkAppTerdampak == "")
                    {
                        p.Status = 0;
                    } else
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Pengadaan / In House"))
                {
                    var checkPengadaan = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.Pengembang).FirstOrDefault();
                    if (checkPengadaan != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("New / Enhance"))
                {
                    var checkNewEnhance = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.JenisPengembangan).FirstOrDefault();
                    if (checkNewEnhance != "")
                    {
                        p.Status = 1;
                    }
                }
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(nameof(UpdateStatus))]
        public async Task<IActionResult> UpdateStatus()
        {
            var dataNotification = _db.Notifications.Where(o => o.Status == 0).ToList();
            foreach (var p in dataNotification)
            {
                if (p.ProjectDocument.Equals("Requirement"))
                {
                    var checkReq = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Memo Requirement") && o.AipId == p.ProjectId).Count();
                    if (checkReq >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Cost & Benefit Analysis"))
                {
                    var checkCost = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Cost & Benefit  Analysis") && o.AipId == p.ProjectId).Count();
                    if (checkCost >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Severity Sistem"))
                {
                    var checkSev = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Severity Sistem") && o.AipId == p.ProjectId).Count();
                    if (checkSev >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Bussiness Impact Analysis"))
                {
                    var checkBus = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Bussiness Impact Analysis") && o.AipId == p.ProjectId).Count();
                    if (checkBus >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Izin / Lapor Regulator"))
                {
                    var checkKajian = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Kajian untuk ijin/lapor regulatori") && o.AipId == p.ProjectId).Count();
                    if (checkKajian >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Budgeting Capex / Opex"))
                {
                    var checkAnggaran = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Anggaran atau Ijin Prinsip (Capex/Opex)") && o.AipId == p.ProjectId).Count();
                    if (checkAnggaran >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Arsitektur / Topologi"))
                {
                    var checkArsi = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Arsitektur atau topologi(AAD)") && o.AipId == p.ProjectId).Count();
                    if (checkArsi >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Risk"))
                {
                    var checkRisk = _db.ProgoDocuments.Where(o => o.JenisDokumen.Equals("Asement Risk ") && o.AipId == p.ProjectId).Count();
                    if (checkRisk >= 1)
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Target Implementasi"))
                {
                    var checkTarget = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.EksImplementasi).FirstOrDefault();
                    if (checkTarget != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Kategori Project"))
                {
                    var checkProject = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.ProjectCategory).FirstOrDefault();
                    if (checkProject != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Sistem / App Impact"))
                {
                    var checkAppTerdampak = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.AplikasiTerdampak).FirstOrDefault();
                    if (checkAppTerdampak != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("Pengadaan / In House"))
                {
                    var checkPengadaan = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.Pengembang).FirstOrDefault();
                    if (checkPengadaan != "")
                    {
                        p.Status = 1;
                    }
                }
                else if (p.ProjectDocument.Equals("New / Enhance"))
                {
                    var checkNewEnhance = _db.ProgoProjects.Where(o => o.AipId == p.ProjectId).Select(o => o.JenisPengembangan).FirstOrDefault();
                    if (checkNewEnhance != "")
                    {
                        p.Status = 1;
                    }
                }
            }
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}

//kalo dari api dokumen yang kita ambil
//- Memo Requirement --> Requirement

//- Cost and efficiency Benefit Analysis --> Cost & Benefit Analysis

//- Severity Sistem

//- Bussiness Impact Analysis

//- Kajian untuk ijin/lapor regulatori --> Izin / Lapor Regulator

//- Anggaran atau Ijin Prinsip (Capex/Opex) --> Budgeting Capex / Opex

//- Arsitektur atau topologi(AAD) --> Artistektur / Topologi

//- Asement Risk -- > Risk

// yang dari project
// eksImplementasi
// projectCategory
// aplikasiTerdampak
// pengembang
// jenisPengembangan
