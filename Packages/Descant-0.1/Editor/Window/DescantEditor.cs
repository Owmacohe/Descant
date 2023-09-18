using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Window
{
    public class DescantEditor : EditorWindow
    {
        Button save;
        Toggle autoSave;
        
        [MenuItem("Window/Descant/Descant Editor")]
        public static void Open()
        {
            GetWindow<DescantEditor>("Descant Editor");
        }

        void CreateGUI()
        {
            AddGraphView();
            AddToolbar();
            
            AddStyleSheet();
        }

        void OnGUI()
        {
            if (autoSave.value) save.visible = false;
            else save.visible = true;
        }

        void AddGraphView()
        {
            DescantGraphView graphView = new DescantGraphView();
            
            rootVisualElement.Add(graphView);
            graphView.StretchToParentSize();
        }

        void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            TextElement fileName = new TextElement();
            fileName.AddToClassList("toolbar-title");
            fileName.text = "DescantDialogue.desc";
            toolbar.Add(fileName);

            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);

            save = new Button();
            save.text = "Save";
            saveSection.Add(save);

            autoSave = new Toggle();
            autoSave.label = "Autosave:";
            saveSection.Add(autoSave);
        }

        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}
