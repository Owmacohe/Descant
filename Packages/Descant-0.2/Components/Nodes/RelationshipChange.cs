using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class RelationshipChange :
        DescantNodeComponent, IInvokedDescantComponent,
        IChoiceNodeComponent, IResponseNodeComponent, IStartNodeComponent, IEndNodeComponent
    {
        public DescantActor Actor;
        public string Name;
        public ValueChangeType ChangeType;
        public float Change;

        public RelationshipChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            DescantActor actor, string name, ValueChangeType changeType, float change)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            Actor = actor;
            Name = name;
            ChangeType = changeType;
            Change = change;
        }

        public void Invoke()
        {
            switch (ChangeType)
            {
                case ValueChangeType.IncreaseBy:
                    Actor.SetRelationship(Name, Actor.Relationships[Name] + Change);
                    break;
                
                case ValueChangeType.DecreaseBy:
                    Actor.SetRelationship(Name, Actor.Relationships[Name] - Change);
                    break;
                
                case ValueChangeType.Set:
                    Actor.SetRelationship(Name, Change);
                    break;
            }
        }
    }
}