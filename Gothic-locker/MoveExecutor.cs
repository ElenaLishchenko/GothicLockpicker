public sealed class MoveExecutor : IMoveExecutor {
    public bool TryApply(
        PuzzleState state,
        IReadOnlyCollection<PlayerMove> movements,
        out PuzzleState nextState) {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(movements);

        foreach (var move in movements) {
            var currentPosition =
                state.Positions[move.LockIndex];

            var nextPosition =
                currentPosition + (int)move.Direction;

            if (nextPosition < 0 ||
                nextPosition >= PuzzleDefinition.HoleCount) {
                nextState = null!;
                return false;
            }
        }

        var positions = state.Positions.ToArray();

        foreach (var move in movements) {
            positions[move.LockIndex] +=
                (int)move.Direction;
        }

        nextState = new PuzzleState(positions);

        return true;
    }
}
