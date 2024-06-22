using System.Collections.Concurrent;
using ChatService.Interfaces;

namespace ChatService.Services;

public class SharedMemoryConnectionsDB: ISharedMemoryConnectionDB
{
    private readonly ConcurrentDictionary<string, string> _connections = new();
    public ConcurrentDictionary<string, string> Connections => _connections;
}