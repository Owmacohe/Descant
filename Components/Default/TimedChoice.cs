// Please see https://omch.tech/descant/#timedchoice for documentation

using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantComponent
    {
        [Inline] public float Time;
        
        [ParameterGroup("When timer runs out (base 1)")] public int ChoiceToPick;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Methods to call")] public string TimerMethodName;
        [ParameterGroup("Methods to call")] public string FinishedMethodName;

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
                if (ScriptName != "")
                {
                    if (TimerMethodName != "" && !DescantComponentUtilities.InvokeFromObjectOrScript(
                            this,
                            ObjectTag,
                            ScriptName,
                            TimerMethodName,
                            percentage.ToString()
                        )) DescantComponentUtilities.MissingMethodError(this, ScriptName, TimerMethodName);

                    if (FinishedMethodName != "" && percentage >= 1)
                        if (!DescantComponentUtilities.InvokeFromObjectOrScript(
                                this,
                                "",
                                "DescantDialogueUI",
                                "DisplayNode",
                                (ChoiceToPick - 1).ToString()
                            )) DescantComponentUtilities.MissingMethodError(this, ScriptName, FinishedMethodName);   
                }
            }

            return true;
        }
    }
}