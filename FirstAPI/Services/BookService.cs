
using FirstAPI.Data;
using FirstAPI.DTO;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Services
{
    public class BookService : IBookService
    {
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
