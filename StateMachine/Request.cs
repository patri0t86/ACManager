using Decal.Adapter;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine
{
    public class Request : IEquatable<Request>
    {
        public RequestType RequestType;
        public string RequesterName = "";
        public int RequesterGuid;
        public string Character = CoreManager.Current.CharacterFilter.Name;
        public List<Spell> SpellsToCast = new List<Spell>();
        public double Heading = -1;
        public string Destination;
        public string ItemToUse;

        public bool Equals(Request other)
        {
            return RequestType.Equals(other.RequestType) && RequesterName.Equals(other.RequesterName);
        }
    }

    public enum RequestType
    {
        Buff,
        Portal,
        Gem
    }
}
