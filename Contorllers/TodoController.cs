using System;
using System.Threading.Tasks;
using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuTodo.Contorllers
{
    [ApiController]
    [Route("v1")]
    public class TodoController : ControllerBase
    {
        // Primeira forma de fazermos injecao de dependencia 

        // Uma forma de usar injecao de dependencia

        // public TodoController(AppDbContext context)
        // {
        //     
        // }

        [HttpGet]
        [Route("todos")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var todos = await context
                .Todos
                .AsNoTracking() // Usamos ele pra ele nao trackear nada que esta aqui pois não tem necessidade de ver 
                // cada um desses itens para saber qual foi alterado, excluido... nao tem necessidade.
                // Pois não vamos fazer nada com essa lista a não ser jogar elas pra tela.
                .ToListAsync();

            return Ok(todos);
        }

        [HttpGet]
        [Route("todos/{id}")] // Adicionando um paramentro
        public async Task<IActionResult> GetByIdAsync([FromServices] AppDbContext context,
                [FromRoute] int id) // [FromRoute] usado para explicitar que veio da rota.
            // Caso voce tiver algum parametro que vem da url voce poderia 
            // passar o [FromQuery] pois o parametro esta vindo da queryString
        {
            var todo = await context
                .Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return todo == null ? NotFound() : Ok(todo);
        }

        // Aqui iremos precisar passar nossa classe do banco e nosso model Todo.
        // Existe duas formas. Passando o Todo e criando um ViewModel.
        [HttpPost("todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid) // Valida se o campo esta preenchido
                return BadRequest();
            
            var todo = new Todo()
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo); // Salva somente na memoria
                await context.SaveChangesAsync(); // Salvar no banco
                return Created($"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("todos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context // Buscando no banco para fazer a atualizacao
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
                return NotFound();
            try
            {
                todo.Title = model.Title;

                context.Todos.Update(todo);
                await context.SaveChangesAsync();

                return Ok(todo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}