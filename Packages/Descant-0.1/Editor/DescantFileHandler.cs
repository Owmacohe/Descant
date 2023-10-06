using System.IO;
using Editor.Data;
using Editor.Window;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public static class DescantFileHandler
    {
        static bool IsDescantFile(int instanceID)
        {
            string path = DescantUtilities.GetPathFromInstanceID(instanceID);
            return path.Substring(path.Length - 5) == ".desc";
        }
        
        [OnOpenAsset]
        public static bool OpenFile(int instanceID)
        {
            string path = DescantUtilities.GetPathFromInstanceID(instanceID);
            bool isDescantFile = IsDescantFile(instanceID);
            
            if (isDescantFile)
            {
                Debug.Log("<b>Descant</b> | Loading file: " + path);
                EditorWindow.GetWindow<DescantEditor>().Load(path);
            }

            return false;
        }
        
        [MenuItem("Assets/Edit Descant Graph")]
        static void EditFile() {
            EditorWindow.GetWindow<DescantEditor>().Load(
                DescantUtilities.GetPathFromInstanceID(
                    Selection.activeObject.GetInstanceID()));
        }
 
        [MenuItem("Assets/Edit Descant Graph", true)]
        static bool ConfirmEditFile() {
            return IsDescantFile(Selection.activeObject.GetInstanceID());
        }

        [MenuItem("Assets/Create/Descant Graph")]
        static void CreateNewFile()
        {
            try
            {
                File.ReadAllText(DescantUtilities.GetCurrentDirectory() + "New Descant Graph.desc");
            }
            catch
            {
                DescantEditor temp = EditorWindow.GetWindow<DescantEditor>();
                temp.Load(null, true);
                AssetDatabase.Refresh();
            }
        }
    }
}