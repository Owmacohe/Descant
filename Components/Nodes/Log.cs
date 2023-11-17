using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Log : DescantNodeComponent
    {
        [Inline] public string Message;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            Debug.Log(Message);
            return result;
        }
    }
}