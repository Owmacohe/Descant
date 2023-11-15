using System;
using System.Collections.Generic;
using DescantUtilities;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Log : DescantNodeComponent
    {
        [Inline] public string Message;
        
        public override List<string> Invoke(List<string> choices)
        {
            Debug.Log(Message);
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}