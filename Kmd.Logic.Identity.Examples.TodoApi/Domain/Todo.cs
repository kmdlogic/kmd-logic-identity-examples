using System;
using System.ComponentModel.DataAnnotations;

namespace Kmd.Logic.Identity.Examples.TodoApi.Domain
{
    public class Todo
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        [Required]
        public string Description { get; set; }
    }
}