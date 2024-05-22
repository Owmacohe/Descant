using System;
using Descant.Utilities;

namespace Descant.Components
{
    /// <summary>
    /// Custom Component used for displaying the DescantIfNode's check
    /// (should never be added as an actual node component as it doesn't do anything)
    /// </summary>
    [Serializable, MaxQuantity(0), NodeType(DescantNodeType.If), DontShowInEditor]
    public class IfComponent : DescantComponent
    {
        public DescantActor Actor;
        
        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;

        /// <summary>
        /// Method to make the comparison
        /// </summary>
        /// <returns>Whether the DescantIfNode's comparison passes or fails</returns>
        public bool Check()
        {
            return (VariableType.Equals(VariableType.Statistic) && DescantComponentUtilities.CompareVariable(
                       Actor.Statistics[VariableName], Comparison, ComparisonType)) ||
                   (VariableType.Equals(VariableType.Topic) && Actor.Topics.Contains(VariableName)) ||
                   (VariableType.Equals(VariableType.Relationship) && DescantComponentUtilities.CompareVariable(
                       Actor.Relationships[VariableName], Comparison, ComparisonType)) ||
                   (VariableType.Equals(VariableType.DialogueAttempts) && DescantComponentUtilities.CompareVariable(
                       Actor.DialogueAttempts, Comparison, ComparisonType));
        }
    }   
}