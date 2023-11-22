using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantNodeComponent
    {
        [Inline] public float Time;
        
        [ParameterGroup("When timer runs out (base 1)")] public int ChoiceToPick;
        
        [ParameterGroup("Script to find")] public string ScriptName;
        
        [ParameterGroup("Method to call")] public string MethodName;

        float startTime;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            startTime = UnityEngine.Time.time;
            
            return result;
        }

        public override bool FixedUpdate()
        {
            float percentage = (UnityEngine.Time.time - startTime) / Time;
            
            foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
            {
                DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, percentage.ToString());
                
                if (percentage >= 1)
                    DescantComponentUtilities.InvokeMethod(
                        i, "DescantDialogueUI", "DisplayNode",
                        (ChoiceToPick - 1).ToString());
            }

            return true;
        }
    }
}