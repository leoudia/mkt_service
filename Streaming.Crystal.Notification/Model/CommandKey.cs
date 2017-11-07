using Streaming.Crystal.Model.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Notification.Model
{
    public class CommandKey : IEquatable<CommandKey>
    {
        private MarketMessageType type;
        private string id;

        public CommandKey(MarketMessageType type, string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("'id' is not null");

            this.type = type;
            this.id = id;
        }

        public bool Equals(CommandKey other)
        {
            return id.Equals(other.id) && type.Equals(other.type);
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() + id.GetHashCode();
        }
    }
}
