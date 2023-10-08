﻿using System.IO;
using Editor.Window;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Static class for managing the creation and editing of new Descant files
    /// </summary>
    public static class DescantFileHandler
    {
        /// <summary>
        /// Checks to see whether the file with the supplied instanceID has the file type .desc
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>Whether the file is a Descant file</returns>
        static bool IsDescantFile(int instanceID)
        {
            string path = DescantUtilities.GetFullPathFromInstanceID(instanceID);
            return path.Substring(path.Length - 5) == ".desc";
        }
        
        /// <summary>
        /// *[CURRENTLY NOT WORKING]*
        /// Opens a Descant file when it is double-clicked upon
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        [OnOpenAsset]
        public static bool OpenFile(int instanceID)
        {
            string path = DescantUtilities.GetFullPathFromInstanceID(instanceID);
            bool isDescantFile = IsDescantFile(instanceID);
            
            if (isDescantFile)
            {
                Debug.Log("<b>Descant</b> | Loading file: " + path);
                EditorWindow.GetWindow<DescantEditor>().Load(path);
            }

            return false;
        }
        
        /// <summary>
        /// Project view contextual menu edit option for Descant files
        /// </summary>
        [MenuItem("Assets/Edit Descant Graph")]
        static void EditFile() {
            EditorWindow.GetWindow<DescantEditor>().Load(
                DescantUtilities.GetFullPathFromInstanceID(
                    Selection.activeObject.GetInstanceID()));
        }
 
        /// <summary>
        /// Method to confirm that the edit option only shows up for Descant files
        /// </summary>
        /// <returns>Whether the selected file is a Descant files</returns>
        [MenuItem("Assets/Edit Descant Graph", true)]
        static bool ConfirmEditFile() {
            return IsDescantFile(Selection.activeObject.GetInstanceID());
        }

        /// <summary>
        /// Project view contextual menu create option for Descant files
        /// </summary>
        [MenuItem("Assets/Create/Descant Graph")]
        static void CreateNewFile()
        {
            // First checking to see if a file with the default name already exists
            try
            {
                File.ReadAllText(
                    Application.dataPath + "/" +
                    DescantUtilities.GetCurrentLocalDirectory() +
                    "New Descant Graph.desc"
                );
            }
            // If it doesn't, we create a new one
            catch
            {
                EditorWindow.GetWindow<DescantEditor>().NewFile();
                AssetDatabase.Refresh(); // Refreshing the AssetDatabase so the new file shows up immediately
            }
        }
    }
}