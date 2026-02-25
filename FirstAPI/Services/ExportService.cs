using ClosedXML.Excel;
using FirstAPI.DTO;
using FirstAPI.Services;
using System.IO;

public class ExportService : IExportService
{
    private readonly IBookService _bookService;

    public ExportService(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var books = await _bookService.GetAllBooksAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("BookRecords");

        // ===== HEADER =====
        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Author";
        worksheet.Cell(1, 4).Value = "YearPublished";

        var headerRange = worksheet.Range("A1:D1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F75B5"); // Blue
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // ===== DATA =====
        int row = 2;
        foreach (var book in books)
        {
            worksheet.Cell(row, 1).Value = book.Id;
            worksheet.Cell(row, 2).Value = book.Title;
            worksheet.Cell(row, 3).Value = book.Author;
            worksheet.Cell(row, 4).Value = book.YearPublished;

            var dataRange = worksheet.Range($"A{row}:D{row}");

            // Alternating light blue rows
            if (row % 2 == 0)
            {
                dataRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public Task<byte[]> ExportToPdfAsync()
    {
        throw new NotImplementedException();
    }
}