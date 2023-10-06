using System;
using System.Collections.Generic;
using Editor;
using Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Runtime
{
    public class DescantGraphController : MonoBehaviour
    {
        [SerializeField] DefaultAsset descantGraph;

        DescantGraphData data;
        DescantNodeData current;

        void Start()
        {
            data = DescantGraphData.Load(
                DescantUtilities.GetPathFromInstanceID(
                    descantGraph.GetInstanceID()));

            current = data.StartNode;
            Debug.Log(current);
            
            InvokeRepeating(nameof(Next), 1, 1);
        }

        string IsolateType(string fullTypeName)
        {
            fullTypeName = fullTypeName.Substring(19);
            fullTypeName = fullTypeName.Remove(fullTypeName.Length - 8);
            
            return fullTypeName;
        }

        DescantNodeData FindNext(string type, int id)
        {
            switch (type)
            {
                case "Choice":
                    foreach (var i in data.ChoiceNodes)
                        if (i.ID == id)
                            return i;
                    break;
                case "Response":
                    foreach (var i in data.ResponseNodes)
                        if (i.ID == id)
                            return i;
                    break;
                case "End":
                    foreach (var i in data.EndNodes)
                        if (i.ID == id)
                            return i;
                    break;
            }

            return null;
        }

        public void Next()
        {
            string isolatedType = IsolateType(current.GetType().ToString());

            if (isolatedType == "End") return;
            
            foreach (var i in data.Connections)
            {
                if (i.From == isolatedType && i.FromID == current.ID)
                {
                    current = FindNext(i.To, i.ToID);
                    break;
                }
            }

            Debug.Log(current);
        }
    }
}