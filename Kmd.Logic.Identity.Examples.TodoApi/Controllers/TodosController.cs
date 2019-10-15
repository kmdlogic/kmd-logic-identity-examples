using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Examples.TodoApi.Auth;
using Kmd.Logic.Identity.Examples.TodoApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Logic.Identity.Examples.TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly TodoDbContext _todoDbContext;

        public TodosController(TodoDbContext todoDbContext)
        {
            _todoDbContext = todoDbContext;
        }

        [HttpGet]
        [Authorize(Scopes.Admin)]
        public async Task<ActionResult<IEnumerable<AdminTodoDto>>> Get()
        {
            var todos = await _todoDbContext.Todos
                .OrderBy(o => o.Date)
                .ToListAsync();

            return Ok(todos.Select(o => new AdminTodoDto() { UserId = o.UserId, Date = o.Date, Description = o.Description }));
        }

        // GET api/todos/12345
        [HttpGet("{userId}")]
        [Authorize(Scopes.Read)]
        public async Task<ActionResult<IEnumerable<TodoDto>>> Get(string userId)
        {
            var todos = await _todoDbContext.Todos
                .Where(o => o.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.Date)
                .ToListAsync();

            return Ok(todos.Select(o => new TodoDto {Date = o.Date, Description = o.Description}));
        }

        // POST api/todos/12345
        [HttpPost("{userId}")]
        [Authorize(Scopes.Write)]
        public async Task Post(string userId, [FromBody] TodoDto todoDto)
        {
            var newTodo = new Todo
            {
                UserId = userId,
                Date = todoDto.Date,
                Description = todoDto.Description
            };

            _todoDbContext.Todos.Add(newTodo);

            await _todoDbContext.SaveChangesAsync();
        }

        // DELETE api/todos/12345
        [HttpDelete("{userId}")]
        [Authorize(Scopes.Write)]
        public async Task Delete(string userId)
        {
            var todos = await _todoDbContext.Todos
                .Where(o => o.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase))
                .ToListAsync();

            _todoDbContext.Todos.RemoveRange(todos);

            await _todoDbContext.SaveChangesAsync();
        }
    }
}
