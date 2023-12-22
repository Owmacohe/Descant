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