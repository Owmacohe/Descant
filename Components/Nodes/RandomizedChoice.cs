using System;
using System.Collections.Generic;
using DescantUtilities;
using Random = UnityEngine.Random;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class RandomizedChoice : DescantNodeComponent
    {
        public override List<string> Invoke(List<string> choices)
        {
            List<string> temp = new List<string>();
            
            for (int i = 0; i < choices.Count; i++)
            {
                string moved = choices[Random.Range(0, choices.Count)];
                temp.Add(moved);
                choices.Remove(moved);
            }
            
            temp.Add(choices[0]);

            return temp;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}