using CsvHelper;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using System.Globalization;

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

        using (var reader = new StringReader(csvString))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var allRows = new List<List<string>>();
            while (csv.Read())
            {
                var row = new List<string>();
                for (int i = 0; csv.TryGetField(i, out string? field); i++)
                {
                    row.Add(field ?? string.Empty);
                }
                allRows.Add(row);
            }

            if (allRows.Count > 0)
            {
                var startRow = asSa ? 0 : 1;
                for (int i = startRow; i < allRows.Count; i++)
                {
                    result.CsvResponse.Add(allRows[i]);
                }
            }
        }

        return result;
    }
}
