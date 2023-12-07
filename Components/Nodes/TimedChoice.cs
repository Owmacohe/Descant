using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantNodeComponent
    {
        [Inline] public float Time;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Methods to call")] public string TimerMethodName;
        [ParameterGroup("Methods to call")] public string FinishedMethodName;
        
        [ParameterGroup("When timer runs out (base 1)")] public int ChoiceToPick;

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
                if (ScriptName != "" && TimerMethodName != "" && !DescantComponentUtilities.InvokeFromObjectOrScript(
                    this,
                    ObjectTag,
                    ScriptName,
                    TimerMethodName,
                    percentage.ToString()
                )) DescantComponentUtilities.MissingMethodError(this, ScriptName, TimerMethodName);

                if (ScriptName != "" && FinishedMethodName != "" && percentage >= 1)
                    if (!DescantComponentUtilities.InvokeFromObjectOrScript(
                        this,
                        ObjectTag,
                        ScriptName,
                        FinishedMethodName,
                        (ChoiceToPick - 1).ToString()
                    )) DescantComponentUtilities.MissingMethodError(this, ScriptName, FinishedMethodName);
            }

            return true;
        }
    }
}