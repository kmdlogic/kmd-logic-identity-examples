using System;

namespace Kmd.Logic.Identity.Examples.TodoApi.Domain
{
    public class AdminTodoDto
    {
        public string UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}