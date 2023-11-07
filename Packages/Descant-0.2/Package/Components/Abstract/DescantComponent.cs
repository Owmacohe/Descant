using System;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity)]
    public abstract class DescantComponent
    {
        public int ID;
        protected DescantConversationController Controller;
        
        protected DescantComponent(DescantConversationController controller, int id)
        {
            ID = id;
            Controller = controller;
        }

        public override string ToString()
        {
            return GetType() + "(" + ID + ")";
        }
    }

    public class MaxQuantityAttribute : Attribute
    {
        public readonly float Quantity;
        
        public MaxQuantityAttribute(float quantity) => Quantity = quantity;
    }
    
    public class InlineGroupAttribute : Attribute
    {
        public readonly int Line;
        
        public InlineGroupAttribute(int line) => Line = line;
    }
}