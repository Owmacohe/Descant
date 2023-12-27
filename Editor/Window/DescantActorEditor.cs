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
        VisualElement UI; // The root UI element for the whole editor
        Toolbar toolbar; // The toolbar at the top of teh editor
        VisualElement actor; // The root element for all the actor VisualElements

        VisualElement statistics; // The parent of the statistics dictionary
        VisualElement topics; // The parent of the topics list
        VisualElement relationships; // The parent of the relationships dictionary
        TextField dialogueAttempts; // The dialogue attempts field
        
        DescantActor data; // The actor currently being displayed
        
        // The full disc path of the last loaded Descant actor
        // (so that it can be re-loaded when the editor is re-loaded (e.g. when there is a script change))
        string lastPath;
        bool GUICreated; // Whether a Descant actor is currently loaded into the editor
        
        [MenuItem("Window/Descant/Descant Actor Editor"), MenuItem("Tools/Descant/Descant Actor Editor")]
        public static void Open()
        {
            GetWindow<DescantActorEditor>("Descant Actor Editor");
        }
        
        #region GUI

        void CreateGUI()
        {
            // If the actor data has already been loaded into the editor, we simple generate the actor UI and toolbar
            // (they're dependant on the data having been previously loaded)
            if (GUICreated)
            {
                UI = new VisualElement();
                UI.AddToClassList("ui");
                rootVisualElement.Add(UI);
                UI.StretchToParentSize();

                AddToolbar();
                AddActor();
                
                AddStyleSheet();
            }
            // Otherwise, we first load the data before adding the actor UI view and toolbar
            // (the Load method will call CreateGUI again when it has finished)
            else
            {
                Load(lastPath);
                AssetDatabase.Refresh();
            }

            // Resetting the loaded variable to indicate that the UI should be
            // reloaded if CreateGUI is ever called again when Unity is refreshed
            // (otherwise the actor UI and toolbar would be added without any data to initialize them with)
            GUICreated = false;
        }
        
        /// <summary>
        /// Removes the actor UI and toolbar from the hierarchy
        /// </summary>
        void RemoveGUI()
        {
            DescantEditorUtilities.RemoveElement(UI);
        }
        
        /// <summary>
        /// Reloads the GUI (presumably after the DescantActorData object has been changed)
        /// </summary>
        void ReloadGUI()
        {
            RemoveGUI();
            CreateGUI();
        }
        
        /// <summary>
        /// Adds the stylesheet to the editor
        /// (the DescantGraphView needs to also have the style sheet set)
        /// </summary>
        void AddStyleSheet()
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.owmacohe.descant/Assets/DescantActorEditorStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Initializes the toolbar's VisualElements
        /// </summary>
        void AddToolbar()
        {
            toolbar = new Toolbar();
            UI.Add(toolbar);
            
            // Initializing the title section
            VisualElement toolbarTitle = new VisualElement();
            toolbarTitle.AddToClassList("toolbar-title");
            toolbar.Add(toolbarTitle);

            // Initializing the Descant actor's file name
            TextElement fileName = new TextElement();
            fileName.text = data.Name;
            fileName.AddToClassList("toolbar-filename");
            toolbarTitle.Add(fileName);
            
            // Initializing the save section
            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);

            // Initializing the save button
            Button save = new Button();
            save.clicked += GetAndSave;
            save.text = "Save";
            saveSection.Add(save);
            
            // Initializing the close button
            Button close = new Button();
            close.clicked += Unload;
            close.text = "Close";
            saveSection.Add(close);
        }

        /// <summary>
        /// Initializes the DescantActor UI
        /// </summary>
        void AddActor()
        {
            actor = new VisualElement();
            actor.AddToClassList("actor");
            UI.Add(actor);

            // Initializing the sections
            
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
        
        #endregion
        
        #region Actor Lists
        
        /// <summary>
        /// Creates a new GUI section for the actor that represents a list of items
        /// </summary>
        /// <param name="lst">The actual list that will be represented</param>
        /// <param name="name">The name of the list</param>
        /// <returns>A new VisualElement list of the items</returns>
        VisualElement AddActorList<T>(List<T> lst, string name)
        {
            // Initializing the list parent
            VisualElement list = new VisualElement();
            list.AddToClassList("actor_list");
            actor.Add(list);

            // Initializing the list name
            TextElement listName = new TextElement();
            listName.text = name;
            listName.AddToClassList("list_name");
            list.Add(listName);
            
            // Initializing the button to add new items
            Button add = new Button();
            add.text = "Add new " + name.ToLower().Substring(0, name.Length - 1);
            add.clicked += () => AddListItem(list, "");
            add.AddToClassList("add_button");
            list.Add(add);

            // Adding all the items in
            foreach (var i in lst)
                AddListItem(list, i);

            return list;
        }

        /// <summary>
        /// Generates a single list item (for use with AddActorList)
        /// </summary>
        /// <param name="list">The parent list that this element will be inserted into</param>
        /// <param name="v">The value to insert into the item</param>
        void AddListItem<T>(VisualElement list, T v)
        {
            // Initializing the item parent
            VisualElement item = new VisualElement();
            item.AddToClassList("dictionary_item");
            list.Insert(list.childCount - 1, item);
            
            // Initializing the item remove button
            Button remove = new Button();
            remove.text = "X";
            remove.clicked += () => DescantEditorUtilities.RemoveElement(item);
            item.Add(remove);
            
            // Initializing the item value field
            TextField value = new TextField();
            value.value = v.ToString();
            item.Add(value);

            value.RegisterValueChangedCallback(callback =>
            {
                value.value = DescantUtilities.FilterText(callback.newValue);
            });
        }
        
        #endregion
        
        #region Actor Dictionaries
        
        /// <summary>
        /// Creates a new GUI section for the actor that represents a dictionary
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="name">The name of the dictionary</param>
        /// <returns>A new VisualElement dictionary</returns>
        VisualElement AddActorDictionary<T, U>(List<T> keys, List<U> values, string name)
        {
            // Initializing the dictionary parent
            VisualElement dictionary = new VisualElement();
            dictionary.AddToClassList("actor_list");
            actor.Add(dictionary);

            // Initializing the dictionary name
            TextElement dictionaryName = new TextElement();
            dictionaryName.text = name;
            dictionaryName.AddToClassList("list_name");
            dictionary.Add(dictionaryName);
            
            // Initializing the button to add new entries
            Button add = new Button();
            add.text = "Add new " + name.ToLower().Substring(0, name.Length - 1);
            add.clicked += () => AddDictionaryItem(dictionary, "", 0);
            add.AddToClassList("add_button");
            dictionary.Add(add);

            // Adding all the entries in
            for (int i = 0; i < keys.Count; i++)
                AddDictionaryItem(dictionary, keys[i], values[i]);

            return dictionary;
        }
        
        /// <summary>
        /// Generates a single dictionary entry (for use with AddActorDictionary)
        /// </summary>
        /// <param name="dictionary">The parent dictionary that this element will be inserted into</param>
        /// <param name="k">The key to insert into the entry</param>
        /// <param name="v">The value to insert into the entry</param>
        void AddDictionaryItem<T, U>(VisualElement dictionary, T k, U v)
        {
            VisualElement item = new VisualElement();
            item.AddToClassList("dictionary_item");
            dictionary.Insert(dictionary.childCount - 1, item);
            
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
        
        #endregion
        
        #region Saving And Loading
        
        /// <summary>
        /// Checks through the entire actor UI for relevant information and saves it to the file
        /// </summary>
        void GetAndSave()
        {
            // Resetting the actor's statistics lists
            data.StatisticKeys = new List<string>();
            data.StatisticValues = new List<float>();
            
            // Getting the statistics dictionary entries
            var statisticsChildren = statistics.Children().ToList();

            // Getting the values from the children and adding them to the lists
            for (int i = 1; i < statisticsChildren.Count - 1; i++)
            {
                var temp = statisticsChildren[i].Children().ToList();
                
                data.StatisticKeys.Add(((TextField)temp[1]).value);
                data.StatisticValues.Add(float.Parse(((TextField)temp[2]).value));
            }
            
            // Resetting the actor's topics list
            data.Topics = new List<string>();
            
            // Getting the topics list items
            var topicsChildren = topics.Children().ToList();

            // Getting the values from the children and adding them to the list
            for (int i = 1; i < topicsChildren.Count - 1; i++)
                data.Topics.Add(((TextField)topicsChildren[i].Children().ToList()[1]).value);
            
            // Resetting the actor's relationships lists
            data.RelationshipKeys = new List<string>();
            data.RelationshipValues = new List<float>();
            
            // Getting the relationships dictionary entries
            var relationshipsChildren = relationships.Children().ToList();

            // Getting the values from the children and adding them to the lists
            for (int i = 1; i < relationshipsChildren.Count - 1; i++)
            {
                var temp = relationshipsChildren[i].Children().ToList();
                
                data.RelationshipKeys.Add(((TextField)temp[1]).value);
                data.RelationshipValues.Add(float.Parse(((TextField)temp[2]).value));
            }
            
            // Getting the values from the dialogue attempts field and applying it to the value
            data.DialogueAttempts = int.Parse(dialogueAttempts.value);
            
            // (the actual saving method must be in a separate, non-Editor class
            // so that both Editor and non-Editor classes can access it)
            DescantEditorUtilities.SaveActor(false, data);
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads the data from a Descant actor file
        /// </summary>
        /// <param name="fullPath">The full disc path to the Descant file to load</param>
        public void Load(string fullPath)
        {
            // Making sure the path isn't null or empty
            if (fullPath != null && fullPath.Trim() != "")
            {
                lastPath = fullPath;

                data = DescantEditorUtilities.LoadActorFromPath(fullPath);
                
                // Reloading the name and path, in case they got changed after the last time this file was loaded
                data.Name = DescantUtilities.FilterText(DescantEditorUtilities.GetDescantFileNameFromPath(fullPath, false));
                data.Path = DescantEditorUtilities.RemoveBeforeLocalPath(fullPath);
                
                DescantEditorUtilities.SaveActor(false, data);

                GUICreated = true;

                // Removing the old GUI (if it exists) and
                // creating the new GUI (now that the data has been loaded)
                ReloadGUI();
            }
        }

        /// <summary>
        /// Un-loads the current data and GUI
        /// </summary>
        void Unload()
        {
            data = null;
            
            GUICreated = false;
            lastPath = null;
            
            RemoveGUI();
        }
        
        /// <summary>
        /// Creates a new blank file, and reloads the GUI
        /// </summary>
        public void NewFile()
        {
            data = new DescantActor("New_Descant_Actor");
            DescantEditorUtilities.SaveActor(true, data);

            GUICreated = true;
            
            ReloadGUI();
        }
        
        #endregion
    }
}
#endif