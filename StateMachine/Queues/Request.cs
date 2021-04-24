using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.Queues
{
    internal class Request : IEquatable<Request>
    {
        public RequestType RequestType;
        public string RequesterName = "";
        public int RequesterGuid;
        public string Character;
        public List<Spell> SpellsToCast = new List<Spell>();
        public double Heading = -1;
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
