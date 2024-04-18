#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Descant.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// The custom Unity EditorWindow for loading, saving, and manipulating Descant graphs
    /// </summary>
    public class DescantEditor : EditorWindow
    {
        /// <summary>
        /// The data object for the current Descant graph
        /// </summary>
        public DescantGraph data;
        
        DescantGraphView graphView; // The graph view part the editor
        Toolbar toolbar; // The toolbar part of the editor
        
        Toggle typewriter; // The Toggle VisualElement for turning the typewriter on and off
        TextField typewriterSpeed; // The field for inputting the typewriter speed
        
        Toggle autoSave; // The autosave toggle button in the toolbar
        TextElement unsaved; // The unsaved changes marker in the toolbar
        
        bool GUICreated; // Whether a Descant graph is currently loaded into the editor
        
        [MenuItem("Window/Descant/Graph Editor"), MenuItem("Descant/Graph Editor")]
        public static void Open()
        {
            GetWindow<DescantEditor>("Descant Graph Editor");
        }

        #region GUI

        void CreateGUI()
        {
            // If the graph data has already been loaded into the editor, we simple generate the graph view and toolbar
            // (they're dependant on the data having been previously loaded)
            if (GUICreated)
            {
                AddGraphView();
                AddToolbar();
                
                DescantEditorUtilities.AddStyleSheet(rootVisualElement.styleSheets, "DescantGraphEditorStyleSheet");
            }
            // Otherwise, we first load the data before adding the graph view and toolbar
            // (the Load method will call CreateGUI again when it has finished)
            else
            {
                Load(data);
                AssetDatabase.Refresh();
            }

            // Resetting the loaded variable to indicate that the UI should be
            // reloaded if CreateGUI is ever called again when Unity is refreshed
            // (otherwise the graph view and toolbar would be added without any data to initialize them with)
            GUICreated = false;
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
            fileName.text = data.name;
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

            // Initializing the typewriter toggle
            typewriter = new Toggle("Typewriter:");
            typewriter.value = data.Typewriter;
            saveSection.Add(typewriter);

            // Initializing the typewriter speed
            typewriterSpeed = new TextField("Speed:");
            typewriterSpeed.value = data.TypewriterSpeed.ToString();
            saveSection.Add(typewriterSpeed);
            
            if (!typewriter.value) typewriterSpeed.visible = false;

            // Adding a callback for when the typewriter toggle value is changed
            typewriter.RegisterValueChangedCallback(callback =>
            {
                typewriterSpeed.visible = typewriter.value;
                
                Save();
            });
            
            /*
            // Initializing the autosave toggle button
            autoSave = new Toggle("Autosave:");
            autoSave.value = data.Autosave;
            saveSection.Add(autoSave);
            */

            // Initializing the save button
            Button save = new Button();
            save.clicked += Save;
            save.text = "Save";
            saveSection.Add(save);

            /*
            if (autoSave.value) save.visible = false;
            
            // Adding a callback for when the autosave value is changed
            autoSave.RegisterValueChangedCallback(callback =>
            {
                // Setting the save button's visibility based on the autosave button's value
                save.visible = !autoSave.value;

                Save();
            });
            */
            
            // Initializing the close button
            Button close = new Button();
            close.clicked += Unload;
            close.text = "Close";
            saveSection.Add(close);
        }
        
        #endregion
        
        #region Saving and Loading

        /// <summary>
        /// Checks through the entire graph view and toolbar for relevant information,
        /// and packages it into a DescantGraphData object
        /// </summary>
        /// <returns>A data object holding the information of the current editor</returns>
        DescantGraph GetData()
        {
            // Initializing the new data object
            // (the name and path are copied from the current data,
            // but everything else comes from the actual VisualElements)
            DescantGraph temp = CreateInstance<DescantGraph>();
            temp.Typewriter = typewriter.value;
            temp.TypewriterSpeed = float.Parse(typewriterSpeed.value);
            temp.ChoiceNodeID = graphView.ChoiceNodeID;
            temp.ResponseNodeID = graphView.ResponseNodeID;
            temp.EndNodeID = graphView.EndNodeID;
            temp.GroupID = graphView.GroupID;
            temp.StartNode = null;

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
                        else
                        {
                            temp.Connections.Add(new DescantConnectionData(
                                i.Type.ToString(),
                                i.ID,
                                "null",
                                -1,
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
                    choices,
                    DescantEditorUtilities.FindAllElements<DescantNodeComponentVisualElement>(i)
                        .Select(visualElement => visualElement.Component)
                        .ToList()
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
                    fields[1].value,
                    DescantEditorUtilities.FindAllElements<DescantNodeComponentVisualElement>(j)
                        .Select(visualElement => visualElement.Component)
                        .ToList()
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
                    graphView.StartNode.GetPosition().position,
                    DescantEditorUtilities.FindAllElements<DescantNodeComponentVisualElement>(graphView.StartNode)
                        .Select(visualElement => visualElement.Component)
                        .ToList()
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
                    k.GetPosition().position,
                    DescantEditorUtilities.FindAllElements<DescantNodeComponentVisualElement>(k)
                        .Select(visualElement => visualElement.Component)
                        .ToList()
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
        /// Method to copy all values from one DescantGraph to another
        /// (simply assigning SerializedObjects breaks the link to the object in the project files)
        /// </summary>
        void AssignData()
        {
            var temp = GetData();

            data.Autosave = temp.Autosave;
            data.Typewriter = temp.Typewriter;
            data.TypewriterSpeed = temp.TypewriterSpeed;
            data.ChoiceNodeID = temp.ChoiceNodeID;
            data.ResponseNodeID = temp.ResponseNodeID;
            data.EndNodeID = temp.EndNodeID;
            data.GroupID = temp.GroupID;

            data.ChoiceNodes = temp.ChoiceNodes;
            data.ResponseNodes = temp.ResponseNodes;
            data.StartNode = temp.StartNode;
            data.EndNodes = temp.EndNodes;

            data.Groups = temp.Groups;
            data.Connections = temp.Connections;
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
        void Save()
        {
            DescantEditorUtilities.FindFirstElement<TextElement>(toolbar).text = data.name;
            unsaved.visible = false;

            AssignData();

            DescantUtilities.SaveSerializedObject(data);
        }

        /// <summary>
        /// Loads the data from a Descant graph file
        /// </summary>
        public void Load(DescantGraph graph)
        {
            if (graph != null)
            {
                data = graph;

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
            
            RemoveGUI();
        }
        
        #endregion
    }
}
#endif