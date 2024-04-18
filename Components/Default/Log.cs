// Please see https://omch.tech/descant/#log for documentation

using System;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Log : DescantComponent
    {
        [Inline, NoFiltering] public string Message;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantUtilities.LogMessage(GetType(), Message);
            return result;
        }
    }
}