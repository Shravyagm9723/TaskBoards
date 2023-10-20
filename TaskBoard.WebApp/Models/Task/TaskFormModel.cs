using TaskBoard.Data;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskBoard.WebApp.Models.Task
{
    public class TaskFormModel
    {
        [Required]       
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Board")]
        public int BoardId { get; set; }

        public IEnumerable<TaskBoardModel> Boards { get; set; }
    }
}
