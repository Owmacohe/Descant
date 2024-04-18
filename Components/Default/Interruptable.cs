// Please see https://omch.tech/descant/#interruptable for documentation

using System;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantComponent
    {
        [ParameterGroup("Keys/buttons to check")] public string KeyCode;
        [ParameterGroup("Keys/buttons to check")] public string ButtonName;
        
        [ParameterGroup("Tag of object to find")] public string ObjectTag;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;
        [ParameterGroup("Method to call"), NoFiltering] public string Parameter;

        public override bool Update()
        {
            bool interrupted = (KeyCode != "" && Input.GetKeyDown(UnityEngine.KeyCode.A)) ||
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