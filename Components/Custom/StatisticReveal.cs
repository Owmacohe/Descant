// Please see https://omch.tech/descant/#statisticreveal for documentation

using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to reveal")] public string StatisticName;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null || ScriptName == "" || MethodName == "") return result;

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