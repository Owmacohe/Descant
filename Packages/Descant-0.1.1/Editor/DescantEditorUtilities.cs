#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DescantEditor
{
    public static class DescantEditorUtilities
    {
        #region VisualElements
        
        /// <summary>
        /// Method to remove the provided element from the hierarchy
        /// (provided it has a parent to be removed from)
        /// </summary>
        /// <param name="element">The VisualElement to be removed</param>
        public static void RemoveElement(VisualElement element)
        {
            if (element != null && element.parent != null) element.parent.Remove(element);
        }

        /// <summary>
        /// Recursive method to find the first VisualElement of some type in the hierarchy of another
        /// </summary>
        /// <param name="element">The parent VisualElement being checked through</param>
        /// <typeparam name="T">The VisualElement type being looked for</typeparam>
        /// <returns>The first VisualElement of type T that is found (null if not found)</returns>
        public static T FindFirstElement<T>(VisualElement element) where T : VisualElement
        {
            // Searching through each child and its children before moving to the next
            foreach (var i in element.Children())
            {
                if (i.GetType() == typeof(T)) return (T)i; // Returning the current element if it matches
                
                // Checking all the children's children
                // (only returning if they aren't null as so not to return null pre-emptively)
                T temp = FindFirstElement<T>(i);
                if (temp != null) return temp;
            }

            return null;
        }
        
        /// <summary>
        /// Recursive method to find all the VisualElements of some type in the hierarchy of another
        /// </summary>
        /// <param name="element">The parent VisualElement being checked through</param>
        /// <typeparam name="T">The VisualElement type being looked for</typeparam>
        /// <returns>All the VisualElements of type T that are found</returns>
        public static List<T> FindAllElements<T>(VisualElement element) where T : VisualElement
        {
            List<T> lst = new List<T>();
            
            // Searching through each child and its children before moving to the next
            foreach (var i in element.Children())
            {
                // Adding the current element to the list if it matches
                if (i.GetType() == typeof(T)) lst.Add((T)i);
                
                // Checking all the children's children
                // (then adding the results to the main list)
                List<T> temp = FindAllElements<T>(i);
                foreach (var j in temp) lst.Add(j);
            }

            return lst;
        }
        
        #endregion
        
        #region Text Maniplation
        
        /// <summary>
        /// Filters a string to remove all special characters (and whitespace)
        /// </summary>
        /// <param name="text">The text to be checked through</param>
        /// <returns>The filtered string with special characters removed</returns>
        public static string FilterText(string text)
        {
            string special = "/\\`~!@#$%^*()+={}[]|;:'\",.<>?";
            
            text = text.Trim();

            for (int i = 0; i < text.Length; i++)
                if (special.Contains(text[i]) || text[i] == ' ')
                    text = text.Remove(i);

            return text;
        }
        
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
        
        #endregion

        #region File Paths
        
        /// <summary>
        /// Extracts the file name of a Descant file from its path
        /// </summary>
        /// <param name="path">The full or local path ending in [filename].desc.json</param>
        /// <returns>The name of the Descant file, without its folder structure or .desc.json extension</returns>
        public static string GetDescantFileNameFromPath(string path)
        {
            string temp = "";
            
            for (int i = 0; i < path.Length; i++)
            {
                // Making sure we've passed the .desc.json at the end
                if (i > 9)
                {
                    char c = path[path.Length - 1 - i]; // Getting the characters from the end
                    
                    if (c == '/') break; // Stopping when we hit the first folder
                    
                    temp = c + temp;
                }
            }

            return temp;
        }
        
        /// <summary>
        /// Gets the currently-focused directory (minus the full path up to the Assets folder)
        /// </summary>
        /// <param name="withFinalSlash">Whether to include one final '/' after the path</param>
        /// <returns>The string of the local directory, after the Assets folder</returns>
        public static string GetCurrentLocalDirectory(bool withFinalSlash = true)
        {
            // Getting the full path to the directory
            string temp = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            
            // If this contains a file, we remove up to the last folder
            if (temp.Contains("."))
                temp = temp.Remove(temp.LastIndexOf('/'));

            // Removing the Assets folder from the start
            temp = RemoveAssetsFolderFromPath(temp, true, true);
            
            return temp + (withFinalSlash ? "/" : "");
        }

        /// <summary>
        /// Removes the Assets folder from a path, ether from the beginning or the end
        /// </summary>
        /// <param name="path">The path being modified</param>
        /// <param name="fromStart">Whether to remove from the start or the end of the path</param>
        /// <param name="removeStartSlash">Whether to remove the starting '/' after the Assets folder</param>
        /// <returns>The path with the Assets folder removed from it</returns>
        public static string RemoveAssetsFolderFromPath(string path, bool fromStart, bool removeStartSlash)
        {
            string temp = fromStart ? path.Substring(6) : path.Substring(0, path.Length - 6);

            if (removeStartSlash && temp.Length > 0 && temp[0] == '/')
                temp = temp.Substring(1);
            
            if (temp.Length > 0 && temp[^1] == '/')
                temp = temp.Substring(0, temp.Length - 1);

            return temp;
        }

        /// <summary>
        /// Isolates a local path from a full one
        /// </summary>
        /// <param name="fullPath">A full disc path</param>
        /// <returns>The local path, isolated from the full one</returns>
        public static string RemoveBeforeLocalPath(string fullPath)
        {
            string temp = Application.dataPath;
            return fullPath.Substring(temp.Length);
        }
        
        /// <summary>
        /// Gets a full path from the insstanceID of a file
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>The full disc path to the file</returns>
        public static string GetFullPathFromInstanceID(int instanceID)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long _);
            
            return RemoveAssetsFolderFromPath(Application.dataPath, false, false) + "/" +
                   AssetDatabase.GUIDToAssetPath(guid);
        }
        
        #endregion

        #region Misc

        /// <summary>
        /// Checks to see if two lists of some type are equal
        /// </summary>
        /// <param name="a">The first list being checked</param>
        /// <param name="b">The second list being checked</param>
        /// <typeparam name="T">The type of both lists</typeparam>
        /// <returns>Whether the two lists are equal</returns>
        public static bool AreListsEqual<T>(List<T> a, List<T> b)
        {
            // Obviously, if they're not the same length, they're not equal
            if (a.Count != b.Count) return false;
            
            // Using each element's default or overridden Equals method to check for equality
            for (int i = 0; i < a.Count; i++)
                if (a[i].Equals(b[i]))
                    return false;
            
            return true;
        }

        #endregion
    }
}
#endif