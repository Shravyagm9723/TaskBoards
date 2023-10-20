using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

using TaskBoard.Data;
using TaskBoard.WebAPI.Models.Response;
using TaskBoard.WebAPI.Models.Task;
using TaskBoard.WebAPI.Models.User;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TaskBoard.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public TasksController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        /// <summary>
        /// Gets a list with all tasks.
        /// </summary>
        /// <response code="200">Returns "OK" with a list of all tasks</response>
        /// <response code="401">Returns "Unauthorized" when user is not authenticated</response>    
        [HttpGet()]
        public IActionResult GetTasks()
        {
            var tasks = this.dbContext
                .Tasks
                .Select(t => new TaskModels()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedOn = t.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                    Board = t.Board.Name,
                    Owner = new Users()
                    {
                        Id = t.OwnerId,
                        Username = t.Owner.UserName,
                        FirstName = t.Owner.FirstName,
                        LastName = t.Owner.LastName,
                        Email = t.Owner.Email
                    }
                })
                .ToList();

            return Ok(tasks);
        }

        /// <summary>
        /// Gets a task by id.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// </remarks>
        /// <response code="200">Returns "OK" with the task</response>
        /// <response code="401">Returns "Unauthorized" when user is not authenticated</response>    
        /// <response code="404">Returns "Not Found" when task with the given id doesn't exist</response> 
        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var taskExists = this.dbContext.Tasks.Any(t => t.Id == id);
            if (!taskExists)
            {
                return NotFound(
                    new ErrorMsg { Message = $"Task #{id} not found." });
            }

            var task = TaskModelById(id);
            return Ok(task);
        }

        /// <summary>
        /// Gets tasks by keyword.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// </remarks>
        /// <response code="200">Returns "OK" with the tasks</response>
        /// <response code="401">Returns "Unauthorized" when user is not authenticated</response>
        [HttpGet("search/{keyword}")]
        public IActionResult GetTasksByKeyword(string keyword)
        {
            var tasks = this.dbContext
                .Tasks
                .Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword))
                .Select(t => new TaskModels()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedOn = t.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                    Board = t.Board.Name,
                    Owner = new Users()
                    {
                        Id = t.OwnerId,
                        Username = t.Owner.UserName,
                        FirstName = t.Owner.FirstName,
                        LastName = t.Owner.LastName,
                        Email = t.Owner.Email
                    }
                })
                .ToList();

            return Ok(tasks);
        }

        /// <summary>
        /// Gets tasks by board name.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// </remarks>
        /// <response code="200">Returns "OK" with the tasks</response>
        /// <response code="401">Returns "Unauthorized" when user is not authenticated</response>
        [HttpGet("board/{boardName}")]
        public IActionResult GetTasksByBoardName(string boardName)
        {
            var tasks = this.dbContext
                .Tasks
                .Where(t => t.Board.Name == boardName)
                .Select(t => new TaskModels()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedOn = t.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                    Board = t.Board.Name,
                    Owner = new Users()
                    {
                        Id = t.OwnerId,
                        Username = t.Owner.UserName,
                        FirstName = t.Owner.FirstName,
                        LastName = t.Owner.LastName,
                        Email = t.Owner.Email
                    }
                })
                .ToList();

            return Ok(tasks);
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// </remarks>
        /// <response code="201">Returns "Created" with the created event</response>
        /// <response code="400">Returns "Bad Request" when an invalid request is sent</response>   
        /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
        [HttpPost("create")]
        public IActionResult CreateTask(TaskBindingModel taskModel)
        {
            if (!this.dbContext.Boards.Any(b => b.Name == taskModel.Board))
            {
                return BadRequest(
                    new ErrorMsg { Message = $"Board {taskModel.Board} name does not exist." });
            }

            Task task = new Task()
            {
                Title = taskModel.Title,
                Description = taskModel.Description,
                BoardId = this.dbContext.Boards.FirstOrDefault(b => b.Name == taskModel.Board).Id,
                CreatedOn = DateTime.Now,
                OwnerId = GetCurrentUserId()
            };

            this.dbContext.Tasks.Add(task);
            this.dbContext.SaveChanges();

            var taskListingModel = TaskModelById(task.Id);

            return CreatedAtAction("GetTaskById", new { id = task.Id }, taskListingModel);
        }

        /// <summary>
        /// Edits a task.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// </remarks>
        /// <response code="204">Returns "No Content"</response>
        /// <response code="400">Returns "Bad Request" when an invalid request is sent</response>   
        /// <response code="401">Returns "Unauthorized" when user is not authenticated or is not the owner of the task</response>  
        /// <response code="404">Returns "Not Found" when task with the given id doesn't exist</response>  
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskBindingModel taskModel)
        {
            var taskExists = this.dbContext.Tasks.Any(t => t.Id == id);
            if (!taskExists)
            {
                return NotFound(
                    new ErrorMsg { Message = $"Task #{id} not found." });
            }

            var task = this.dbContext.Tasks.FirstOrDefault(t => t.Id == id);
            task.Title = taskModel.Title;
            task.Description = taskModel.Description;
            task.BoardId = GetBoardId(taskModel.Board);

            this.dbContext.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <remarks>
        /// You should be an authenticated user!
        /// You should be the owner of the deleted task!
        /// </remarks>
        /// <response code="200">Returns "OK" with the deleted task</response>
        /// <response code="401">Returns "Unauthorized" when user is not authenticated or is not the owner of the task</response>  
        /// <response code="404">Returns "Not Found" when task with the given id doesn't exist</response> 
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var taskExists = this.dbContext.Tasks.Any(t => t.Id == id);
            if (!taskExists)
            {
                return NotFound(
                    new ErrorMsg { Message = $"Task #{id} not found." });
            }

            var task = this.dbContext.Tasks.FirstOrDefault(t => t.Id == id);
            var taskListingModel = TaskModelById(task.Id);

            this.dbContext.Tasks.Remove(task);
            this.dbContext.SaveChanges();

            return Ok(taskListingModel);
        }

        private TaskModels TaskModelById(int id)
            => this.dbContext
                .Tasks
                .Where(t => t.Id == id)
                .Select(t => 
                    new TaskModels()
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        CreatedOn = t.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                        Board = t.Board.Name,
                        Owner = new Users()
                        {
                            Id = t.OwnerId,
                            Username = t.Owner.UserName,
                            FirstName = t.Owner.FirstName,
                            LastName = t.Owner.LastName,
                            Email = t.Owner.Email
                        }
                    })
                .FirstOrDefault();

        private string GetCurrentUserId()
        {
            string currentUsername = this.User.FindFirst(ClaimTypes.Name)?.Value;
            var currentUserId = this.dbContext
                .Users
                .FirstOrDefault(x => x.UserName == currentUsername)
                .Id;
            return currentUserId;
        }

        private int GetBoardId(string boardName)
            => this.dbContext.Boards.FirstOrDefault(b => b.Name == boardName).Id;
    }
}
