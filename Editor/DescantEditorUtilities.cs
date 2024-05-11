using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    public static class DescantEditorUtilities
    {
        public const string VERSION = "1.2.0";
        
        #if UNITY_EDITOR
        
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
                if (i.GetType() == typeof(T) || i.GetType().IsSubclassOf(typeof(T))) return (T)i; // Returning the current element if it matches
                
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
                if (i.GetType() == typeof(T) || i.GetType().IsSubclassOf(typeof(T))) lst.Add((T)i);
                
                // Checking all the children's children
                // (then adding the results to the main list)
                List<T> temp = FindAllElements<T>(i);
                foreach (var j in temp) lst.Add(j);
            }

            return lst;
        }
        
        /// <summary>
        /// Recursive method to find the first VisualElements of some class in the hierarchy of another
        /// </summary>
        /// <param name="element">The parent VisualElement being checked through</param>
        /// <param name="className">The name of the class being looked for</param>
        /// <returns>The first VisualElement of class 'className' that is found</returns>
        public static VisualElement FindFirstElement(VisualElement element, string className)
        {
            // Searching through each child and its children before moving to the next
            foreach (var i in element.Children())
            {
                if (i.ClassListContains(className)) return i; // Returning the current element if it matches
                
                // Checking all the children's children
                // (only returning if they aren't null as so not to return null pre-emptively)
                VisualElement temp = FindFirstElement(i, className);
                if (temp != null) return temp;
            }

            return null;
        }

        /// <summary>
        /// Recursive method to find all the VisualElements of some class in the hierarchy of another
        /// </summary>
        /// <param name="element">The parent VisualElement being checked through</param>
        /// <param name="className">The name of the class being looked for</param>
        /// <returns>All the VisualElements of class 'className' that are found</returns>
        public static List<VisualElement> FindAllElements(VisualElement element, string className)
        {
            List<VisualElement> lst = new List<VisualElement>();
            
            // Searching through each child and its children before moving to the next
            foreach (var i in element.Children())
            {
                // Adding the current element to the list if it matches
                if (i.ClassListContains(className)) lst.Add(i);
                
                // Checking all the children's children
                // (then adding the results to the main list)
                List<VisualElement> temp = FindAllElements(i, className);
                foreach (var j in temp) lst.Add(j);
            }

            return lst;
        }

        #endregion

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
        
        /// <summary>
        /// Adds a stylesheet to an editor
        /// </summary>
        public static void AddStyleSheet(VisualElementStyleSheetSet styleSheetSet, string name)
        {
            // All the possible paths that a stylesheet could be located at
            string[] paths =
            {
                "Packages/com.owmacohe.descant/Resources/",
                "Packages/Descant-" + VERSION + "/Resources/",
                
                "Assets/com.owmacohe.descant/Resources/",
                "Assets/Descant-" + VERSION + "/Resources/",
                
                "Assets/com.owmacohe.descant/Resources/",
                "Assets/Packages/Descant-" + VERSION + "/Resources/"
            };

            StyleSheet styleSheet = null;

            // Trying all the possible paths and stopping when we find it
            foreach (var i in paths)
            {
                styleSheet = (StyleSheet)EditorGUIUtility.Load(i + name + ".uss");
                if (styleSheet != null) break;
            }
            
            if (styleSheet != null) styleSheetSet.Add(styleSheet);
        }
        
        #endif
    }
}