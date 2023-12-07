using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        
        [ParameterGroup("Statistic to reveal")] public string StatisticName;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null) return result;

            if (DescantComponentUtilities.InvokeFromObjectOrScript(
                this,
                ObjectTag,
                ScriptName,
                MethodName,
                actor.StatisticValues[actor.StatisticKeys.IndexOf(StatisticName)].ToString()
            )) return result;
            
            DescantComponentUtilities.MissingMethodError(this, ScriptName, MethodName);
            
            return result;
        }
    }
}