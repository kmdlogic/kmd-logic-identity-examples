using System;

namespace Kmd.Logic.Identity.Examples.AggregatedCalendar.Domain
{
    public class CalendarEvent
    {
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}