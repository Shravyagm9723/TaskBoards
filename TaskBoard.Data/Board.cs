using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Data
{

    public class Board
    {
        public int Id { get; init; }

        [Required]
        public string Name { get; init; }

        public IEnumerable<Task> Tasks { get; set; } = new List<Task>();
    }
}
