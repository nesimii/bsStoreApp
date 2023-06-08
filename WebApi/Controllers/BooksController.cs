using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.EFCore;

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
            try
            {

                var books = _context.Books.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var book = _context
                .Books
                .Where(b => b.Id.Equals(id))
                .SingleOrDefault();
                if (book is null) return NotFound(); //404

                return Ok(book);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                {
                    return BadRequest(); //400;
                }
                _context.Books.Add(book);
                _context.SaveChanges();
                return StatusCode(201, book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute] int id, [FromBody] Book book)
        {
            try
            { //check db has a book??
                var dbBook = _context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (dbBook is null) return NotFound();  //404
                if (id != book.Id) return BadRequest(); //400

                dbBook.Title = book.Title;
                dbBook.Price = book.Price;

                _context.SaveChanges();
                return Ok(book);    //200 }catch (Exception ex) { }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var dbBook = _context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();
                if (dbBook is null) return BadRequest(
                    new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not be found",
                    });
                _context.Books.Remove(dbBook);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public IActionResult partiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var dbBook = _context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();
            if (dbBook is null)
            {
                return BadRequest(
                    new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not be found",
                    }
                );
            }
            bookPatch.ApplyTo(dbBook);
            _context.SaveChanges();
            return NoContent(); // 204
        }
    }
}
