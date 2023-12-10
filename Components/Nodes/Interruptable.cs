using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantNodeComponent
    {
        [ParameterGroup("Keys/buttons to check")] public string KeyCode;
        [ParameterGroup("Keys/buttons to check")] public string ButtonName;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call"), NoFiltering] public string Parameter;

        public override bool Update()
        {
            bool interrupted = (KeyCode != "" && Input.GetKeyDown(KeyCode)) ||
                               (ButtonName != "" && Input.GetButtonDown(ButtonName));

            if (interrupted && ScriptName != "" && MethodName != "")
            {
                bool invoked = DescantComponentUtilities.InvokeFromObjectOrScript(
                    this,
                    ObjectTag,
                    ScriptName,
                    MethodName,
                    Parameter
                );
                
                if (!invoked) DescantComponentUtilities.MissingMethodError(this, ScriptName, MethodName);
                
                invoked = DescantComponentUtilities.InvokeFromObjectOrScript(
                    this,
                    "",
                    "DescantDialogueUI",
                    "EndDialogue",
                    ""
                );

                if (!invoked) DescantComponentUtilities.MissingMethodError(
                    this,
                    "DescantDialogueUI",
                    "EndDialogue"
                );
            }

            return !interrupted;
        }
    }
}