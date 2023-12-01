#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DescantEditor
{
    /// <summary>
    /// Static class for managing the creation and editing of new Descant files
    /// </summary>
    public static class DescantFileHandler
    {
        /// <summary>
        /// Checks to see whether the file with the supplied instanceID has the file type .desc.json
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>Whether the file is a Descant file</returns>
        static bool IsDescantGraphFile(int instanceID)
        {
            string path = DescantEditorUtilities.GetFullPathFromInstanceID(instanceID);
            return path.Substring(path.Length - 10) == ".desc.json";
        }
        
        static bool IsDescantActorFile(int instanceID)
        {
            string path = DescantEditorUtilities.GetFullPathFromInstanceID(instanceID);
            return path.Substring(path.Length - 15) == ".descactor.json";
        }
        
        /// <summary>
        /// Project view contextual menu edit option for Descant files
        /// </summary>
        [MenuItem("Assets/Edit Descant Graph")]
        static void EditGraphFile() {
            EditorWindow.GetWindow<DescantEditor>("Descant Graph Editor").Load(
                DescantEditorUtilities.GetFullPathFromInstanceID(
                    Selection.activeObject.GetInstanceID()));
        }
 
        /// <summary>
        /// Method to confirm that the edit option only shows up for Descant files
        /// </summary>
        /// <returns>Whether the selected file is a Descant files</returns>
        [MenuItem("Assets/Edit Descant Graph", true)]
        static bool ConfirmEditGraphFile() {
            return IsDescantGraphFile(Selection.activeObject.GetInstanceID());
        }
        
        [MenuItem("Assets/Edit Descant Actor")]
        static void EditActorFile() {
            EditorWindow.GetWindow<DescantActorEditor>("Descant Actor Editor").Load(
                DescantEditorUtilities.GetFullPathFromInstanceID(
                    Selection.activeObject.GetInstanceID()));
        }
        
        [MenuItem("Assets/Edit Descant Actor", true)]
        static bool ConfirmEditActorFile() {
            return IsDescantActorFile(Selection.activeObject.GetInstanceID());
        }

        /// <summary>
        /// Project view contextual menu create option for Descant files
        /// </summary>
        [MenuItem("Assets/Create/Descant Graph")]
        static void CreateNewGraphFile()
        {
            // First checking to see if a file with the default name already exists
            try
            {
                File.ReadAllText(
                    Application.dataPath + "/" +
                    DescantEditorUtilities.GetCurrentLocalDirectory() +
                    "New_Descant_Graph.desc.json"
                );
            }
            // If it doesn't, we create a new one
            catch
            {
                EditorWindow.GetWindow<DescantEditor>("Descant Graph Editor").NewFile();
                AssetDatabase.Refresh(); // Refreshing the AssetDatabase so the new file shows up immediately
            }
        }
        
        [MenuItem("Assets/Create/Descant Actor")]
        static void CreateNewActorFile()
        {
            // First checking to see if a file with the default name already exists
            try
            {
                File.ReadAllText(
                    Application.dataPath + "/" +
                    DescantEditorUtilities.GetCurrentLocalDirectory() +
                    "New_Descant_Actor.descactor.json"
                );
            }
            // If it doesn't, we create a new one
            catch
            {
                EditorWindow.GetWindow<DescantActorEditor>("Descant Actor Editor").NewFile();
                AssetDatabase.Refresh(); // Refreshing the AssetDatabase so the new file shows up immediately
            }
        }
    }
}
#endif