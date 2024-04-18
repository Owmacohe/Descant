﻿// Please see https://omch.tech/descant/#changedresponse for documentation

using System;
using System.Collections.Generic;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class ChangedResponse : DescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;
        
        [ParameterGroup("Changed value"), NoFiltering] public string ChangeTo;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null) return result;
            
            if ((VariableType.Equals(VariableType.Statistic) && CompareVariable(
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.Topic) && actor.Topics.Contains(VariableName)) ||
                (VariableType.Equals(VariableType.Relationship) && CompareVariable(
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.DialogueAttempts) && CompareVariable(
                    actor.DialogueAttempts, Comparison, ComparisonType)))
            {
                result.Text[0] = new KeyValuePair<int, string>(result.Text[0].Key, ChangeTo);
            }
            
            return result;
        }
    }
}