using TaskBoard.Data;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskBoard.WebApp.Models.Task
{
   
    public class TaskSearchFormModel
    {
        public string Keyword { get; init; }

        public IEnumerable<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }
}
