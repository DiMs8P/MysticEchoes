using System.Drawing;

namespace MazeGeneration;

public class GenerationRandom : Random
{
    private List<Point> CardinalDirections = new()
    {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1),
    };

    public GenerationRandom(int seed)
        : base(seed)
    {

    }

    public bool Chance(double chance)
    {
        if (chance is < 0 or > 1)
        {
            throw new ArgumentException();
        }

        return chance <= NextDouble();
    }

    public int NextBetween(int a, int b)
    {
        return Next(int.Min(a, b), int.Max(a, b));
    }

    public double GetDivideProportion(double maxShift)
    {
        return 0.5 - maxShift + NextDouble() * maxShift * 2;
    }

    public Point NextInRectangle(Rectangle rect)
    {
        return new Point(
            NextBetween(rect.Left, rect.Right),
            NextBetween(rect.Bottom, rect.Top)
        );
    }

    public Point NextCardinalDirection()
    {
        return CardinalDirections[Next(0, CardinalDirections.Count)];
    }
}