
using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;
using FirstAPI.Services;
using FirstAPI.DTO;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // we are injecting FirstAPIContext into our controller through its constructor
        private readonly IBookService _service;
        private readonly IExportService _exportService;
        public BooksController(IBookService service, IExportService exportService)
        {
            _service = service;
            _exportService = exportService;
        }



        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetBooks()
        {
            return Ok(await _service.GetAllBooksAsync());
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ActionResult<PagedResultDto<BookDto>>> GetAllBooksSP([FromQuery] QueryObject query)
        {
            var result = await _service.GetAllBooksSPAsync(query);
            return Ok(result);
        }



        [HttpGet("ExportExcel")]
        public async Task<ActionResult> ExportBooksToExcel()
        {
            // Implementation for exporting books to Excel
            // This is a placeholder for the actual implementation
            var fileBytes = await _exportService.ExportToExcelAsync();
            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BookRecords.xlsx"
                );
        }

        [HttpGet("ExportToPDF")]
        public async Task<ActionResult<List<Book>>> ExportBooksToPDF()
        {
            var pdf = await _exportService.ExportToPdfAsync();
            return File(pdf, "application/pdf", "Books.pdf");
        }

        // this http get method again for when user is requesting one specific resources
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var book = await _service.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound("Book with the given id was not found"); //404 not found
            }
            return Ok(book); //200 ok
        }

        /*[HttpGet("SP{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _service.Books.FromSql($"Sp_GetBooksById {id}").ToListAsync();
            if (book == null || book.Count ==0)
            {
                return NotFound(); //404 not found
            }
            return Ok(book); //200 ok
        }*/

        [HttpPost]
        public async Task<ActionResult<BookDto>> AddBook(CreateBookDto newBook)
        {
            var createdBook = await _service.AddBookAsync(newBook);

            return CreatedAtAction(nameof(GetBookById), new { createdBook.Id}, createdBook); // CreatedAtAction returns 201 created
        }

        /*[HttpPost("insert")]
        public async Task<ActionResult<Book>> AddBook(Book newBook)
        {
            if (newBook == null)
                return BadRequest(); //400 bad request

            _context.Books.Add(newBook);
            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_InsertBooks @Title={newBook.Title}, @Author = {newBook.Author}, @YearPublished = {newBook.YearPublished}");

            return CreatedAtAction(nameof(GetBookById), new { newBook.Id }, newBook); //201 created
        }*/

        [HttpPut("{id}")]    // when an api call is made to update a resource, we need to provide the id of the resource we want to update 
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updatedBook)
        {
            var book = await _service.UpdateBookAsync(id, updatedBook);
            if (!book)
                return NotFound("Book with the given id was not found");
            return Ok(book);
            
        }


        /*[HttpPut("SP{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_UpdateBooks @Id={id},@Title={updatedBook.Title}, @Author={updatedBook.Author}, @YearPublished={updatedBook.YearPublished}");
            return NoContent(); //204 no content
        }*/

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _service.DeleteBookAsync(id);
            if (!book)
                return NotFound("Book with the given is was not found"); //404 not found
            return NoContent(); //204 no content
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            _context.Books.Remove(book);

            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_DeleteBook @Id={id}");
            return NoContent(); //204 no content
        }*/
    }
}
