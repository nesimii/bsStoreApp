using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public BooksController(RepositoryContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);

        }

    }
}
