public sealed class PuzzleDefinitionValidator {
    public void Validate(
        PuzzleDefinition definition) {
        ArgumentNullException.ThrowIfNull(definition);

        if (definition.LockCount <= 0) {
            throw new InvalidOperationException(
                "LockCount must be greater than zero.");
        }

        if (definition.Locks.Count != definition.LockCount) {
            throw new InvalidOperationException(
                "Lock definitions count does not match LockCount.");
        }

        ValidateLockDefinitions(definition);
    }

    private static void ValidateLockDefinitions(
        PuzzleDefinition definition) {
        var duplicateLockDefinitions =
            definition.Locks
                .GroupBy(x => x.LockIndex)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

        if (duplicateLockDefinitions.Length > 0) {
            throw new InvalidOperationException(
                $"Duplicate lock definitions found: {string.Join(", ", duplicateLockDefinitions)}.");
        }

        foreach (var lockDefinition in definition.Locks) {
            ValidateLockDefinition(
                definition,
                lockDefinition);
        }
    }

    private static void ValidateLockDefinition(
        PuzzleDefinition definition,
        LockDefinition lockDefinition) {
        if (lockDefinition.LockIndex < 0 ||
            lockDefinition.LockIndex >= definition.LockCount) {
            throw new InvalidOperationException(
                $"Invalid lock index '{lockDefinition.LockIndex}'.");
        }

        var duplicateInfluences =
            lockDefinition.Moves
                .GroupBy(x => x.LockIndex)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

        if (duplicateInfluences.Length > 0) {
            throw new InvalidOperationException(
                $"Lock {lockDefinition.LockIndex} contains duplicate influences.");
        }

        foreach (var move in lockDefinition.Moves) {
            if (move.LockIndex < 0 ||
                move.LockIndex >= definition.LockCount) {
                throw new InvalidOperationException(
                    $"Lock {lockDefinition.LockIndex} references invalid lock index '{move.LockIndex}'.");
            }

            if (move.LockIndex == lockDefinition.LockIndex) {
                throw new InvalidOperationException(
                    $"Lock {lockDefinition.LockIndex} cannot influence itself.");
            }
        }
    }
}
