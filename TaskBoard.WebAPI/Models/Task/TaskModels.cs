using TaskBoard.WebAPI.Models.User;

namespace TaskBoard.WebAPI.Models.Task
{
    public class TaskModels : TaskDetails
    {
        public string CreatedOn { get; init; }

        public string Board { get; init; }

        public Users Owner { get; init; }
    }
}
