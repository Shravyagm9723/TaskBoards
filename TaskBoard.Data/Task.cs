﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Data
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public int BoardId { get; set; }

        public Board Board { get; init; }

        [Required]
        public string OwnerId { get; set; }

        public User Owner { get; init; }
    }
}
