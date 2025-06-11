using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Helpers;

public static class PostgresCsvConverter
{
    public static PostgresQueryResultDto ToQueryDto(string csvString, bool asSa)
    {
        var result = new PostgresQueryResultDto { CsvResponse = new List<List<string>>() };
        if (string.IsNullOrEmpty(csvString))
        {
            return result;
        }

        var entries = csvString
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim().Split(',').ToList())
            .ToList();
        if (entries.Count == 0)
        {
            return result;
        }
        result.CsvResponse.AddRange(entries.Slice(asSa ? 0 : 1, entries.Count - 1));
        return result;
    }
}
