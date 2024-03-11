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
        /// Checks to see whether the file with the supplied instanceID is a DescantGraph
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>Whether the file is a DescantGraph</returns>
        static bool IsDescantGraphFile(int instanceID)
        {
            return DescantEditorUtilities.GetObjectFromInstanceID(instanceID) != null;
        }
        
        /// <summary>
        /// Project view contextual menu edit option for Descant Graph files
        /// </summary>
        [MenuItem("Assets/Edit Descant Graph")]
        static void EditGraphFile() {
            EditorWindow.GetWindow<DescantEditor>("Descant Graph Editor").Load(
                DescantEditorUtilities.GetObjectFromInstanceID(
                    Selection.activeObject.GetInstanceID()));
        }
 
        /// <summary>
        /// Method to confirm that the edit option only shows up for Descant Graph files
        /// </summary>
        /// <returns>Whether the selected file is a Descant Graph file</returns>
        [MenuItem("Assets/Edit Descant Graph", true)]
        static bool ConfirmEditGraphFile() {
            return IsDescantGraphFile(Selection.activeObject.GetInstanceID());
        }
    }
}
#endif