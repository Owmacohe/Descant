using System;
using System.Collections.Generic;
using DescantUtilities;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class EventScript : DescantNodeComponent
    {
        [ParameterGroup("Script to call")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call")] public string Parameter;

        public override List<string> Invoke(List<string> choices)
        {
            foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
                if (DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, Parameter))
                    return choices;
            
            Debug.Log("<b>EventScript:</b> Unable to find or execute the given script!");
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
    
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class EventObject : DescantNodeComponent
    {
        [Inline] public string ObjectTag;
        [ParameterGroup("Script to call")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call")] public string Parameter;

        public override List<string> Invoke(List<string> choices)
        {
            foreach (var i in GameObject.FindWithTag(ObjectTag).GetComponents<MonoBehaviour>())
                if (DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, Parameter))
                    return choices;
            
            Debug.Log("<b>EventScript:</b> Unable to find or execute the given script!");
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}