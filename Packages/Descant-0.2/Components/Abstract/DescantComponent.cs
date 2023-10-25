using System;
using Runtime;

namespace Components
{
    [Serializable]
    public abstract class DescantComponent
    {
        public int ID;
        public float MaxQuantity;
        protected DescantConversationController Controller;
        
        protected DescantComponent(DescantConversationController controller, int id, float max)
        {
            ID = id;
            MaxQuantity = max;
            Controller = controller;
        }

        public override string ToString()
        {
            return GetType() + "(" + ID + ")";
        }
    }
}