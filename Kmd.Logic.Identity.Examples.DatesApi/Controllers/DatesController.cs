using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Examples.DatesApi.Auth;
using Kmd.Logic.Identity.Examples.DatesApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Logic.Identity.Examples.DatesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DatesController : ControllerBase
    {
        private readonly DatesDbContext _datesDbContext;

        public DatesController(DatesDbContext datesDbContext)
        {
            _datesDbContext = datesDbContext;
        }

        // GET api/dates
        [HttpGet]
        [Authorize(Scopes.Read)]
        public async Task<ActionResult<IEnumerable<DateDto>>> Get()
        {
            var dates = await _datesDbContext.DateDetails.ToListAsync();
            return Ok(dates.OrderBy(o => o.Date).Select(o => new DateDto{ Date = o.Date, Description = o.Description }));
        }

        // POST api/dates
        [HttpPost]
        [Authorize(Scopes.Write)]
        public async Task Post([FromBody] DateDto dateDto)
        {
            if (!(await DateExists(dateDto)))
            {
                var newDateDetail = new DateDetail
                {
                    Date = dateDto.Date,
                    Description = dateDto.Description
                };

                _datesDbContext.DateDetails.Add(newDateDetail);
                await _datesDbContext.SaveChangesAsync();
            }
        }

        // DELETE api/dates
        [HttpDelete]
        [Authorize(Scopes.Write)]
        public async Task Delete()
        {
            var existingDates = await _datesDbContext.DateDetails.ToListAsync();
            _datesDbContext.DateDetails.RemoveRange(existingDates);
            await _datesDbContext.SaveChangesAsync();
        }

        private async Task<bool> DateExists(DateDto dateDto)
        {
            return await _datesDbContext.DateDetails.AnyAsync(o => o.Date == dateDto.Date);
        }
    }
}
