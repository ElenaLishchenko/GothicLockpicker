public interface IMoveExecutor {
    bool TryApply(
        PuzzleState state,
        IReadOnlyCollection<PlayerMove> movements,
        out PuzzleState nextState);
}