using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.Queues
{
    internal class Request : IEquatable<Request>
    {
        public RequestType RequestType;
        public string RequesterName;
        public string BuffCommand;
        public int RequesterGuid;
        public string Character;
        public List<int> SpellsToCast = new List<int>();
        public double Heading;
        public string Destination;
        public string ItemToUse;

        public bool Equals(Request other)
        {
            return RequestType.Equals(other.RequestType) && RequesterName.Equals(other.RequesterName);
        }
    }

    internal enum RequestType
    {
        Buff,
        Portal,
        Gem
    }
}
