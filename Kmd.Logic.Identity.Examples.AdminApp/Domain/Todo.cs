using System;

namespace Kmd.Logic.Identity.Examples.AdminApp.Domain
{
    public class Todo
    {
        public string UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}