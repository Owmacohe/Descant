using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class RandomizedChoice : DescantNodeComponent // TODO: make this always last
    {
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            var temp = new List<KeyValuePair<int, string>>();

            int copy = result.Choices.Count;
            
            for (int i = 0; i < copy; i++)
            {
                var moved = result.Choices[Random.Range(0, result.Choices.Count)];
                temp.Add(moved);
                result.Choices.Remove(moved);
            }

            result.Choices = temp;

            return result;
        }
    }
}