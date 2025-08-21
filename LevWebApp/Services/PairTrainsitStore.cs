using System.Collections.Concurrent;
using LevWebApp.Models;

namespace LevWebApp.Services
{
    // Simple in-memory store for passing NamePair objects across pages/tabs.
    // NOTE: Singleton lifetime so it works across Blazor Server circuits.
    public class PairTransitStore
    {
        private readonly ConcurrentDictionary<string, NamePair> _map = new();
        private readonly ConcurrentDictionary<string, DateTimeOffset> _ttl = new();

        public string Put(NamePair pair, TimeSpan? keepAlive = null)
        {
            var id = Guid.NewGuid().ToString("N");
            _map[id] = pair;
            _ttl[id] = DateTimeOffset.UtcNow + (keepAlive ?? TimeSpan.FromMinutes(20));
            return id;
        }

        public bool TryGet(string id, out NamePair? pair)
        {
            pair = null;
            if (!_map.TryGetValue(id, out var p)) return false;

            // Expire old entries (basic TTL)
            if (_ttl.TryGetValue(id, out var until) && until < DateTimeOffset.UtcNow)
            {
                _map.TryRemove(id, out _);
                _ttl.TryRemove(id, out _);
                return false;
            }

            pair = p;
            return true;
        }
    }
}
