namespace MTerminal.WPF.Autocomplete;

internal static class CommandAutocomplete
{
    /// <summary>
    /// Gets the first string that matches input string, or starts with input string.
    /// </summary>
    /// <returns>Matched string or empty string if nothing matched.</returns>
    internal static string Match(string input, IEnumerable<string> collection)
    {
        if (string.IsNullOrWhiteSpace(input) || collection is null || !collection.Any())
            return string.Empty;

        return collection.FirstOrDefault(c => c.StartsWith(input.Trim()), string.Empty);
    }

    /// <summary>
    /// Gets the first alphabetically ordered string that matches input string, or starts with input string.
    /// </summary>
    /// <returns>Matched string or empty string if nothing matched.</returns>
    internal static string MatchOrdered(string input, IEnumerable<string> collection)
    {
        if (string.IsNullOrWhiteSpace(input) || collection is null || !collection.Any())
            return string.Empty;

        return collection.OrderBy(c => c).FirstOrDefault(c => c.StartsWith(input), string.Empty);
    }

    /// <summary>
    /// Gets the next alphabetically ordered command that matches input string, or starts with input string.
    /// </summary>
    /// <returns>Matched string or empty string if nothing matched.</returns>
    internal static string GetNextOrdered(string input, IEnumerable<string> collection)
    {
        if (string.IsNullOrWhiteSpace(input) || collection is null || !collection.Any())
            return string.Empty;

        return collection.OrderBy(c => c).FirstOrDefault(c => string.Compare(c, input) > 0, string.Empty);
    }

    internal static string GetPreviousOrdered(string input, IEnumerable<string> collection)
    {
        if (string.IsNullOrWhiteSpace(input) || collection is null || !collection.Any())
            return string.Empty;

        return collection.OrderByDescending(c => c).FirstOrDefault(c => string.Compare(c, input) < 0, string.Empty);
    }

    /// <summary>
    /// Matches all string from a collection that start with input string.
    /// </summary>
    /// <returns>Collection of matched strings or empty collection if none matched.</returns>
    internal static IEnumerable<string> MatchAll(string input, IEnumerable<string> collection)
    {
        if (string.IsNullOrWhiteSpace(input) || collection is null || !collection.Any())
            return Array.Empty<string>();

        return collection.Where(c => c.StartsWith(input.Trim())).OrderBy(c => c);
    }
}