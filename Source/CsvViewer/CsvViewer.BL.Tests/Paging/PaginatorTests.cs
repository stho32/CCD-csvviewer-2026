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
        CsvDocument document = CreateDocument("eins", "zwei", "drei", "vier", "fuenf");
        int originalRowCount = document.Rows.RowCount;

        // Act
        Result<IReadOnlyList<CsvDocument>> result = Paginator.Paginate(document, 2);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value, Has.Count.EqualTo(3));
        Assert.That(result.Value![0].Rows.RowCount, Is.EqualTo(2));
        Assert.That(result.Value[1].Rows.RowCount, Is.EqualTo(2));
        Assert.That(result.Value[2].Rows.RowCount, Is.EqualTo(1));
        Assert.That(result.Value[0].Rows[0][0], Is.EqualTo("eins"));
        Assert.That(result.Value[1].Rows[0][0], Is.EqualTo("drei"));
        Assert.That(result.Value[2].Rows[0][0], Is.EqualTo("fuenf"));
        Assert.That(document.Rows.RowCount, Is.EqualTo(originalRowCount));
    }

    [Test]
    public void Wenn_DokumentKeineDatenzeilenHat_dann_EineLeereSeiteEntsteht()
    {
        // Arrange
        CsvDocument document = CreateDocument();

        // Act
        Result<IReadOnlyList<CsvDocument>> result = Paginator.Paginate(document, 10);

        // Assert
        Assert.That(result.IsSuccess, Is.True, result.Message);
        Assert.That(result.Value, Has.Count.EqualTo(1));
        Assert.That(result.Value![0].Header, Is.SameAs(document.Header));
        Assert.That(result.Value[0].Rows.RowCount, Is.Zero);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Wenn_SeitengroesseNichtPositivIst_dann_KeineSeitenEntstehen(int pageSize)
    {
        // Arrange
        CsvDocument document = CreateDocument("eins");
        int originalRowCount = document.Rows.RowCount;

        // Act
        Result<IReadOnlyList<CsvDocument>> result =
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
        Result<IReadOnlyList<CsvDocument>> result = Paginator.Paginate(document, 10);

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
