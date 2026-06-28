using System.Collections.Concurrent;

public static class SessionStorage {
    public static ConcurrentDictionary<long, UserSession>
        Sessions { get; }
            = new();
}