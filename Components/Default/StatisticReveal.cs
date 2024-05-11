// Please see https://omch.tech/descant/#statisticreveal for documentation

using System;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantComponent
    {
        [Inline] public DescantActor Actor;
        
        [ParameterGroup("Statistic to reveal")] public string StatisticName;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            if (DescantComponentUtilities.InvokeFromObjectOrScript(
                this,
                ObjectTag,
                ScriptName,
                MethodName,
                Actor.Statistics[StatisticName].ToString()
            )) return result;
            
            DescantComponentUtilities.MissingMethodError(this, ScriptName, MethodName);
            
            return result;
        }
    }
}