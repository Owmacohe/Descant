using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class EventScript : DescantNodeComponent
    {
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call")] public string Parameter;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
                if (DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, Parameter))
                    return result;
            
            Debug.Log("<b>EventScript:</b> Unable to find or execute the given script!");
            return result;
        }
    }
    
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class EventObject : DescantNodeComponent
    {
        [Inline] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call")] public string Parameter;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            foreach (var i in GameObject.FindWithTag(ObjectTag).GetComponents<MonoBehaviour>())
                if (DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, Parameter))
                    return result;
            
            Debug.Log("<b>EventScript:</b> Unable to find or execute the given script!");
            return result;
        }
    }
}