using CsvViewer.BL.Common;
using CsvViewer.BL.Csv;
using CsvViewer.BL.Paging;

namespace CsvViewer.BL.Tests.Paging;

public class PaginatorTests
{
    [Test]
    public void Wenn_FuenfZeilenMitSeitengroesseZweiPaginiertWerden_dann_DreiSeitenEntstehen()
    {
        // Arrange
        CsvDocument document = CreateDocument("eins", "zwei", "drei", "vier", "fünf");
        int originalRowCount = document.Rows.RowCount;

        // Act
        Result<PagedDocument> result = Paginator.Paginate(document, 2);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value!.Header, Is.SameAs(document.Header));
        Assert.That(result.Value.Pages.PageCount, Is.EqualTo(3));
        Assert.That(result.Value.Pages[0].RowCount, Is.EqualTo(2));
        Assert.That(result.Value.Pages[1].RowCount, Is.EqualTo(2));
        Assert.That(result.Value.Pages[2].RowCount, Is.EqualTo(1));
        Assert.That(result.Value.Pages[0][0][0], Is.EqualTo("eins"));
        Assert.That(result.Value.Pages[1][0][0], Is.EqualTo("drei"));
        Assert.That(result.Value.Pages[2][0][0], Is.EqualTo("fünf"));
        Assert.That(document.Rows.RowCount, Is.EqualTo(originalRowCount));
    }

    [Test]
    public void Wenn_DokumentKeineDatenzeilenHat_dann_EineLeereSeiteEntsteht()
    {
        // Arrange
        CsvDocument document = CreateDocument();

        // Act
        Result<PagedDocument> result = Paginator.Paginate(document, 10);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value!.Header, Is.SameAs(document.Header));
        Assert.That(result.Value.Pages.PageCount, Is.EqualTo(1));
        Assert.That(result.Value.Pages[0].RowCount, Is.Zero);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Wenn_SeitengroesseNichtPositivIst_dann_KeineSeitenEntstehen(int pageSize)
    {
        // Arrange
        CsvDocument document = CreateDocument("eins");
        int originalRowCount = document.Rows.RowCount;

        // Act
        Result<PagedDocument> result =
            Paginator.Paginate(document, pageSize);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(document.Rows.RowCount, Is.EqualTo(originalRowCount));
    }

    [Test]
    public void Wenn_DokumentFehlt_dann_KeineSeitenEntstehen()
    {
        // Arrange
        CsvDocument? document = null;

        // Act
        Result<PagedDocument> result = Paginator.Paginate(document, 10);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Message, Does.Contain("fehlt"));
    }

    private static CsvDocument CreateDocument(params string[] values)
    {
        return new CsvDocument(
            new CsvHeader(["Wert"]),
            new CsvRowCollection(values.Select(value => new CsvRow([value]))));
    }
}
