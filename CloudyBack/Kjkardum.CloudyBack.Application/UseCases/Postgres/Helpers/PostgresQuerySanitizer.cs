using System.Text;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Helpers;

public static class PostgresQuerySanitizer
{
    public static string StripCommentsAndWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var output = new StringBuilder();
        int length = input.Length;
        bool inSingleQuote = false;
        bool inDoubleQuote = false;
        bool inLineComment = false;
        bool inBlockComment = false;

        for (int i = 0; i < length; i++)
        {
            char c = input[i];
            char next = i + 1 < length ? input[i + 1] : '\0';

            // Handle block comment start
            if (!inSingleQuote && !inDoubleQuote && !inLineComment && !inBlockComment && c == '/' && next == '*')
            {
                inBlockComment = true;
                i++; // Skip '*'
                continue;
            }

            // Handle block comment end
            if (inBlockComment && c == '*' && next == '/')
            {
                inBlockComment = false;
                i++; // Skip '/'
                continue;
            }

            // Handle line comment start
            if (!inSingleQuote && !inDoubleQuote && !inLineComment && !inBlockComment && c == '-' && next == '-')
            {
                inLineComment = true;
                i++; // Skip second '-'
                continue;
            }

            // Handle line comment end
            if (inLineComment && (c == '\n' || c == '\r'))
            {
                inLineComment = false;
            }

            if (inBlockComment || inLineComment)
            {
                continue;
            }

            // Handle string literal (single quote)
            if (!inDoubleQuote && c == '\'')
            {
                output.Append(c);
                inSingleQuote = !inSingleQuote || (i > 0 && input[i - 1] == '\\');
                continue;
            }

            // Handle quoted identifier (double quote)
            if (!inSingleQuote && c == '"')
            {
                output.Append(c);
                inDoubleQuote = !inDoubleQuote || (i > 0 && input[i - 1] == '\\');
                continue;
            }

            output.Append(c);
        }

        // Normalize whitespace (excluding quoted strings)
        var result = new StringBuilder();
        bool insideString = false;
        bool insideIdentifier = false;
        bool lastWasSpace = false;

        foreach (char ch in output.ToString())
        {
            if (ch == '\'' && !insideIdentifier)
            {
                insideString = !insideString;
                result.Append(ch);
                lastWasSpace = false;
            }
            else if (ch == '"' && !insideString)
            {
                insideIdentifier = !insideIdentifier;
                result.Append(ch);
                lastWasSpace = false;
            }
            else if (!insideString && !insideIdentifier && char.IsWhiteSpace(ch))
            {
                if (!lastWasSpace)
                {
                    result.Append(' ');
                    lastWasSpace = true;
                }
            }
            else
            {
                result.Append(ch);
                lastWasSpace = false;
            }
        }

        return result.ToString().Trim();
    }

    public static bool ContainsRoleOrSessionSet(string query)
    {
        var cleaned = StripCommentsAndWhitespace(query).ToUpperInvariant();
        return cleaned.Contains("SET ROLE") || cleaned.Contains("SET SESSION AUTHORIZATION");
    }
}
