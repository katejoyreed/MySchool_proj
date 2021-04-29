using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySchool.Data;
using MySchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySchool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchedulerController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/scheduler
        public IEnumerable<WebAPIEvent> Get()
        {
            return _context.SchedulerEvents
                .ToList()
                .Select(e => (WebAPIEvent)e);
        }

        // GET: api/scheduler/5
        public WebAPIEvent Get(int id)
        {
            return (WebAPIEvent)_context.SchedulerEvents.Find(id);
        }

        // PUT: api/scheduler/5
        [HttpPut]
        public IActionResult EditSchedulerEvent(int id, WebAPIEvent webAPIEvent)
        {
            var updatedSchedulerEvent = (SchedulerEvent)webAPIEvent;
            updatedSchedulerEvent.Id = id;
            _context.Entry(updatedSchedulerEvent).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(new
            {
                action = "updated"
            });
        }

        // POST: api/scheduler/5
        [HttpPost]
        public IActionResult CreateSchedulerEvent(WebAPIEvent webAPIEvent)
        {
            var newSchedulerEvent = (SchedulerEvent)webAPIEvent;
            _context.SchedulerEvents.Add(newSchedulerEvent);
            _context.SaveChanges();

            return Ok(new
            {
                tid = newSchedulerEvent.Id,
                action = "inserted"
            });
        }

        // DELETE: api/scheduler/5
        [HttpDelete]
        public IActionResult DeleteSchedulerEvent(int id)
        {
            var schedulerEvent = _context.SchedulerEvents.Find(id);
            if (schedulerEvent != null)
            {
                _context.SchedulerEvents.Remove(schedulerEvent);
                _context.SaveChanges();
            }

            return Ok(new
            {
                action = "deleted"
            });
        }

        
    }
}
