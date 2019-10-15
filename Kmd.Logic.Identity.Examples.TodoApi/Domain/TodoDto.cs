using System;

namespace Kmd.Logic.Identity.Examples.TodoApi.Domain
{
    public class TodoDto
    {
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}