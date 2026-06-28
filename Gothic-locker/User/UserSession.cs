public sealed class UserSession {
    public SessionStep Step { get; set; }

    public int LockCount { get; set; }

    public int CurrentLockIndex { get; set; }

    public List<LockDefinition> Locks { get; } = [];

    public List<LockMove> CurrentMoves { get; } = [];
}