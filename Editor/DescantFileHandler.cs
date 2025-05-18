#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Descant.Editor
{
    /// <summary>
    /// Static class for managing the creation and editing of new Descant files
    /// </summary>
    public static class DescantFileHandler
    {
        /// <summary>
        /// Gets a ScriptableObject from the instanceID of a file
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>The graph object (null if the selection is not of the desired type)</returns>
        static T GetAssetFromInstanceID<T>(int instanceID) where T : ScriptableObject
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long _);
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            if (asset.GetType() == typeof(DescantGraph)) return (T) asset;
            return null;
        }
        
        /// <summary>
        /// Checks to see whether the file with the supplied instanceID is a DescantGraph
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>Whether the file is a DescantGraph</returns>
        static bool IsDescantGraphFile(int instanceID)
        {
            return GetAssetFromInstanceID<DescantGraph>(instanceID) != null;
        }
        
        /// <summary>
        /// Project view contextual menu edit option for Descant Graph files
        /// </summary>
        [MenuItem("Assets/Edit Descant Graph")]
        static void EditGraphFile()
        {
            EditorWindow.GetWindow<DescantEditor>("Descant Graph Editor").Load(
                GetAssetFromInstanceID<DescantGraph>(Selection.activeObject.GetInstanceID()));
        }
 
        /// <summary>
        /// Method to confirm that the edit option only shows up for Descant Graph files
        /// </summary>
        /// <returns>Whether the selected file is a Descant Graph file</returns>
        [MenuItem("Assets/Edit Descant Graph", true)]
        static bool ConfirmEditGraphFile() => IsDescantGraphFile(Selection.activeObject.GetInstanceID());
    }
}
#endif