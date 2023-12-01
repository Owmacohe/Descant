using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantNodeComponent
    {
        [ParameterGroup("Keys/buttons to check")] public string KeyCode;
        [ParameterGroup("Keys/buttons to check")] public string ButtonName;
        [ParameterGroup("Script to find")] public string ScriptName;
        [ParameterGroup("Method to call")] public string MethodName;

        public override bool Update()
        {
            bool interrupted = (KeyCode != "" && Input.GetKeyDown(KeyCode)) ||
                               (ButtonName != "" && Input.GetButtonDown(ButtonName));

            if (interrupted && ScriptName != "" && MethodName != "")
            {
                bool invoked = false;

                foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
                {
                    if (DescantComponentUtilities.InvokeMethod(i, ScriptName, MethodName, null))
                    {
                        invoked = true;
                        break;
                    }
                }

                if (!invoked)
                {
                    // TODO: unable to find script error message   
                }
            }

            return !interrupted;
        }
    }
}