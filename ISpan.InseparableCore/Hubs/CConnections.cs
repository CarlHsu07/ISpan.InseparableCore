using System.Collections.Concurrent;

namespace ISpan.InseparableCore.Hubs
{
    public class CConnections
    {
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public void AddConnection(string connectionId, string memberId)
        {
            _connections.TryAdd(connectionId, memberId);
        }

        public void RemoveConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }

        public string GetMemberId(string connectionId)
        {
            _connections.TryGetValue(connectionId, out string memberId);
            return memberId;
        }
    }
}
