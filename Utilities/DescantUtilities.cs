using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum to list all of the types of DescantNodes
/// </summary>
public enum DescantNodeType { Choice, Response, Start, End, Any }

public static class DescantUtilities
{
    #region Text Manipulation

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

    /// <summary>
    /// Filters a string to remove all special characters (and whitespace)
    /// </summary>
    /// <param name="text">The text to be checked through</param>
    /// <param name="allowPeriod">Whether to allow the filtering to ignore periods</param>
    /// <returns>The filtered string with special characters removed</returns>
    public static string FilterText(string text, bool allowPeriod = false)
    {
        string special = "/\\`~!@#$%^*()+={}[]|;:'\",<>?"; // allowed: &_
            
        text = text.Trim();

        for (int i = 0; i < text.Length; i++)
            if (special.Contains(text[i]) || text[i] == ' ' || (!allowPeriod && text[i] == '.'))
                text = text.Remove(i);

        return text;
    }

    #endregion

    /// <summary>
    /// Prints an error message to the console, formatted in red with a bold source name
    /// </summary>
    /// <param name="source">The type of the script this is being called from</param>
    /// <param name="message">The error message to display</param>
    public static void ErrorMessage(Type source, string message)
    {
        Debug.Log("<color='#f08080'><b>" + source.Name + ":</b> " + message + "</color>");
    }

    #region Rounding

    /// <summary>
    /// Quickly rounds a float to the specified decimal length
    /// </summary>
    /// <param name="f">The float to be rounded</param>
    /// <param name="decimalPlaces">The number of decimal places to include</param>
    /// <returns>The rounded float</returns>
    public static float RoundToDecimal(float f, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);

        return Mathf.Round(f * factor) / factor;
    }

    /// <summary>
    /// Semi-recursive method to check through all floats in a class (and those in its member classes),
    /// and round them to the specified decimal length
    /// </summary>
    /// <param name="obj">The object with the floats to be rounded</param>
    /// <param name="decimalPlaces">The number of decimal places to include</param>
    /// <param name="checkedObjects">
    /// A list of previously-checked objects
    /// (to make sure we don't hit any infinite loops)
    /// </param>
    public static void RoundObjectToDecimal<T>(T obj, int decimalPlaces, List<object> checkedObjects = null) where T : class
    {
        if (obj == null) return;

        // Initializing the checkObjects if this is the first time
        if (checkedObjects == null)
            checkedObjects = new List<object>();
        else if (checkedObjects.Contains(obj)) return;
        
        checkedObjects.Add(obj); // Adding the object being checked to the checkedObjects
        
        // Checking each of the class's fields
        foreach (var i in obj.GetType().GetFields())
        {
            if (i.FieldType == typeof(List<float>))
            {
                var temp = (List<float>)i.GetValue(obj);

                for (int j = 0; j < temp.Count; j++)
                    temp[j] = RoundToDecimal(temp[j], decimalPlaces);
            }
            else if (i.FieldType == typeof(float))
            {
                i.SetValue(obj, RoundToDecimal((float) i.GetValue(obj), decimalPlaces));
            }
            else if (i.FieldType.IsClass)
            {
                RoundObjectToDecimal(i.GetValue(obj), decimalPlaces, checkedObjects);
            }
        }
    }

    #endregion
}