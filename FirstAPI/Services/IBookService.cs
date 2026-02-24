using FirstAPI.DTO;
using FirstAPI.Models;

namespace FirstAPI.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> AddBookAsync(CreateBookDto book);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto book);
        Task<bool> DeleteBookAsync(int id);
    }
}
