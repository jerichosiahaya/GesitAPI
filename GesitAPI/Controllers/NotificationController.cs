using GesitAPI.Data;
using GesitAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Notification>> Get()
        {
            //return new string[] { "value1", "value2" };
            return await _notification.GetAll();
        }

        // GET api/<NotificationsController>/5
        [HttpGet("{id}")]
        public async Task<Notification> Get(int id)
        {
            var results = await _notification.GetById(id.ToString());
            return results;
        }


        // POST api/<NotificationsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Notification notification)
        {
            try
            {
                await _notification.Insert(notification);
                return Ok($"Data {notification.Id} berhasil ditambahkan!");
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
