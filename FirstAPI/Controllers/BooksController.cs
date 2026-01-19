
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FirstAPI.Data;
using FirstAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

using System.ComponentModel;
using System.Data;

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
        private readonly FirstAPIContext _context; 
        public BooksController(FirstAPIContext context)
        {
            _context = context;
        }



        /*[HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            return Ok(await _context.Books.ToListAsync());
        }*/

        [HttpGet]
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

        }



        [HttpGet("ExportExcel")]
        public async Task<ActionResult<List <Book>>> ExportBooksToExcel()
        {
            // Implementation for exporting books to Excel
            // This is a placeholder for the actual implementation
            var books = await _context.Books.ToListAsync();
            var _bookdata = ConvertToDataTable(books);
            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(_bookdata);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.AddWorksheet(_bookdata, "Book Records");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BookRecords.xlsx");
                    }
                }
            }
        }


        [NonAction]
        private DataTable ConvertToDataTable(List<Book> books)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Books";
            // Define columns
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Author", typeof(string));
            dt.Columns.Add("YearPublished", typeof(int));
            // Populate rows
            foreach (var book in books)
            {
                dt.Rows.Add(
                    book.Id,
                    book.Title,
                    book.Author,
                    book.YearPublished
                    );
            }
            return dt;
        }

        [HttpGet("ExportToPDF")]
        public async Task<ActionResult<List<Book>>> ExportBooksToPDF()
        {
            var books = await _context.Books.ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;
            var document = CreateDocument(books);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", "Books.pdf");
        }

        [NonAction]
        private static IDocument CreateDocument(List<Book> books)
        {
            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    // 🔹 Page setup
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x
                        .FontSize(14)
                        .FontFamily("Times New Roman")
                        .FontColor(Colors.Grey.Darken2));

                    // 🟦 Main title
                    page.Header()
                        .Text("Books Record")
                        .Bold()
                        .FontSize(36)
                        .FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    // 🧾 Table section
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);  // ID
                                columns.RelativeColumn(3);   // Title
                                columns.RelativeColumn(3);   // Author
                                columns.RelativeColumn(2);   // YearPublished
                            });

                            // 📘 Header Row (Styled)
                            table.Header(header =>
                            {
                                HeaderCellStyle(header.Cell()).Text("ID");
                                HeaderCellStyle(header.Cell()).Text("Title");
                                HeaderCellStyle(header.Cell()).Text("Author");
                                HeaderCellStyle(header.Cell()).Text("Year Published");
                            });

                            // 📄 Body Rows (Alternating Colors)
                            for (int i = 0; i < books.Count; i++)
                            {
                                var book = books[i];
                                CellStyleRow(table.Cell(), i).Text(book.Id.ToString());
                                CellStyleRow(table.Cell(), i).Text(book.Title);
                                CellStyleRow(table.Cell(), i).Text(book.Author);
                                CellStyleRow(table.Cell(), i).Text(book.YearPublished.ToString());
                            }
                        });
                });
            });
        }

        [NonAction]
        // 🎨 Header Cell Style — Slightly larger + bold + darker gray text
        private static QuestPDF.Infrastructure.IContainer HeaderCellStyle(QuestPDF.Infrastructure.IContainer cell)
        {
            return cell
                .BorderBottom(1)
                .PaddingVertical(8)
                .PaddingHorizontal(8)
                .DefaultTextStyle(x => x
                    .FontColor(Colors.Grey.Darken4)
                    .FontSize(18)
                    .SemiBold())
                .AlignCenter();
        }

        [NonAction]
        // 🎨 Body Row Style — Alternating background + smaller font
        private static QuestPDF.Infrastructure.IContainer CellStyleRow(QuestPDF.Infrastructure.IContainer container, int index)
        {
            var backgroundColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

            return container
                .Background(backgroundColor)
                .PaddingVertical(8)
                .PaddingHorizontal(16)
                .DefaultTextStyle(x => x
                    .FontSize(14)
                    .FontColor(Colors.Grey.Darken2));
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
        /*[HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(); //404 not found
            }
            return Ok(book); //200 ok
        }*/

        [HttpGet("SP{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books.FromSql($"Sp_GetBooksById {id}").ToListAsync();
            if (book == null || book.Count ==0)
            {
                return NotFound(); //404 not found
            }
            return Ok(book); //200 ok
        }

        /*[HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book newBook)
        {
            if (newBook == null)
                return BadRequest(); //400 bad request

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { newBook.Id}, newBook); // CreatedAtAction returns 201 created
        }*/

        [HttpPost("insert")]
        public async Task<ActionResult<Book>> AddBook(Book newBook)
        {
            if (newBook == null)
                return BadRequest(); //400 bad request

            _context.Books.Add(newBook);
            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_InsertBooks @Title={newBook.Title}, @Author = {newBook.Author}, @YearPublished = {newBook.YearPublished}");

            return CreatedAtAction(nameof(GetBookById), new { newBook.Id }, newBook); //201 created
        }

        /*[HttpPut("{id}")]    // when an api call is made to update a resource, we need to provide the id of the resource we want to update 
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            book.Id = updatedBook.Id;
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.YearPublished = updatedBook.YearPublished;

            await _context.SaveChangesAsync();
            return NoContent(); //204 no content
        }*/

        /*[HttpPut("{id}")]    // when an api call is made to update a resource, we need to provide the id of the resource we want to update 
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            book.Id = updatedBook.Id;
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.YearPublished = updatedBook.YearPublished;

            await _context.SaveChangesAsync();
            return NoContent(); //204 no content
        }*/

        [HttpPut("SP{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_UpdateBooks @Id={id},@Title={updatedBook.Title}, @Author={updatedBook.Author}, @YearPublished={updatedBook.YearPublished}");
            return NoContent(); //204 no content
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();
            return NoContent(); //204 no content
        }*/

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(); //404 not found

            _context.Books.Remove(book);

            await _context.Database.ExecuteSqlInterpolatedAsync($"exec Sp_DeleteBook @Id={id}");
            return NoContent(); //204 no content
        }
    }
}
