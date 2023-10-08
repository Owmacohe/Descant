using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Data;
using Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Window
{
    public class DescantEditor : EditorWindow
    {
        public DescantGraphData data;
        public Toggle AutoSave;
        
        DescantGraphView graphView;
        Toolbar toolbar;
        TextElement unsaved;

        string lastLoaded;
        bool loaded;
        
        [MenuItem("Window/Descant/Descant Editor")]
        public static void Open()
        {
            GetWindow<DescantEditor>("Descant Editor");
        }

        void CreateGUI()
        {
            if (loaded)
            {
                AddGraphView();
                AddToolbar();
            
                AddStyleSheet();
                
                /*
                rootVisualElement.RegisterCallback<KeyDownEvent>(callback =>
                {
                    // TODO: ctrl + S
                });
                */
            }
            else
            {
                Load(lastLoaded);
            }

            loaded = false;
        }

        void RemoveGUI()
        {
            DescantUtilities.RemoveElement(graphView);
            DescantUtilities.RemoveElement(toolbar);
        }

        void AddGraphView()
        {
            graphView = new DescantGraphView(this);
            
            rootVisualElement.Add(graphView);
            graphView.StretchToParentSize();
        }

        void AddToolbar()
        {
            toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            VisualElement toolbarTitle = new VisualElement();
            toolbarTitle.AddToClassList("toolbar-title");
            toolbar.Add(toolbarTitle);

            TextElement fileName = new TextElement();
            fileName.AddToClassList("toolbar-filename");
            fileName.text = data.Name;
            toolbarTitle.Add(fileName);

            unsaved = new TextElement();
            unsaved.AddToClassList("toolbar-unsaved");
            unsaved.text = "*";
            unsaved.visible = false;
            toolbarTitle.Add(unsaved);

            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);
            
            AutoSave = new Toggle("Autosave:");
            AutoSave.value = data.Autosave;
            saveSection.Add(AutoSave);

            Button save = new Button();
            save.clicked += () => Save(true);
            save.text = "Save";
            saveSection.Add(save);

            if (AutoSave.value) save.visible = false;
            
            Button close = new Button();
            close.clicked += () =>
            {
                data = null;
                RemoveGUI();
            };
            close.text = "Close";
            saveSection.Add(close);

            AutoSave.RegisterValueChangedCallback(callback =>
            {
                if (AutoSave.value) save.visible = false;
                else save.visible = true;

                Save();
            });
        }

        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        public DescantGraphData GetData()
        {
            DescantGraphData temp = new DescantGraphData(data.Name);

            temp.Path = data.Path;
            
            temp.Autosave = AutoSave.value;
            
            temp.ChoiceNodeID = graphView.ChoiceNodeID;
            temp.ResponseNodeID = graphView.ResponseNodeID;
            temp.EndNodeID = graphView.EndNodeID;
            temp.GroupID = graphView.GroupID;
            
            temp.ChoiceNodes = new List<DescantChoiceNodeData>();
            temp.ResponseNodes = new List<DescantResponseNodeData>();
            temp.StartNode = null;
            temp.EndNodes = new List<DescantEndNodeData>();
            
            temp.Connections = new List<DescantConnectionData>();
            
            temp.Groups = new List<DescantGroupData>();

            foreach (var i in graphView.ChoiceNodes)
            {
                var choices = new List<string>();
                var ports = DescantUtilities.FindAllElements<Port>(i);
                
                foreach (var ii in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ii.output.node;
                    
                    temp.Connections.Add(new DescantConnectionData(
                        outputNode.Type.ToString(),
                        outputNode.ID,
                        i.Type.ToString(),
                        i.ID
                    ));
                }

                for (int ij = 0; ij < ports.Count; ij++)
                {
                    if (ij > 0)
                    {
                        choices.Add(DescantUtilities.FindFirstElement<TextField>(ports[ij]).value);

                        if (ports[ij].connections.Any())
                        {
                            DescantNode inputNode = (DescantNode)ports[ij].connections.ElementAt(0).input.node;
                    
                            temp.Connections.Add(new DescantConnectionData(
                                i.Type.ToString(),
                                i.ID,
                                inputNode.Type.ToString(),
                                inputNode.ID,
                                ij
                            ));
                        }
                    }
                }

                temp.ChoiceNodes.Add(new DescantChoiceNodeData(
                    DescantUtilities.FindFirstElement<TextField>(i).value,
                    i.Type.ToString(),
                    i.ID,
                    i.GetPosition().position,
                    choices
                ));
            }

            foreach (var j in graphView.ResponseNodes)
            {
                List<TextField> fields = DescantUtilities.FindAllElements<TextField>(j);
                var ports = DescantUtilities.FindAllElements<Port>(j);

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
                
                temp.ResponseNodes.Add(new DescantResponseNodeData(
                    fields[0].value,
                    j.Type.ToString(),
                    j.ID,
                    j.GetPosition().position,
                    fields[1].value
                ));
            }

            if (graphView.StartNode != null)
            {
                var startPort = DescantUtilities.FindFirstElement<Port>(graphView.StartNode);
                
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
                
                temp.StartNode = new DescantStartNodeData(
                    DescantUtilities.FindFirstElement<TextField>(graphView.StartNode).value,
                    graphView.StartNode.Type.ToString(),
                    graphView.StartNode.GetPosition().position
                );
            }

            foreach (var k in graphView.EndNodes)
            {
                var ports = DescantUtilities.FindAllElements<Port>(k);
                
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
                
                temp.EndNodes.Add(new DescantEndNodeData(
                    DescantUtilities.FindFirstElement<TextField>(k).value,
                    k.Type.ToString(),
                    k.ID,
                    k.GetPosition().position
                ));
            }

            foreach (var l in graphView.Groups)
            {
                var contained = l.containedElements;
                var elements = new List<string>();
                var elementIDs = new List<int>();

                foreach (var li in contained)
                {
                    if (graphView.ChoiceNodes.Contains(li) ||
                        graphView.ResponseNodes.Contains(li) ||
                        graphView.StartNode.Equals(li) ||
                        graphView.EndNodes.Contains(li))
                    {
                        elements.Add(((DescantNode)li).Type.ToString());
                        elementIDs.Add(((DescantNode)li).ID);   
                    }
                }

                temp.Groups.Add(new DescantGroupData(
                    DescantUtilities.FindFirstElement<TextField>(l).value,
                    l.ID,
                    l.GetPosition().position,
                    elements,
                    elementIDs
                ));
            }
            
            temp.CleanUpConnections();

            return temp;
        }

        public void Save(bool refresh = false)
        {
            DescantUtilities.FindFirstElement<TextElement>(toolbar).text = data.Name;
            unsaved.visible = false;
            
            data = GetData();
            
            data.Save(false);
            if (refresh) AssetDatabase.Refresh();
        }

        public void Load(string fullPath)
        {
            if (fullPath != null && fullPath.Trim() != "")
            {
                lastLoaded = fullPath;

                data = DescantGraphData.Load(fullPath);
                data.Name = DescantUtilities.GetDescantFileNameFromPath(fullPath);
                data.Path = DescantUtilities.RemoveBeforeLocalPath(fullPath);
                data.Save(false);

                loaded = true;

                AutoSave = null;
                RemoveGUI();
                CreateGUI();
            }
        }

        public void NewFile()
        {
            data = new DescantGraphData("New Descant Graph");
            data.Save(true);

            loaded = true;
            
            AutoSave = null;
            RemoveGUI();
            CreateGUI();
        }

        public void MarkUnsavedChanges()
        {
            unsaved.visible = true;
        }
    }
}
