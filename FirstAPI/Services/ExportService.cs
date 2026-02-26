using ClosedXML.Excel;
using FirstAPI.DTO;
using FirstAPI.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

    public async Task<byte[]> ExportToPdfAsync()
    {
        var books = await _bookService.GetAllBooksAsync();

        QuestPDF.Settings.License = LicenseType.Community;

        var document = CreateDocument(books);

        return document.GeneratePdf();
    }
    private static IDocument CreateDocument(List<BookDto> books)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x
                    .FontSize(14)
                    .FontFamily("Times New Roman")
                    .FontColor(Colors.Grey.Darken2));

                // Main Title
                page.Header()
                    .Text("Books Record")
                    .Bold()
                    .FontSize(36)
                    .FontColor(Colors.Blue.Darken2)
                    .AlignCenter();

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            HeaderCellStyle(header.Cell()).Text("ID");
                            HeaderCellStyle(header.Cell()).Text("Title");
                            HeaderCellStyle(header.Cell()).Text("Author");
                            HeaderCellStyle(header.Cell()).Text("Year Published");
                        });

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

    private static IContainer HeaderCellStyle(IContainer cell)
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

    private static IContainer CellStyleRow(IContainer container, int index)
    {
        var backgroundColor = index % 2 == 0
            ? Colors.Grey.Lighten4
            : Colors.White;

        return container
            .Background(backgroundColor)
            .PaddingVertical(8)
            .PaddingHorizontal(16)
            .DefaultTextStyle(x => x
                .FontSize(14)
                .FontColor(Colors.Grey.Darken2));
    }


}