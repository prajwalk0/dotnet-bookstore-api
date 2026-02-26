
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
        /*making list static ---> because this way the list will be created once when 
        the controller is first instantiated and then on each upcoming HTTP request we
        will use the same list. So basically if we add sth to our list or modify sth 
        delete sth, these changes will be saved for all the upcoming API requests.*/

        //static private List<Book> books = new List<Book>    // if we remove static keyword this list would be created each time we made a new http request and we would lose every modification.
        //{
        //    new Book
        //    {
        //        Id=1,
        //        Title = "The Great Gatsby",
        //        Author = "F. Scott Fitzgerald",
        //        YearPublished = 1925
        //    },
        //    new Book
        //    {
        //        Id = 2,
        //        Title = "To Kill a Mockingbird",
        //        Author = "Harper Lee",
        //        YearPublished= 1960
        //    },
        //    new Book
        //    {
        //        Id = 3,
        //        Title = "1984",
        //        Author = "George Orwell",
        //        YearPublished=1949
        //    },
        //    new Book
        //    {
        //        Id = 4,
        //        Title = "Pride and Prejudice",
        //        Author = "Jane Austen",
        //        YearPublished = 1813
        //    },
        //    new Book
        //    {
        //        Id = 5,
        //        Title="Mobi-Dick",
        //        Author="Herman Melville",
        //        YearPublished=1851
        //    }
        //};


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

        /*[HttpGet]
        [Route("getall")]
        public async Task<ActionResult<List<Book>>> GetAllBooksSP([FromQuery] QueryObject query)   // -----> here show this by calling database stored procedure
        {
            var books = await _context.Books.FromSqlRaw("Sp_GetBooks").ToListAsync();
            //books = books.OrderBy(b => b.YearPublished).ToList();
            if (query.SortBy != null)
            {
                if (query.SortBy.ToLower() == "author")
                {
                    books = query.IsDescending ? books.OrderByDescending(b => b.Author).ToList() : books.OrderBy(b => b.Author).ToList();
                }
                else if (query.SortBy.ToLower() == "title")
                {
                    books = query.IsDescending ? books.OrderByDescending(b => b.Title).ToList() : books.OrderBy(b => b.Title).ToList();
                }
                else if (query.SortBy.ToLower() == "yearpublished")
                {
                    books = query.IsDescending ? books.OrderByDescending(b => b.YearPublished).ToList() : books.OrderBy(b => b.YearPublished).ToList();
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            var pagedBooks = books.Skip(skipNumber).Take(query.PageSize).ToList();

            var pagedResponse = new
            {
                TotalCount = books.Count,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(books.Count / (double)query.PageSize),
                Books = pagedBooks
            };

            return Ok(pagedResponse);

        }*/
        


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




        /*[HttpGet("{page}")]
        public async Task<ActionResult<List<Book>>> GetBooks(int page)
        {
            if (_context.Books==null)
                return NotFound();

            var pageResults = 4f;  // we get 3 items on one page
            var pageCount = Math.Ceiling(_context.Books.Count() / pageResults); // we calculate how many pages we have
            var books=await _context.Books
                .Skip((page - 1) * (int)pageResults) // we skip the items of the prteevious pages
                .Take((int)pageResults)  // we take only the items of the current page
                .ToListAsync();
            books= books.OrderBy(b => b.Author).ToList(); // we order the books by author

            return Ok(books);
        }*/

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
