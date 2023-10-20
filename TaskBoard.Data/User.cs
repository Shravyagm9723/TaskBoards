using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace TaskBoard.Data
{

    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }
    }
}
