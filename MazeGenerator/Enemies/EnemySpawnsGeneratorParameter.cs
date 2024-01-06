using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Drawing;

namespace MazeGeneration.Enemies;

public class EnemySpawnsGeneratorParameter
{
    private readonly GenerationRandom _random;
    private readonly FrozenDictionary<EnemyType, int> _costs;
    private readonly FrozenDictionary<EnemyType, double> _probabilities;
    private readonly FrozenDictionary<EnemyType, Point> _sizes;

    public EnemySpawnsGeneratorParameter(
        GenerationRandom random,
        Dictionary<EnemyType, int> costs,
        Dictionary<EnemyType, int> frequencies,
        Dictionary<EnemyType, Point> sizes)
    {
        _random = random;
        _costs = costs.ToFrozenDictionary();
        var sumFrequencies = frequencies.Values.Sum();
        _probabilities = frequencies.ToFrozenDictionary(
            kv => kv.Key,
            kv => (double)kv.Value / sumFrequencies);

        _sizes = sizes.ToFrozenDictionary();
    }

    public bool NextEnemy(int remainingCost, out EnemyType result)
    {
        var availableEnemies = _probabilities
            .Where(kv => GetCost(kv.Key) <= remainingCost);
        if (!availableEnemies.Any())
        {
            result = default;
            return false;
        }
        var maxProbability = availableEnemies.Sum(kv => kv.Value);
        var initialRandomNumber = _random.NextLessOrEqual(maxProbability);

        var randomNumber = initialRandomNumber;
        foreach (var (enemyType, probability) in availableEnemies.OrderByDescending(kv => kv.Value))
        {
            if (probability >= randomNumber)
            {
                result = enemyType;
                return true;
            }
            randomNumber -= probability;
        }

        throw new Exception($"Can't choose enemy with random number {initialRandomNumber:F15}");
    }

    public int GetCost(EnemyType type)
    {
        return _costs[type];
    }

    public Point GetSize(EnemyType type)
    {
        return _sizes[type];
    }
}
