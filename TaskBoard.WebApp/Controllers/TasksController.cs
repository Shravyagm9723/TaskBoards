using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

using TaskBoard.Data;
using TaskBoard.WebApp.Models.Task;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TaskBoard.WebApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public TasksController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public IActionResult Details(int id)
        {
            var task = this.dbContext
                .Tasks
                .Where(t => t.Id == id)
                .Select(t => new TaskDetailsViewModel()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedOn = t.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                    Board = t.Board.Name,
                    Owner = t.Owner.UserName
                })
                .FirstOrDefault();


            if (task == null)
            {
                return BadRequest();
            }

            return View(task);
        }

        [HttpPost]
        public IActionResult Create(TaskFormModel taskModel)
        {
            if (!GetBoards().Any(b => b.Id == taskModel.BoardId))
            {
                this.ModelState.AddModelError(nameof(taskModel.BoardId), "Board does not exist.");
            }

            if (!ModelState.IsValid)
            {
                taskModel.Boards = GetBoards();

                return View(taskModel);
            }

            string currentUserId = GetUserId();
            Task task = new Task()
            {
                Title = taskModel.Title,
                Description = taskModel.Description,
                CreatedOn = DateTime.Now,
                BoardId = taskModel.BoardId,
                OwnerId = currentUserId
            };
            this.dbContext.Tasks.Add(task);
            this.dbContext.SaveChanges();

            var boards = this.dbContext.Boards;

            return RedirectToAction("All", "Boards");
        }

        [HttpPost]
        public IActionResult Delete(TaskViewModel taskModel)
        {
            Task task = dbContext.Tasks.Find(taskModel.Id);
            if (task == null)
            {
                return BadRequest();
            }

            string currentUserId = GetUserId();
            if (currentUserId != task.OwnerId)
            {
                // Not an owner -> return "Unauthorized"
                return Unauthorized();
            }

            this.dbContext.Tasks.Remove(task);
            this.dbContext.SaveChanges();
            return RedirectToAction("All", "Boards");
        }

        [HttpPost]
        public IActionResult Search(TaskSearchFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tasks = this.dbContext
                .Tasks
                .Select(t => new TaskViewModel()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Owner = t.Owner.UserName
                });

            var keyword = model.Keyword == null ? string.Empty : model.Keyword.Trim().ToLower();
            if (!String.IsNullOrEmpty(keyword) && !String.IsNullOrEmpty(keyword))
            {
                tasks = tasks
                .Where(t => t.Title.ToLower().Contains(keyword)
                    || t.Description.ToLower().Contains(keyword));
            }

            model.Tasks = tasks;

            return View(model);
        }

        private IEnumerable<TaskBoardModel> GetBoards()
            => this.dbContext
                .Boards
                .Select(b => new TaskBoardModel()
                {
                    Id = b.Id,
                    Name = b.Name
                });

        private string GetUserId()
            => this.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
