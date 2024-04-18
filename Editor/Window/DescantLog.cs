#if UNITY_EDITOR
using System.Linq;
using Descant.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// Simple editor window to display logs from Descant events
    /// </summary>
    public class DescantLog : EditorWindow
    {
        DescantLogData log; // The log file where the Descant Graph log events are stored
        VisualElement logList; // The editor's log event list parent VisualElement
        
        [MenuItem("Window/Descant/Log"), MenuItem("Descant/Log")]
        public static void Open()
        {
            GetWindow<DescantLog>("Descant Log");
        }

        void CreateGUI()
        {
            log = Resources.Load<DescantLogData>("Default Log (DO NOT DELETE)"); // Accessing the log file
            
            // Initializing the window
            DescantEditorUtilities.AddStyleSheet(rootVisualElement.styleSheets, "DescantLogStyleSheet");
            AddElements();

            // When the game stops being played, we re-generate the log list
            Application.quitting += () =>
            {
                Clear();
                Generate();
            };
        }

        /// <summary>
        /// Method for loading in the Descant Log UI
        /// </summary>
        void AddElements()
        {
            // Initializing the toolbar
            Toolbar toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            TextElement title = new TextElement();
            title.AddToClassList("toolbar-title");
            title.text = "Descant Log";
            toolbar.Add(title);

            // Adding in a section on the side for the log options
            VisualElement section = new VisualElement();
            section.AddToClassList("log-section");
            toolbar.Add(section);

            // Initializing the capture toggle
            Toggle capture = new Toggle("Capture log:");
            capture.value = log.Capture;
            section.Add(capture);

            capture.RegisterValueChangedCallback(evt =>
            {
                log.Capture = evt.newValue;
                DescantUtilities.SaveSerializedObject(log);
            });

            // Initializing the clear button
            Button clear = new Button();
            clear.text = "Clear log";
            section.Add(clear);

            clear.clicked += () =>
            {
                log.Clear();
                Clear();
            };

            // Initializing the log list parent
            logList = new VisualElement();
            logList.AddToClassList("log");
            rootVisualElement.Add(logList);

            if (!Application.isPlaying) Generate();
        }

        /// <summary>
        /// Quick method to load all the log events from teh file into a visually appealing format
        /// </summary>
        void Generate()
        {
            bool a = true;

            foreach (var i in log.Events)
            {
                string logEvent = "[" + i.Time + "] <b>" +
                                  i.Type + "</b>" +
                                  (i.Subtype.Equals("") ? "" : " (<i>type:</i> " + i.Subtype + ")") +
                                  (i.Name.Equals("") ? "" : " (<i>name:</i> " + i.Name + ")");
                
                // Initializing the log event line
                TextElement temp = new TextElement();
                temp.text = logEvent;
                temp.AddToClassList("log-element-" + (a ? "a" : "b"));
                logList.Add(temp);

                a = !a; // Toggling between light and dark lines (so it's easier to read)
            }
        }

        /// <summary>
        /// Quick method to clear all the log event VisualElements
        /// (doesn't clear the actual file, use DescantLogData.Clear() to do so)
        /// </summary>
        void Clear()
        {
            while (logList.childCount > 0)
                DescantEditorUtilities.RemoveElement(logList.Children().ToArray()[0]);
        }
    }
}
#endif