// Please see https://omch.tech/descant/#randomizedchoice for documentation

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class RandomizedChoice : DescantComponent
    {
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            var temp = new List<KeyValuePair<int, string>>();

            int copy = result.Text.Count;
            
            for (int i = 0; i < copy; i++)
            {
                var moved = result.Text[Random.Range(0, result.Text.Count)];
                temp.Add(moved);
                result.Text.Remove(moved);
            }

            result.Text = temp;

            return result;
        }
    }
}