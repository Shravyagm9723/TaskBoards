using TaskBoard.Data;

using System.ComponentModel.DataAnnotations;

namespace TaskBoard.WebAPI.Models.Task
{
    public class TaskBindingModel
    {
        [Required]
        public string Title { get; init; }

        [Required]       
        public string Description { get; init; }

        [Required]
        public string Board { get; init; }
    }
}
