namespace FirstAPI.DTO
{
    public class UpdateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = null!;
        public int YearPublished { get; set; }
    }
}
