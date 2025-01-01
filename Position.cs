namespace SnakeGame;

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Position Translate(Direction dir)
    {
        return new Position(Row + dir.RowOffset, Column + dir.ColumnOffset);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Position pos
            && Row == pos.Row
            && Column == pos.Column;
    }

    public static bool operator ==(Position left, Position right)
    {
        return EqualityComparer<Position>.Default.Equals(left, right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !(left == right);
    }
}