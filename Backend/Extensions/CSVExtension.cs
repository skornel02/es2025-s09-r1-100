namespace Backend.Extensions;

public static class CSVExtension
{
    public static async Task<List<List<string>>> ParseCsvAsync(this IFormFile file, string separator = ",")
    {
        using var contentStream = file.OpenReadStream();
        using var reader = new StreamReader(contentStream);

        var result = new List<List<string>>();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                continue;
            }

            var columns = line.Split(separator, StringSplitOptions.TrimEntries);
            result.Add([.. columns]);
        }

        return result;
    }
}
