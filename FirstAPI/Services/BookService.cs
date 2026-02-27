
using FirstAPI.Data;
using FirstAPI.DTO;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Services
{
    public class BookService : IBookService
    {
        /*making list static ---> because this way the list will be created once when 
        the controller is first instantiated and then on each upcoming HTTP request we
        will use the same list. So basically if we add sth to our list or modify sth 
        delete sth, these changes will be saved for all the upcoming API requests.*/

        /*static private List<Book> books = new List<Book>    // if we remove static keyword this list would be created each time we made a new http request and we would lose every modification.
        
        {
            new Book
            {
                Id=1,
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                YearPublished = 1925
            },
            new Book
            {
                Id = 2,
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                YearPublished= 1960
            },
            new Book
            {
                Id = 3,
                Title = "1984",
                Author = "George Orwell",
                YearPublished=1949
            },
            new Book
            {
                Id = 4,
                Title = "Pride and Prejudice",
                Author = "Jane Austen",
                YearPublished = 1813
            },
            new Book
            {
                Id = 5,
                Title="Mobi-Dick",
                Author="Herman Melville",
                YearPublished=1851
           }
        };*/
        private readonly FirstAPIContext _context;
        public BookService(FirstAPIContext context)
        {
            _context = context;
        }

        public async Task<BookDto> AddBookAsync(CreateBookDto book)
        {
            var newBook = new Book
            {
                Title = book.Title,
                Author = book.Author,
                YearPublished = book.YearPublished,
            };
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return new BookDto 
            {
                Id = newBook.Id,
                Title = newBook.Title,
                Author = newBook.Author,
                YearPublished = newBook.YearPublished
            };
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var bookToDelete = await _context.Books.FindAsync(id);
            if (bookToDelete == null)
                return false;
            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            return await _context.Books.Select(x=> new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author,
                YearPublished = x.YearPublished
            }).ToListAsync(); 
        }

        public async Task<PagedResultDto<BookDto>> GetAllBooksSPAsync(QueryObject query)
        {
            var books = await _context.Books
                .FromSqlRaw("EXEC Sp_GetBooks")
                .AsNoTracking()
                .ToListAsync();

            // ===== SORTING =====
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                books = query.SortBy.ToLower() switch
                {
                    "author" => query.IsDescending
                        ? books.OrderByDescending(b => b.Author).ToList()
                        : books.OrderBy(b => b.Author).ToList(),

                    "title" => query.IsDescending
                        ? books.OrderByDescending(b => b.Title).ToList()
                        : books.OrderBy(b => b.Title).ToList(),

                    "yearpublished" => query.IsDescending
                        ? books.OrderByDescending(b => b.YearPublished).ToList()
                        : books.OrderBy(b => b.YearPublished).ToList(),

                    _ => books
                };
            }

            var totalCount = books.Count;

            // ===== PAGINATION =====
            var pagedBooks = books
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            // ===== MAP ENTITY → DTO =====
            var dtoList = pagedBooks.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                YearPublished = b.YearPublished
            }).ToList();

            return new PagedResultDto<BookDto>
            {
                TotalCount = totalCount,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Items = dtoList
            };
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Where(x=>x.Id == id)
                .Select(c=> new BookDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Author = c.Author,
                    YearPublished = c.YearPublished
                })
                .FirstOrDefaultAsync();

        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto book)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return false;
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.YearPublished = book.YearPublished;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
