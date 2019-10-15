using System;
using System.ComponentModel.DataAnnotations;

namespace Kmd.Logic.Identity.Examples.DatesApi.Domain
{
    public class DateDetail
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        [Required]
        public string Description { get; set; }
    }
}