using System;
using System.Collections.Generic;

namespace Kmd.Logic.Identity.Examples.DatePublisherService
{
    public static class DatesOfInterest
    {
        private static readonly Random Random = new Random();

        private static readonly List<DateDto> Dates = new List<DateDto>
        {
            new DateDto
            {
                Date = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Description = "New Year's Day"
            },
            new DateDto
            {
                Date = new DateTimeOffset(2019, 3, 20, 0, 0, 0, TimeSpan.Zero),
                Description = "Autumn Equinox"
            },
            new DateDto
            {
                Date = new DateTimeOffset(2019, 6, 21, 0, 0, 0, TimeSpan.Zero),
                Description = "Winter Solstice"
            },
            new DateDto
            {
                Date = new DateTimeOffset(2019, 9, 23, 0, 0, 0, TimeSpan.Zero),
                Description = "Spring Equinox"
            },
            new DateDto
            {
                Date = new DateTimeOffset(2019, 12, 22, 0, 0, 0, TimeSpan.Zero),
                Description = "Summer Solstice"
            }
        };

        public static DateDto GetRandomDateOfInterest()
        {
            return Dates[Random.Next(Dates.Count)];
        }
    }
}