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
    public class UserTasksController : ControllerBase
    {

        private Repository<UserTask> Repository
        {
            get
            {
                return (Repository<UserTask>)HttpContext.RequestServices.GetService(typeof(Repository<UserTask>));
            }
        }

        // GET: api/UserTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks()
        {
            return (await Repository.GetAll()).ToList();
        }

        // GET: api/UserTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetUserTask(int id)
        {
            var userTask = await Repository.GetByID(id);

            if (userTask == null)
            {
                return NotFound();
            }

            return userTask;
        }

        // PUT: api/UserTasks/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserTask(int id, UserTask userTask)
        {
            if (id != userTask.Id)
            {
                return BadRequest();
            }

            if (await Repository.Update(userTask))
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }
        }

        // POST: api/UserTasks
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<UserTask>> PostUserTask(UserTask userTask)
        {
            await Repository.Insert(userTask);

            return CreatedAtAction("GetUserTask", new { id = userTask.Id }, userTask);
        }

        // DELETE: api/UserTasks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserTask>> DeleteUserTask(int id)
        {
            var userTask = await Repository.GetByID(id);
            if (userTask == null)
            {
                return NotFound();
            }

            await Repository.Delete(id);

            return userTask;
        }
    }
}
