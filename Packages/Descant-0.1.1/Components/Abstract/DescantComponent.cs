using System;
using UnityEngine;

namespace DescantComponents
{
    [Serializable]
    public abstract class DescantComponent
    {
        public int ID;

        protected DescantComponent(int id) // TODO: find a way to have components interact with the controller at runtime
        {
            ID = id;
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