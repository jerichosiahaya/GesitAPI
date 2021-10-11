using AutoMapper;
using GesitAPI.Data;
using GesitAPI.Dtos;
using GesitAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GesitAPI.Dtos.NotificationDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GesitAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotification _notification;
        public NotificationController(INotification notification)
        {
            _notification = notification;
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
    }
}
