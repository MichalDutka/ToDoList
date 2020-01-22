using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private Repository<TaskGroup> GroupsRepository
        {
            get
            {
                return (Repository<TaskGroup>)HttpContext.RequestServices.GetService(typeof(Repository<TaskGroup>));
            }
        }

        private Repository<UserTask> TasksRepository
        {
            get
            {
                return (Repository<UserTask>)HttpContext.RequestServices.GetService(typeof(Repository<UserTask>));
            }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<UserTask> tasks = await TasksRepository.GetAll();
            IEnumerable<TaskGroup> groups = await GroupsRepository.GetAll();

            foreach (var task in tasks)
            {
                if (!groups.Any(g => g.UserTasks != null && g.UserTasks.Any(t => t.Id == task.Id)))
                {
                    await TasksRepository.Delete(task.Id);
                }
            }

            ViewData.Model = (await GroupsRepository.GetAll()).ToList();

            return View();
        }

        public async Task<IActionResult> TaskGroupEditor(int id)
        {
            ViewData.Model = await GroupsRepository.GetByID(id);

            return View();
        }

        public async Task<ActionResult> TaskEditor(int id, int groupId)
        {
            ViewData.Add("groupId", groupId);

            if (id >= 0)
            {
                ViewData.Model = await TasksRepository.GetByID(id);
                ViewData["IsNew"] = false;
            }
            else
            {
                DateTime now = DateTime.Now;

                ViewData.Model = new UserTask()
                {
                    Name = "New Task",
                    Deadline = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0),
                    Status = Models.TaskStatus.New,
                    User = 0
                };
                ViewData["IsNew"] = true;
            }

            return PartialView();
        }

        public async Task<IActionResult> SaveTask(UserTask task, bool isNew, int groupId)
        {

            if (!isNew)
            {
                UserTask updatedTask = await TasksRepository.GetByID(task.Id);

                updatedTask.Name = task.Name;
                updatedTask.Status = task.Status;
                updatedTask.Deadline = task.Deadline;
                updatedTask.User = task.User;

                await TasksRepository.Update(updatedTask);
            }
            else
            {
                UserTask newTask = new UserTask()
                {
                    Name = task.Name,
                    Deadline = task.Deadline,
                    Status = task.Status,
                    User = task.User
                };

                await TasksRepository.Insert(newTask);

                TaskGroup group = await GroupsRepository.GetByID(groupId);

                if (group.UserTasks != null)
                {
                    group.UserTasks = group
                        .UserTasks
                        .Concat(new List<UserTask> { newTask })
                        .ToList();
                }
                else
                {
                    group.UserTasks = new List<UserTask>() { newTask };
                }

                await GroupsRepository.Update(group);

            }

            return RedirectToAction("TaskGroupEditor", new { Id = groupId });
        }
        public async Task<IActionResult> RemoveTask(int id, int groupId)
        {
            await TasksRepository.Delete(id);

            TaskGroup group =  await GroupsRepository.GetByID(groupId);
            group.UserTasks = group.UserTasks.Where(t => t.Id != id).ToList();


            return RedirectToAction("TaskGroupEditor", new { Id = groupId });
        }

        public async Task<IActionResult> CreateTaskGroup()
        {
            TaskGroup newGroup = new TaskGroup()
            {
                Name = "New Task Group",
                UserTasks = new List<UserTask>()
            };
            await GroupsRepository.Insert(newGroup);

            return RedirectToAction("TaskGroupEditor", new { id = newGroup.Id });
        }

        public async Task<IActionResult> RemoveTaskGroup(int id)
        {
            TaskGroup group = await GroupsRepository.GetByID(id);

            if (group.UserTasks == null)
            {
                group.UserTasks = new List<UserTask>();

            }

            while (group.UserTasks.Any())
            {
                await TasksRepository.Delete((group.UserTasks.ToList())[0].Id);
            }

            await GroupsRepository.Update(group);

            await GroupsRepository.Delete(id);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SaveGroupName(TaskGroup group)
        {
            TaskGroup updatedGroup = await GroupsRepository.GetByID(group.Id);
            updatedGroup.Name = group.Name;

            await GroupsRepository.Update(updatedGroup);

            return RedirectToAction("TaskGroupEditor", new { id = group.Id });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
