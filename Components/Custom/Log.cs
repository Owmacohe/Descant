// Please see https://omch.tech/descant/#log for documentation

using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Log : DescantComponent
    {
        [Inline] public string Message;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            Debug.Log(Message);
            return result;
        }
    }
}