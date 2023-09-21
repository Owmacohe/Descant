using System;
using System.Collections.Generic;
using System.IO;
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
        
        [MenuItem("Window/Descant/Descant Editor")]
        public static void Open()
        {
            GetWindow<DescantEditor>("Descant Editor");
        }

        void CreateGUI()
        {
            Load();
            
            AddGraphView();
            AddToolbar();
            
            AddStyleSheet();
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

            TextElement fileName = new TextElement();
            fileName.AddToClassList("toolbar-title");
            fileName.text = data.Name;
            toolbar.Add(fileName);

            VisualElement saveSection = new VisualElement();
            saveSection.AddToClassList("save-section");
            toolbar.Add(saveSection);

            Button save = new Button();
            save.clicked += Save;
            save.text = "Save";
            saveSection.Add(save);

            AutoSave = new Toggle("Autosave:");
            AutoSave.value = data.Autosave;
            saveSection.Add(AutoSave);

            if (AutoSave.value) save.visible = false;

            AutoSave.RegisterValueChangedCallback(callback =>
            {
                if (AutoSave.value)
                {
                    save.visible = false;
                    Save();
                }
                else save.visible = true;
            });
        }

        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantStyleSheet.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        public void Save()
        {
            data.Autosave = AutoSave.value;
            
            data.ChoiceNodeID = graphView.ChoiceNodeID;
            data.ResponseNodeID = graphView.ResponseNodeID;
            data.EndNodeID = graphView.EndNodeID;
            data.GroupID = graphView.GroupID;
            
            data.ChoiceNodes = new List<ChoiceNodeData>();
            data.ResponseNodes = new List<ResponseNodeData>();
            data.StartNode = null;
            data.EndNodes = new List<EndNodeData>();
            
            data.Connections = new List<ConnectionData>();
            
            data.Groups = new List<GroupData>();

            foreach (var i in graphView.ChoiceNodes)
            {
                var choices = new List<string>();
                var ports = DescantUtilities.FindAllElements<Port>(i);
                string nodeName = DescantUtilities.FindFirstElement<TextField>(i).value;
                
                foreach (var ii in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ii.output.node;
                    
                    data.Connections.Add(new ConnectionData(
                        DescantUtilities.FindFirstElement<TextField>(outputNode).value,
                        outputNode.ID,
                        nodeName,
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
                    
                            data.Connections.Add(new ConnectionData(
                                nodeName,
                                i.ID,
                                DescantUtilities.FindFirstElement<TextField>(inputNode).value,
                                inputNode.ID,
                                ij
                            ));
                        }
                    }
                }

                data.ChoiceNodes.Add(new ChoiceNodeData(nodeName, i.ID, i.GetPosition().position, choices));
            }

            foreach (var j in graphView.ResponseNodes)
            {
                List<TextField> fields = DescantUtilities.FindAllElements<TextField>(j);
                var ports = DescantUtilities.FindAllElements<Port>(j);
                string nodeName = fields[0].value;

                foreach (var ji in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ji.output.node;
                    
                    data.Connections.Add(new ConnectionData(
                        DescantUtilities.FindFirstElement<TextField>(outputNode).value,
                        outputNode.ID,
                        nodeName,
                        j.ID
                    ));
                }

                if (ports[1].connections.Any())
                {
                    DescantNode inputNode = (DescantNode)ports[1].connections.ElementAt(0).input.node;
                
                    data.Connections.Add(new ConnectionData(
                        nodeName,
                        inputNode.ID,
                        DescantUtilities.FindFirstElement<TextField>(inputNode).value,
                        inputNode.ID
                    ));
                }
                
                data.ResponseNodes.Add(new ResponseNodeData(
                    nodeName,
                    j.ID,
                    j.GetPosition().position,
                    fields[1].value
                ));
            }

            if (graphView.StartNode != null)
            {
                var startPort = DescantUtilities.FindFirstElement<Port>(graphView.StartNode);
                string startNodeName = DescantUtilities.FindFirstElement<TextField>(graphView.StartNode).value;
                
                if (startPort.connections.Any())
                {
                    DescantNode inputNode = (DescantNode)startPort.connections.ElementAt(0).input.node;
                
                    data.Connections.Add(new ConnectionData(
                        startNodeName,
                        graphView.StartNode.ID,
                        DescantUtilities.FindFirstElement<TextField>(inputNode).value,
                        inputNode.ID
                    ));
                }
                
                data.StartNode = new StartNodeData(startNodeName, graphView.StartNode.GetPosition().position);
            }

            foreach (var k in graphView.EndNotes)
            {
                var ports = DescantUtilities.FindAllElements<Port>(k);
                string nodeName = DescantUtilities.FindFirstElement<TextField>(k).value;
                
                foreach (var ji in ports[0].connections)
                {
                    DescantNode outputNode = (DescantNode)ji.output.node;
                    
                    data.Connections.Add(new ConnectionData(
                        DescantUtilities.FindFirstElement<TextField>(outputNode).value,
                        outputNode.ID,
                        nodeName,
                        k.ID
                    ));
                }
                
                data.EndNodes.Add(new EndNodeData(nodeName, k.ID, k.GetPosition().position));
            }

            foreach (var l in graphView.Groups)
            {
                var contained = l.containedElements;
                var elements = new List<string>();
                var elementIDs = new List<int>();

                foreach (var li in contained)
                {
                    elements.Add(DescantUtilities.FindFirstElement<TextField>(li).value);
                    elementIDs.Add(((DescantNode) li).ID);
                }

                data.Groups.Add(new GroupData(
                    DescantUtilities.FindFirstElement<TextField>(l).value,
                    l.ID,
                    l.GetPosition().position,
                    elements,
                    elementIDs
                ));
            }

            data.ClearConnectionDuplicates();
            data.ClearConnectionDuplicates();
            data.Save();
            AssetDatabase.Refresh();
        }

        void Load()
        {
            data = JsonUtility.FromJson<DescantGraphData>(File.ReadAllText(Application.dataPath + "/Loaded.desc"));
            if (data == null) data = new DescantGraphData("DescantDialogue");

            //RemoveGUI();
            //CreateGUI();
        }
    }
}
