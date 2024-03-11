using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Enum to list all of the types of DescantNodes
/// </summary>
public enum DescantNodeType { Choice, Response, Start, End, Any }

public static class DescantUtilities
{
    #region Text Manipulation

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
        LogMessage(source, message, "#f08080");
    }

    /// <summary>
    /// Prints a message to the console, formatted with a bold source name
    /// </summary>
    /// <param name="source">The type of the script this is being called from</param>
    /// <param name="message">The message to display</param>
    /// <param name="colour">The colour that the message should print in</param>
    public static void LogMessage(Type source, string message, string colour = "white")
    {
        Debug.Log("<color='" + colour + "'><b>" + source.Name + ":</b> " + message + "</color>");
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Method to make sure that SerializedObjects are properly saved
    /// </summary>
    /// <param name="obj">The object to save</param>
    public static void SaveSerializedObject(Object obj)
    {
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endif
}