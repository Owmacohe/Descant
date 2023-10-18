using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class RelationshipChange : DescantNodeComponent, IInvokedDescantComponent
    {
        public DescantActor Actor { get; }
        public string Relationship { get; }
        public ValueChangeType Type { get; }
        public float Change { get; }

        public RelationshipChange(DescantConversationController controller, int id, DescantActor actor, string relationship, ValueChangeType type, float change)
            : base(controller, id, DescantNodeType.Any, float.PositiveInfinity)
        {
            Actor = actor;
            Relationship = relationship;
            Type = type;
            Change = change;
        }

        public void Invoke()
        {
            switch (Type)
            {
                case ValueChangeType.IncreaseBy:
                    Actor.SetRelationship(Relationship, Actor.Statistics[Relationship] + Change);
                    break;
                
                case ValueChangeType.DecreaseBy:
                    Actor.SetRelationship(Relationship, Actor.Statistics[Relationship] - Change);
                    break;
                
                case ValueChangeType.Set:
                    Actor.SetRelationship(Relationship, Change);
                    break;
            }
        }
    }
}