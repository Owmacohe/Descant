using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public static class DescantUtilities
    {
        public static string FilterText(string text)
        {
            string special = "/\\`~!@#$%^*()+={}[]|;:'\",.<>?";
            
            text = text.Trim();

            for (int i = 0; i < text.Length; i++)
                if (special.Contains(text[i]))
                    text = text.Remove(i);

            return text;
        }

        public static void RemoveElement(VisualElement element)
        {
            if (element != null && element.parent != null) element.parent.Remove(element);
        }

        public static T FindFirstElement<T>(VisualElement element) where T : VisualElement
        {
            foreach (var i in element.Children())
            {
                if (i.GetType() == typeof(T)) return (T)i;
                
                T temp = FindFirstElement<T>(i);
                if (temp != null) return temp;
            }

            return null;
        }
        
        public static List<T> FindAllElements<T>(VisualElement element) where T : VisualElement
        {
            List<T> lst = new List<T>();
            
            foreach (var i in element.Children())
            {
                if (i.GetType() == typeof(T)) lst.Add((T)i);
                
                List<T> temp = FindAllElements<T>(i);
                foreach (var j in temp) lst.Add(j);
            }

            return lst;
        }
        
        public static string FormatJSON(string json)
        {
            string temp = "";
            string currentIndent = "";

            foreach (char i in json)
            {
                // Indenting back after data members or objects end
                if (i is '}' or ']')
                {
                    currentIndent = currentIndent.Substring(0, currentIndent.Length - 1);
                    temp += '\n' + currentIndent;
                }

                temp += i;
 
                // Adding a space after colons
                if (i is ':')
                    temp += ' ';

                // New lines after commas
                if (i is ',')
                    temp += '\n' + currentIndent;

                // Indenting when data members or objects begin
                if (i is '{' or '[')
                {
                    currentIndent += "\t";
                    temp += '\n' + currentIndent;
                }
            }

            return temp;
        }

        public static string GetFileNameFromPath(string fullPath)
        {
            string temp = "";
            
            for (int i = 0; i < fullPath.Length; i++)
            {
                if (i > 4)
                {
                    char c = fullPath[fullPath.Length - 1 - i];
                    
                    if (c == '/') break;
                    
                    temp = c + temp;
                }
            }

            return temp;
        }

        public static bool AreListsEqual<T>(List<T> a, List<T> b)
        {
            if (a.Count != b.Count) return false;
            
            for (int i = 0; i < a.Count; i++)
                if (a[i].Equals(b[i]))
                    return false;
            
            return true;
        }

        public static string GetCurrentDirectory(bool withFinalSlash = true)
        {
            string pathAfterAssets = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            
            if (pathAfterAssets.Contains("."))
                pathAfterAssets = pathAfterAssets.Remove(pathAfterAssets.LastIndexOf('/'));

            pathAfterAssets = RemoveAssetsFolderFromPath(pathAfterAssets);

            return Application.dataPath + "/" + pathAfterAssets + (withFinalSlash ? "/" : "");
        }

        public static string RemoveAssetsFolderFromPath(string path)
        {
            string temp = path.Substring(6);

            if (temp[0] == '/')
                temp = temp.Substring(1);

            return temp;
        }
        
        public static string GetPathFromInstanceID(int instanceID)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long _);
            return AssetDatabase.GUIDToAssetPath(guid);
        }
    }
}