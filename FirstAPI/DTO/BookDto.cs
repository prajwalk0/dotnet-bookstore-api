namespace FirstAPI.DTO
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Author { get; set; } = string.Empty;
        public int YearPublished { get; set; }
    }
}
