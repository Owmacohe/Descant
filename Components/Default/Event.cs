// Please see https://omch.tech/descant/#event for documentation

using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Event : DescantComponent
    {
        [Inline] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call"), NoFiltering] public string Parameter;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            if (ScriptName == "" || MethodName == "") return result;
            
            if (DescantComponentUtilities.InvokeFromObjectOrScript(
                this,
                ObjectTag,
                ScriptName,
                MethodName,
                Parameter
            )) return result;

            DescantComponentUtilities.MissingMethodError(this, ScriptName, MethodName);
            
            return result;
        }
    }
}