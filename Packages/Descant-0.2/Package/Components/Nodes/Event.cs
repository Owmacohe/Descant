using System;
using UnityEngine.Events;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Event :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [ParameterGroup("Event to call")] public string ScriptName;
        [ParameterGroup("Event to call")] public string Parameter;
        
        public Event(
            DescantConversationController controller,
            int nodeID,
            int id,
            string scriptName, string parameter)
            : base(controller, nodeID, id)
        {
            ScriptName = scriptName;
            Parameter = parameter;
        }

        public void Invoke()
        {
            // TODO: call method using
            // ScriptName and Parameter
        }
    }
}