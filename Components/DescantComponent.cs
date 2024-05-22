using System;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    /// <summary>
    /// Descant node Component parent class
    /// </summary>
    [Serializable]
    public abstract class DescantComponent
    {
        /// <summary>
        /// Whether this Component is currently collapsed in the UI
        /// </summary>
        [HideInInspector] public bool Collapsed;

        /// <summary>
        /// Called when the node to which this Component is attached is reached in the dialogue
        /// </summary>
        /// <param name="result">The state of the text, actors, and portraits when the Component is Invoked</param>
        /// <returns>The state of the text, actors, and portraits after the Component is Invoked</returns>
        public virtual DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            return result;
        }
        
        /// <summary>
        /// Called when FixedUpdate occurs in the DescantDialogueController
        /// </summary>
        /// <returns>Whether or not to stop the dialogue immediately</returns>
        public virtual bool FixedUpdate()
        {
            return true;
        }
        
        /// <summary>
        /// Called when Update occurs in the DescantDialogueController
        /// </summary>
        /// <returns>Whether or not to stop the dialogue immediately</returns>
        public virtual bool Update()
        {
            return true;
        }

        /// <summary>
        /// Overridden Equals method
        /// </summary>
        /// <param name="other">The object being compared against</param>
        /// <returns>Whether the other object has the same data as this one</returns>
        public override bool Equals(object other)
        {
            try
            {
                _ = (DescantComponent) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantComponent)other);
        }

        public override int GetHashCode()
        {
            return Collapsed.GetHashCode();
        }

        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantComponent has the same data as this one</returns>
        public bool Equals(DescantComponent other)
        {
            return ToString().Equals(other.ToString());
        }
        
        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string temp = "";
            
            foreach (var i in GetType().GetFields())
                temp += " " + i.GetValue(this);

            return GetType() + " (" + (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }
    
    /// <summary>
    /// A DescantComponent class attribute to indicate the maximum number of
    /// Components of the attached type that can be added to a DescantNode
    /// </summary>
    public class MaxQuantityAttribute : Attribute
    {
        public readonly float Quantity;
        
        public MaxQuantityAttribute(float quantity) => Quantity = quantity;
    }

    /// <summary>
    /// A DescantComponent class attribute to indicate which type(s) of
    /// DescantNodes that the attached type can be added to
    /// </summary>
    public class NodeTypeAttribute : Attribute
    {
        public readonly DescantNodeType Type;
        
        public NodeTypeAttribute(DescantNodeType type) => Type = type;
    }

    /// <summary>
    /// A DescantComponent class attribute to indicate whether to show the
    /// component in the DescantNodes' component dropdowns
    /// </summary>
    public class DontShowInEditorAttribute : Attribute { }
    
    /// <summary>
    /// A DescantComponent property attribute to indicate that the attached parameter
    /// should be rendered inline with the name in the Descant Graph GUI
    /// </summary>
    public class InlineAttribute : Attribute { }
    
    /// <summary>
    /// A DescantComponent property attribute to indicate to which group/row
    /// the attached parameter should be rendered in the Descant Graph GUI
    /// </summary>
    public class ParameterGroupAttribute : Attribute
    {
        public readonly string Group;
        
        public ParameterGroupAttribute(string group) => Group = group;
    }
    
    /// <summary>
    /// A DescantComponent property attribute to indicate that the attached
    /// parameter should not be filtered in the Descant Graph GUI
    /// </summary>
    public class NoFilteringAttribute : Attribute { }
}