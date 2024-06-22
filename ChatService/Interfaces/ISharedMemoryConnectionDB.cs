using System.Collections.Concurrent;

namespace ChatService.Interfaces;

public interface ISharedMemoryConnectionDB
{
    ConcurrentDictionary<string, string> Connections { get; }
}