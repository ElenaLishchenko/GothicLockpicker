public interface IPuzzleSolver {
    IReadOnlyList<SolutionStep>? Solve(
        PuzzleDefinition definition,
        PuzzleState initialState);
}