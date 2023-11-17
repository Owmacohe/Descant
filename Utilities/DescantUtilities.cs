/// <summary>
/// Enum to list all of the types of DescantNodes
/// </summary>
public enum DescantNodeType { Choice, Response, Start, End, Any }

public static class DescantUtilities
{
    /// <summary>
    /// Formats a single-line JSON string into something that is much more human-readable
    /// </summary>
    /// <param name="json">The JSON text to be formatted</param>
    /// <returns>The formatted JSON, with indents and line breaks</returns>
    public static string FormatJSON(string json)
    {
        string temp = ""; // The string to copy sections of the original text into
        string currentIndent = ""; // The current string of indents (will shrink and grow throughout the process)
        bool isInQuotation = false;

        foreach (char i in json)
        {
            if (i is '\"') isInQuotation = !isInQuotation;
                
            // Indenting back after data members or objects end
            if (i is '}' or ']' && !isInQuotation)
            {
                currentIndent = currentIndent.Substring(0, currentIndent.Length - 1);
                temp += '\n' + currentIndent;
            }

            temp += i;
 
            // Adding a space after colons
            if (i is ':' && !isInQuotation)
                temp += ' ';

            // New lines after commas
            if (i is ',' && !isInQuotation)
                temp += '\n' + currentIndent;

            // Indenting when data members or objects begin
            if (i is '{' or '[' && !isInQuotation)
            {
                currentIndent += "\t";
                temp += '\n' + currentIndent;
            }
        }

        return temp;
    }
}