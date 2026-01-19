using Microsoft.EntityFrameworkCore;
using FirstAPI.Models;
namespace FirstAPI.Data
{
    public class FirstAPIContext : DbContext // DbContext class for Entity Framework Core
    {
        public FirstAPIContext(DbContextOptions<FirstAPIContext> options) : base(options) { } // constructor that takes DbContextOptions and passes them to the base class constructor

        protected override void OnModelCreating(ModelBuilder modelBuilder)   // Method to configure the model and seed initial data into the database
        {
            base.OnModelCreating(modelBuilder);     
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    YearPublished = 1925
                },
                  new Book
                  {
                      Id = 2,
                      Title = "To Kill a Mockingbird",
                      Author = "Harper Lee",
                      YearPublished = 1960
                  },
                   new Book
                   {
                       Id = 3,
                       Title = "1984",
                       Author = "George Orwell",
                       YearPublished = 1949
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
                       Title = "Mobi-Dick",
                       Author = "Herman Melville",
                       YearPublished = 1851
                   }
                );
        }
        public DbSet<FirstAPI.Models.Book> Books { get; set; } = null!; // DbSet allows us to access this Books table in the database and perform different operations to our data
    }
}
