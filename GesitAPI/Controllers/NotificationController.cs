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
    //[Authorize]
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
            // TO DO DIPINDAHKAN KE STARTUP
            // automapper config
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Notification, NotificationDto>()
                );

            var result = await _notification.GetAll();
            var mapper = new Mapper(config);
            List<NotificationDto> resultData = mapper.Map<List<Notification>, List<NotificationDto>>(result.ToList());
            return Ok(resultData);
        }

        // GET api/<NotificationsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // TO DO DIPINDAHKAN KE STARTUP
            // automapper config
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Notification, NotificationDto>()
                );

            var result = await _notification.GetById(id.ToString());
            var mapper = new Mapper(config);
            var empDTO = mapper.Map<NotificationDto>(result);
            return Ok(empDTO);
        }


        // POST api/<NotificationsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] NotificationInsert notification)
        {
            try
            {
                var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<NotificationDto, Notification>()
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
