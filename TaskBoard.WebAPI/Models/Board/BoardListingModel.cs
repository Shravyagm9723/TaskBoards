using System.Collections.Generic;

using TaskBoard.WebAPI.Models.Task;

namespace TaskBoard.WebAPI.Models.Board
{
    public class BoardListingModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public IEnumerable<TaskDetails> Tasks { get; set; } = new List<TaskDetails>();
    }
}
