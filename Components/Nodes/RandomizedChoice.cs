using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class RandomizedChoice : DescantNodeComponent
    {
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantNodeInvokeResult temp = new DescantNodeInvokeResult(
                new List<KeyValuePair<int, string>>(),
                result.Actors
            );

            int copy = result.Choices.Count;
            
            for (int i = 0; i < copy; i++)
            {
                var moved = result.Choices[Random.Range(0, result.Choices.Count)];
                temp.Choices.Add(moved);
                result.Choices.Remove(moved);
            }

            return temp;
        }
    }
}