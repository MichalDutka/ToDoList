using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskGroupsController : ControllerBase
    {
        private Repository<TaskGroup> Repository
        {
            get
            {
                return (Repository<TaskGroup>)HttpContext.RequestServices.GetService(typeof(Repository<TaskGroup>));
            }
        }

        // GET: api/TaskGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskGroup>>> GetGroups()
        {
            return (await Repository.GetAll()).ToList();
        }

        // GET: api/TaskGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskGroup>> GetTaskGroup(int id)
        {
            var taskGroup = await Repository.GetByID(id);

            if (taskGroup == null)
            {
                return NotFound();
            }

            return taskGroup;
        }

        // PUT: api/TaskGroups/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskGroup(int id, TaskGroup taskGroup)
        {
            if (id != taskGroup.Id)
            {
                return BadRequest();
            }

            if (await Repository.Update(taskGroup))
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }
        }

        // POST: api/TaskGroups
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TaskGroup>> PostTaskGroup(TaskGroup taskGroup)
        {
            await Repository.Insert(taskGroup);

            return CreatedAtAction("GetTaskGroup", new { id = taskGroup.Id }, taskGroup);
        }

        // DELETE: api/TaskGroups/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskGroup>> DeleteTaskGroup(int id)
        {
            var taskGroup = await Repository.GetByID(id);
            if (taskGroup == null)
            {
                return NotFound();
            }

            await Repository.Delete(id);

            return taskGroup;
        }
    }
}
