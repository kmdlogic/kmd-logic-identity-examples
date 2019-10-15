using System;

namespace Kmd.Logic.Identity.Examples.DatesApi.Domain
{
    public class DateDto
    {
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}