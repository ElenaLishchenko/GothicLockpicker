public record PuzzleState(
    IReadOnlyList<int> Positions) {
    public bool IsSolved =>
        Positions.All(p => p == PuzzleDefinition.TargetPosition);
}
