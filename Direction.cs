namespace SnakeGame;

public class Direction
{
    public static readonly Direction Left = new(0, -1);
    public static readonly Direction Right = new(0, 1);
    public static readonly Direction Up = new(-1, 0);
    public static readonly Direction Down = new(1, 0);
    public int RowOffset { get; set; }
    public int ColumnOffset { get; set; }

    private Direction(int rowOffset, int columnOffset)
    {
        RowOffset = rowOffset;
        ColumnOffset = columnOffset;
    }

    public Direction Opposite()
    {
        return new Direction(-RowOffset, -ColumnOffset);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RowOffset, ColumnOffset);
    }

    public override bool Equals(object? other)
    {
        return other is Direction direction
               && RowOffset == direction.RowOffset
               && ColumnOffset == direction.ColumnOffset;
    }

    public static bool operator ==(Direction? left, Direction? right)
    {
        return EqualityComparer<Direction>.Default.Equals(left, right);
    }

    public static bool operator !=(Direction? left, Direction? right)
    {
        return !(left == right);
    }
}