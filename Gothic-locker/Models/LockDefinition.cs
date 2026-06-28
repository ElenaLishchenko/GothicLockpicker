public sealed class LockDefinition {
    /// <summary>
    /// Lock identifier.
    /// </summary>
    public required int LockIndex { get; init; }

    /// <summary>
    /// Other locks affected by this lock.
    /// Self influence is not allowed.
    /// </summary>
    public required IReadOnlyCollection<LockMove> Moves {
        get;
        init;
    }
}