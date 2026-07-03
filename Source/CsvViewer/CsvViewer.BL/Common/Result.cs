namespace CsvViewer.BL.Common;

public record Result<T>(T? Value, bool IsSuccess, string Message);
