#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DescantComponents;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DescantEditor
{
    public class DescantActorEditor : EditorWindow
    {
        VisualElement UI;
        Toolbar toolbar;
        VisualElement actor;

        VisualElement statistics;
        VisualElement topics;
        VisualElement relationships;
        TextField dialogueAttempts;
        
        DescantActor data;

        bool loaded;
        string lastLoaded;
        
        [MenuItem("Window/Descant/Descant Actor Editor"), MenuItem("Tools/Descant/Descant Actor Editor")]
        public static void Open()
        {
            GetWindow<DescantActorEditor>("Descant Actor Editor");
        }

        void CreateGUI()
        {
            if (loaded)
            {
                UI = new VisualElement();
                UI.AddToClassList("ui");
                rootVisualElement.Add(UI);
                UI.StretchToParentSize();

                AddToolbar();
                AddActor();
                
                AddStyleSheet();
            }
            else
            {
                Load(lastLoaded);
                AssetDatabase.Refresh();
            }

            loaded = false;
        }
        
        void RemoveGUI()
        {
            DescantEditorUtilities.RemoveElement(UI);
        }
        
        void ReloadGUI()
        {
            RemoveGUI();
            CreateGUI();
        }
        
        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantActorEditorStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        void AddToolbar()
        {
            toolbar = new Toolbar();
            UI.Add(toolbar);
                
            VisualElement toolbarTitle = new VisualElement();
            toolbarTitle.AddToClassList("toolbar-title");
            toolbar.Add(toolbarTitle);

            TextElement fileName = new TextElement();
            fileName.text = data.Name;
            fileName.AddToClassList("toolbar-filename");
            toolbarTitle.Add(fileName);
                
            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);

            Button save = new Button();
            save.clicked += ReadAndSave;
            save.text = "Save";
            saveSection.Add(save);
            
            Button close = new Button();
            close.clicked += Unload;
            close.text = "Close";
            saveSection.Add(close);
        }

        void AddActor()
        {
            actor = new VisualElement();
            actor.AddToClassList("actor");
            UI.Add(actor);

            statistics = AddActorDictionary(data.StatisticKeys, data.StatisticValues, "Statistics");
            topics = AddActorList(data.Topics, "Topics");
            relationships = AddActorDictionary(data.RelationshipKeys, data.RelationshipValues, "Relationships");

            dialogueAttempts = new TextField();
            dialogueAttempts.label = "Dialogue attempts: ";
            dialogueAttempts.value = data.DialogueAttempts.ToString();
            dialogueAttempts.AddToClassList("actor_list");
            actor.Add(dialogueAttempts);

            dialogueAttempts.RegisterValueChangedCallback(callback =>
            {
                dialogueAttempts.value = DescantUtilities.FilterText(callback.newValue);
            });
        }

        void AddListItem<T>(VisualElement list, T v)
        {
            VisualElement item = new VisualElement();
            item.AddToClassList("dictionary_item");
            list.Insert(list.childCount - 1, item);
            
            Button remove = new Button();
            remove.text = "X";
            remove.clicked += () => DescantEditorUtilities.RemoveElement(item);
            item.Add(remove);
                
            TextField value = new TextField();
            value.value = v.ToString();
            item.Add(value);

            value.RegisterValueChangedCallback(callback =>
            {
                value.value = DescantUtilities.FilterText(callback.newValue);
            });
        }

        VisualElement AddActorList<T>(List<T> lst, string name)
        {
            VisualElement list = new VisualElement();
            list.AddToClassList("actor_list");
            actor.Add(list);

            TextElement listName = new TextElement();
            listName.text = name;
            listName.AddToClassList("list_name");
            list.Add(listName);
            
            Button add = new Button();
            add.text = "Add new " + name.ToLower().Substring(0, name.Length - 1);
            add.clicked += () => AddListItem(list, "");
            add.AddToClassList("add_button");
            list.Add(add);

            foreach (var i in lst)
                AddListItem(list, i);

            return list;
        }
        
        void AddDictionaryItem<T, U>(VisualElement list, T k, U v)
        {
            VisualElement item = new VisualElement();
            item.AddToClassList("dictionary_item");
            list.Insert(list.childCount - 1, item);
            
            Button remove = new Button();
            remove.text = "X";
            remove.clicked += () => DescantEditorUtilities.RemoveElement(item);
            item.Add(remove);
                
            TextField key = new TextField();
            key.value = k.ToString();
            item.Add(key);

            key.RegisterValueChangedCallback(callback =>
            {
                key.value = DescantUtilities.FilterText(callback.newValue);
            });
                
            TextField value = new TextField();
            value.value = v.ToString();
            item.Add(value);

            value.RegisterValueChangedCallback(callback =>
            {
                value.value = DescantUtilities.FilterText(callback.newValue, true);
            });
        }
        
        VisualElement AddActorDictionary<T, U>(List<T> keys, List<U> values, string name)
        {
            VisualElement list = new VisualElement();
            list.AddToClassList("actor_list");
            actor.Add(list);

            TextElement listName = new TextElement();
            listName.text = name;
            listName.AddToClassList("list_name");
            list.Add(listName);
            
            Button add = new Button();
            add.text = "Add new " + name.ToLower().Substring(0, name.Length - 1);
            add.clicked += () => AddDictionaryItem(list, "", 0);
            add.AddToClassList("add_button");
            list.Add(add);

            for (int i = 0; i < keys.Count; i++)
                AddDictionaryItem(list, keys[i], values[i]);

            return list;
        }

        public void Load(string fullPath)
        {
            // Making sure the path isn't null or empty
            if (fullPath != null && fullPath.Trim() != "")
            {
                lastLoaded = fullPath;

                data = DescantEditorUtilities.LoadActorFromPath(fullPath);
                
                // Reloading the name and path, in case they got changed after the last time this file was loaded
                data.Name = DescantUtilities.FilterText(DescantEditorUtilities.GetDescantFileNameFromPath(fullPath, false));
                data.Path = DescantEditorUtilities.RemoveBeforeLocalPath(fullPath);
                
                DescantEditorUtilities.SaveActor(false, data);

                loaded = true;

                // Removing the old GUI (if it exists) and
                // creating the new GUI (now that the data has been loaded)
                ReloadGUI();
            }
        }

        void ReadAndSave()
        {
            var statisticsChildren = statistics.Children().ToList();
            data.StatisticKeys = new List<string>();
            data.StatisticValues = new List<float>();

            for (int i = 1; i < statisticsChildren.Count - 1; i++)
            {
                var temp = statisticsChildren[i].Children().ToList();
                
                data.StatisticKeys.Add(((TextField)temp[1]).value);
                data.StatisticValues.Add(float.Parse(((TextField)temp[2]).value));
            }
            
            var topicsChildren = topics.Children().ToList();
            data.Topics = new List<string>();

            for (int i = 1; i < topicsChildren.Count - 1; i++)
                data.Topics.Add(((TextField)topicsChildren[i].Children().ToList()[1]).value);
            
            var relationshipsChildren = relationships.Children().ToList();
            data.RelationshipKeys = new List<string>();
            data.RelationshipValues = new List<float>();

            for (int i = 1; i < relationshipsChildren.Count - 1; i++)
            {
                var temp = relationshipsChildren[i].Children().ToList();
                
                data.RelationshipKeys.Add(((TextField)temp[1]).value);
                data.RelationshipValues.Add(float.Parse(((TextField)temp[2]).value));
            }

            data.DialogueAttempts = int.Parse(dialogueAttempts.value);
            
            DescantEditorUtilities.SaveActor(false, data);
            AssetDatabase.Refresh();
        }

        void Unload()
        {
            data = null;
            
            loaded = false;
            lastLoaded = null;
            
            RemoveGUI();
        }
        
        public void NewFile()
        {
            data = new DescantActor("New_Descant_Actor");
            DescantEditorUtilities.SaveActor(true, data);

            loaded = true;
            
            ReloadGUI();
        }
    }
}
#endif