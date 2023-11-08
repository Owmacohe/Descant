#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DescantEditor
{
    /// <summary>
    /// The custom Unity EditorWindow for loading, saving, and manipulating Descant graphs
    /// </summary>
    public class DescantEditor : EditorWindow
    {
        /// <summary>
        /// The data object for the current Descant graph
        /// </summary>
        public DescantGraphData data;
        
        // TODO: component IDs
        
        /// <summary>
        /// The graph view part the editor
        /// </summary>
        DescantGraphView graphView;
        
        /// <summary>
        /// The toolbar part of the editor
        /// </summary>
        Toolbar toolbar;
        
        /// <summary>
        /// The autosave toggle button in the toolbar
        /// </summary>
        Toggle autoSave;
        
        /// <summary>
        /// The unsaved changes marker in the toolbar
        /// </summary>
        TextElement unsaved;

        /// <summary>
        /// Whether a Descant graph is currently loaded into the editor
        /// </summary>
        bool loaded;
        
        /// <summary>
        /// The full disc path of the last loaded Descant graph
        /// (so that it can be re-loaded when the editor is re-loaded (e.g. when there is a script change))
        /// </summary>
        string lastLoaded;
        
        // TODO: ctrl-S functionality
        
        [MenuItem("Window/Descant/Descant Editor"), MenuItem("Tools/Descant/Descant Editor")]
        public static void Open()
        {
            GetWindow<DescantEditor>("Descant Editor");
        }

        void CreateGUI()
        {
            // If the graph data has already been loaded into the editor, we simple generate the graph view and toolbar
            // (they're dependant on the data having been previously loaded)
            if (loaded)
            {
                AddGraphView();
                AddToolbar();
            
                AddStyleSheet();
            }
            // Otherwise, we first load the data before adding the graph view and toolbar
            // (the Load method will call CreateGUI again when it has finished)
            else
            {
                Load(lastLoaded);
            }

            // Resetting the loaded variable to indicate that the UI should be
            // reloaded if CreateGUI is ever called again naturally
            // (otherwise the graph view and toolbar would be added without any data to initialize them with)
            loaded = false;
        }

        /// <summary>
        /// Removes the graph view and toolbar from the hierarchy
        /// </summary>
        void RemoveGUI()
        {
            DescantEditorUtilities.RemoveElement(graphView);
            DescantEditorUtilities.RemoveElement(toolbar);
        }

        /// <summary>
        /// Reloads the GUI (presumably after the DescantGraphData object has been changed)
        /// </summary>
        void ReloadGUI()
        {
            RemoveGUI();
            CreateGUI();
        }

        /// <summary>
        /// Initializes the graph view
        /// (its VisualElement initializing takes place in its constructor)
        /// </summary>
        void AddGraphView()
        {
            graphView = new DescantGraphView(this);
            
            rootVisualElement.Add(graphView);
            graphView.StretchToParentSize(); // Making sure it's properly scaled up
        }

        /// <summary>
        /// Initializes the toolbar's VisualElements
        /// </summary>
        void AddToolbar()
        {
            toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            // Initializing the title section
            VisualElement toolbarTitle = new VisualElement();
            toolbarTitle.AddToClassList("toolbar-title");
            toolbar.Add(toolbarTitle);

            // Initializing the Descant graph's file name
            TextElement fileName = new TextElement();
            fileName.AddToClassList("toolbar-filename");
            fileName.text = data.Name;
            toolbarTitle.Add(fileName);

            // Initializing the unsaved marker next to the file name
            unsaved = new TextElement();
            unsaved.AddToClassList("toolbar-unsaved");
            unsaved.text = "*";
            unsaved.visible = false;
            toolbarTitle.Add(unsaved);

            // Initializing the save section
            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);
            
            // Initializing the autosave toggle button
            autoSave = new Toggle("Autosave:");
            autoSave.value = data.Autosave;
            saveSection.Add(autoSave);

            // Initializing the save button
            Button save = new Button();
            save.clicked += () => Save(true);
            save.text = "Save";
            saveSection.Add(save);

            if (autoSave.value) save.visible = false;
            
            // Adding a callback for when the autosave value is changed
            autoSave.RegisterValueChangedCallback(callback =>
            {
                // Setting the save button's visibility based on the autosave button's value
                if (autoSave.value) save.visible = false;
                else save.visible = true;

                Save();
            });
            
            // Initializing the close button
            Button close = new Button();
            close.clicked += Unload;
            close.text = "Close";
            saveSection.Add(close);
        }

        /// <summary>
        /// Adds the stylesheet to the editor
        /// (the DescantGraphView needs to also have the style sheet set)
        /// </summary>
        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Checks through the entire graph view and toolbar for relevant information,
        /// and packages it into a DescantGraphData object
        /// </summary>
        /// <returns>A data object holding the information of the current editor</returns>
        DescantGraphData GetData()
        {
            // Initializing the new data object
            // (the name and path are copied from the current data,
            // but everything else comes from the actual VisualElements)
            DescantGraphData temp = new DescantGraphData(data.Name)
            {
                Path = data.Path,
                Autosave = autoSave.value,
                ChoiceNodeID = graphView.ChoiceNodeID,
                ResponseNodeID = graphView.ResponseNodeID,
                EndNodeID = graphView.EndNodeID,
                GroupID = graphView.GroupID,
                ChoiceNodes = new List<DescantChoiceNodeData>(),
                ResponseNodes = new List<DescantResponseNodeData>(),
                StartNode = null,
                EndNodes = new List<DescantEndNodeData>(),
                Connections = new List<DescantConnectionData>(),
                Groups = new List<DescantGroupData>()
            };

            // Checking through the current DescantChoiceNodes
            foreach (var i in graphView.ChoiceNodes)
            {
                var choices = new List<string>();
                var ports = DescantEditorUtilities.FindAllElements<Port>(i);
                
                for (int ii = 0; ii < ports.Count; ii++)
                {
                    if (ii == 0)
                    {
                        // Creating DescantConnectionData objects for each incoming connection
                        foreach (var iii in ports[ii].connections)
                        {
                            DescantNode outputNode = (DescantNode)iii.output.node;
                    
                            temp.Connections.Add(new DescantConnectionData(
                                outputNode.Type.ToString(),
                                outputNode.ID,
                                i.Type.ToString(),
                                i.ID
                            ));
                        }
                    }
                    else
                    {
                        // Saving the text for each choice
                        choices.Add(DescantEditorUtilities.FindFirstElement<TextField>(ports[ii]).value);

                        // Creating DescantConnectionData objects for each outgoing connection
                        if (ports[ii].connections.Any())
                        {
                            DescantNode inputNode = (DescantNode)ports[ii].connections.ElementAt(0).input.node;
                    
                            temp.Connections.Add(new DescantConnectionData(
                                i.Type.ToString(),
                                i.ID,
                                inputNode.Type.ToString(),
                                inputNode.ID,
                                ii
                            ));
                        }
                    }
                }

                // Creating the actual DescantChoiceNodeData object
                temp.ChoiceNodes.Add(new DescantChoiceNodeData(
                    DescantEditorUtilities.FindFirstElement<TextField>(i).value,
                    i.Type.ToString(),
                    i.ID,
                    i.GetPosition().position,
                    choices
                ));
            }

            // Checking through the current DescantResponseNodes
            foreach (var j in graphView.ResponseNodes)
            {
                List<TextField> fields = DescantEditorUtilities.FindAllElements<TextField>(j);
                var ports = DescantEditorUtilities.FindAllElements<Port>(j);

                // Creating DescantConnectionData objects for each incoming connection
                foreach (var ji in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ji.output.node;
                    
                    temp.Connections.Add(new DescantConnectionData(
                        outputNode.Type.ToString(),
                        outputNode.ID,
                        j.Type.ToString(),
                        j.ID
                    ));
                }

                // Creating a DescantConnectionData object for the outgoing connection
                if (ports[1].connections.Any())
                {
                    DescantNode inputNode = (DescantNode)ports[1].connections.ElementAt(0).input.node;
                
                    temp.Connections.Add(new DescantConnectionData(
                        inputNode.Type.ToString(),
                        inputNode.ID,
                        inputNode.Type.ToString(),
                        inputNode.ID
                    ));
                }
                
                // Creating the actual DescantResponseNodeData object
                temp.ResponseNodes.Add(new DescantResponseNodeData(
                    fields[0].value,
                    j.Type.ToString(),
                    j.ID,
                    j.GetPosition().position,
                    fields[1].value
                ));
            }

            // Checking the current DescantStartNode
            if (graphView.StartNode != null)
            {
                var startPort = DescantEditorUtilities.FindFirstElement<Port>(graphView.StartNode);
                
                // Creating a DescantConnectionData object for the outgoing connection
                if (startPort.connections.Any())
                {
                    DescantNode inputNode = (DescantNode)startPort.connections.ElementAt(0).input.node;
                
                    temp.Connections.Add(new DescantConnectionData(
                        graphView.StartNode.Type.ToString(),
                        graphView.StartNode.ID,
                        inputNode.Type.ToString(),
                        inputNode.ID
                    ));
                }
                
                // Creating the actual DescantStartNodeData object
                temp.StartNode = new DescantStartNodeData(
                    DescantEditorUtilities.FindFirstElement<TextField>(graphView.StartNode).value,
                    graphView.StartNode.Type.ToString(),
                    graphView.StartNode.GetPosition().position
                );
            }

            // Checking through the current DescantEndNodes
            foreach (var k in graphView.EndNodes)
            {
                var ports = DescantEditorUtilities.FindAllElements<Port>(k);
                
                // Creating DescantConnectionData objects for each incoming connection
                foreach (var ji in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ji.output.node;
                    
                    temp.Connections.Add(new DescantConnectionData(
                        outputNode.Type.ToString(),
                        outputNode.ID,
                        k.Type.ToString(),
                        k.ID
                    ));
                }
                
                // Creating the actual DescantEndNodeData object
                temp.EndNodes.Add(new DescantEndNodeData(
                    DescantEditorUtilities.FindFirstElement<TextField>(k).value,
                    k.Type.ToString(),
                    k.ID,
                    k.GetPosition().position
                ));
            }

            // Checking through the current DescantNodeGroups
            foreach (var l in graphView.Groups)
            {
                var contained = l.containedElements;
                var elements = new List<string>();
                var elementIDs = new List<int>();

                // Saving the type and IDs of each DescantNode contained within the DescantNodeGroup
                foreach (var li in contained)
                {
                    // Making sure the contained element is one of the ones already saved
                    // (we don't want any 'pointers' to non-saved nodes)
                    if (graphView.ChoiceNodes.Contains(li) ||
                        graphView.ResponseNodes.Contains(li) ||
                        graphView.StartNode.Equals(li) ||
                        graphView.EndNodes.Contains(li))
                    {
                        elements.Add(((DescantNode)li).Type.ToString());
                        elementIDs.Add(((DescantNode)li).ID);   
                    }
                }

                // Creating the actual DescantNodeGroupData object
                temp.Groups.Add(new DescantGroupData(
                    DescantEditorUtilities.FindFirstElement<TextField>(l).value,
                    l.ID,
                    l.GetPosition().position,
                    elements,
                    elementIDs
                ));
            }
            
            temp.CleanUpConnections();

            return temp;
        }

        /// <summary>
        /// Saves the data if autosave is turned on, otherwise it marks that data needs to be saved
        /// </summary>
        public void CheckAndSave()
        {
            // Making sure the toolbar has actually be loaded first
            if (autoSave != null)
            {
                if (autoSave.value) Save();
                // Getting a copy of the current data to compare it with the saved data
                else if (!data.Equals(GetData()))
                    unsaved.visible = true;
            }
        }

        /// <summary>
        /// Gathers and saves the data from the current graph view and toolbar
        /// </summary>
        /// <param name="refresh">Whether to refresh the AssetDatabase after the data has been saved</param>
        void Save(bool refresh = false)
        {
            DescantEditorUtilities.FindFirstElement<TextElement>(toolbar).text = data.Name;
            unsaved.visible = false;
            
            data = GetData();
            data.Save(false);
            
            if (refresh) AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads the data from a Descant file
        /// </summary>
        /// <param name="fullPath">The full disc path to the Descant file to load</param>
        public void Load(string fullPath)
        {
            // Making sure the path isn't null or empty
            if (fullPath != null && fullPath.Trim() != "")
            {
                lastLoaded = fullPath;

                data = DescantGraphData.LoadFromPath(fullPath);
                
                // Reloading the name and path, in case they got changed after the last time this file was loaded
                data.Name = DescantEditorUtilities.GetDescantFileNameFromPath(fullPath);
                data.Path = DescantEditorUtilities.RemoveBeforeLocalPath(fullPath);
                
                data.Save(false);

                loaded = true;

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
            
            loaded = false;
            lastLoaded = null;
            
            RemoveGUI();
        }

        /// <summary>
        /// Creates a new blank file, and reloads the GUI
        /// </summary>
        public void NewFile()
        {
            data = new DescantGraphData("New Descant Graph");
            data.Save(true);

            loaded = true;
            
            ReloadGUI();
        }
    }
}
#endif