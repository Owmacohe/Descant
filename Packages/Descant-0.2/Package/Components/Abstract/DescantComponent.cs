using System;
using Descant.Package.Runtime;
using UnityEngine;

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
        
        // TODO: some sort of generic Equals method?

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
    
    public class ParameterGroupAttribute : Attribute
    {
        public readonly string Group;
        
        public ParameterGroupAttribute(string group) => Group = group;
    }
    
    public class InlineAttribute : Attribute { }
}