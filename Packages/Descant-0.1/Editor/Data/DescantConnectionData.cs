using System;

namespace Editor.Data
{
    [Serializable]
    public class ConnectionData
    {
        public string From;
        public int FromID;
        public string To;
        public int ToID;
        public int ChoiceIndex;

        public ConnectionData(string from, int fromID, string to, int toID, int choiceIndex = 0)
        {
            From = from;
            FromID = fromID;
            To = to;
            ToID = toID;
            ChoiceIndex = choiceIndex;
        }

        public bool Equals(ConnectionData connection)
        {
            return
                From == connection.From && FromID == connection.FromID &&
                To == connection.To && ToID == connection.ToID &&
                ChoiceIndex == connection.ChoiceIndex;
        }
    }
}