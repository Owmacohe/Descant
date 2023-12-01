using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Script to find")] public string ScriptName;
        
        [ParameterGroup("Method to call")] public string MethodName;
        
        [ParameterGroup("Statistic to reveal")] public string StatisticName;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, ActorName);

            if (actor == null) return result;
            
            foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
                if (DescantComponentUtilities.InvokeMethod(
                        i, ScriptName, MethodName,
                        actor.StatisticValues[actor.StatisticKeys.IndexOf(StatisticName)].ToString()))
                    return result;
            
            Debug.Log("<b>EventScript:</b> Unable to find or execute the given script!");
            
            return result;
        }
    }
}