using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {

                var books = _manager.Book.GetAllBooks(false);
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
                var book = _manager.Book.GetOneBookById(id, false);
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
                _manager.Book.CreateOneBook(book);
                _manager.Save();
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
                var dbBook = _manager.Book.GetOneBookById(id, true);

                if (dbBook is null) return NotFound();  //404
                if (id != book.Id) return BadRequest(); //400

                dbBook.Title = book.Title;
                dbBook.Price = book.Price;

                _manager.Save();
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
                var dbBook = _manager.Book.GetOneBookById(id, true);
                if (dbBook is null) return BadRequest(
                    new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not be found",
                    });
                _manager.Book.DeleteOneBook(dbBook);
                _manager.Save();
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
            try
            {
                var dbBook = _manager.Book.GetOneBookById(id, true);
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
                _manager.Book.Update(dbBook);
                _manager.Save();
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
