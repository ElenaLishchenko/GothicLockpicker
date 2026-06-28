public sealed class PuzzleDefinition {
    /// <summary>
    /// Number of holes in every lock.
    /// </summary>
    public const int HoleCount = 7;

    /// <summary>
    /// Center hole position.
    /// Puzzle is solved when all locks are here.
    /// </summary>
    public const int TargetPosition = 3;

    /// <summary>
    /// Total number of locks in puzzle.
    /// Maximum supported value is 10.
    /// </summary>
    public required int LockCount { get; init; }

    /// <summary>
    /// Influence definitions for every lock.
    /// </summary>
    public required IReadOnlyCollection<LockDefinition> Locks {
        get;
        init;
    }
}