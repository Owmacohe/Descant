using Editor.Window;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public static class DescantFileHandler
    {
        [OnOpenAsset(1)]
        public static bool OpenFile(int instanceID)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long _);
            string path = AssetDatabase.GUIDToAssetPath(guid);
            bool isDescantFile = path.Substring(path.Length - 5) == ".desc";
            
            if (isDescantFile)
            {
                Debug.Log("<b>Descant</b> | Loading file: " + path);
                EditorWindow.GetWindow<DescantEditor>().Load(path);
            }

            return false;
        }
    }
}