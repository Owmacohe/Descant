using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Window
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
    }
}