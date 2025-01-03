﻿namespace SnakeGame;

public class GameState
{
    private readonly Random _random = new();
    private readonly LinkedList<Position> _snakePositions = [];
    private readonly LinkedList<Direction> directionChanges = [];

    public GameState(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Grid = new GridValue[Rows, Columns];
        SnakeDirection = Direction.Right;

        AddSnake();
        AddFood();
    }

    public int Rows { get; set; }
    public int Columns { get; set; }
    public GridValue[,] Grid { get; set; }
    public Direction SnakeDirection { get; private set; }
    public int Score { get; private set; }
    public bool GameOver { get; private set; }

    private Position FoodPosition { get; set; }

    private void AddSnake()
    {
        var middleRow = Rows / 2;
        for (var c = 1; c <= 3; c++)
        {
            Grid[middleRow, c] = GridValue.Snake;
            _snakePositions.AddFirst(new Position(middleRow, c));
        }
    }

    private IEnumerable<Position> EmptyPositions()
    {
        for (var r = 0; r < Rows; r++)
        for (var c = 0; c < Columns; c++)
            if (Grid[r, c] == GridValue.Empty)
                yield return new Position(r, c);
    }

    private void AddFood()
    {
        var empty = new List<Position>(EmptyPositions());

        if (empty.Count == 0) return;

        var pos = empty[_random.Next(empty.Count)];
        FoodPosition = pos;
        Grid[pos.Row, pos.Column] = GridValue.Food;
    }

    public Position HeadPosition()
    {
        return _snakePositions.First.Value;
    }

    public Position TailPosition()
    {
        return _snakePositions.Last.Value;
    }

    public IEnumerable<Position> SnakePositions()
    {
        return _snakePositions;
    }

    private void AddHead(Position pos)
    {
        _snakePositions.AddFirst(pos);
        Grid[pos.Row, pos.Column] = GridValue.Snake;
    }

    private void RemoveTail()
    {
        var tail = _snakePositions.Last.Value;
        Grid[tail.Row, tail.Column] = GridValue.Empty;
        _snakePositions.RemoveLast();
    }

    public void ChangeDirection(Direction dir)
    {
        if (CanChangeDirection(dir)) directionChanges.AddLast(dir);
    }

    private bool CanChangeDirection(Direction newDirection)
    {
        if (directionChanges.Count == 2)
            return false;
        var lastDirection = GetLastDirection();
        return newDirection != lastDirection && newDirection != lastDirection.Opposite();
    }

    private Direction GetLastDirection()
    {
        if (directionChanges.Count == 0)
            return SnakeDirection;

        return directionChanges.Last.Value;
    }

    private bool OutsideGrid(Position pos)
    {
        return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
    }

    private GridValue WillHit(Position newHeadPosition)
    {
        if (OutsideGrid(newHeadPosition)) return GridValue.Outside;

        if (newHeadPosition == TailPosition()) return GridValue.Empty;

        return Grid[newHeadPosition.Row, newHeadPosition.Column];
    }

    public void Move()
    {
        if (directionChanges.Count > 0)
        {
            SnakeDirection = directionChanges.First.Value;
            directionChanges.RemoveFirst();
        }

        var previousHeadPosition = HeadPosition();
        var newHeadPos = previousHeadPosition.Translate(SnakeDirection);
        var hit = WillHit(newHeadPos);

        switch (hit)
        {
            case GridValue.Outside:
                HandleBoundaryHit(previousHeadPosition);
                break;
            case GridValue.Snake:
                GameOver = true;
                break;
            case GridValue.Empty:
                RemoveTail();
                AddHead(newHeadPos);
                break;
            case GridValue.Food:
                AddHead(newHeadPos);
                Score++;
                AddFood();
                break;
        }
    }

    private void HandleBoundaryHit(Position previousHeadPosition)
    {
        var newHeadPos = SnakeDirection switch
        {
            { RowOffset: -1, ColumnOffset: 0 } => new Position(Rows - 1, previousHeadPosition.Column),
            { RowOffset: 0, ColumnOffset: 1 } => new Position(previousHeadPosition.Row, 0),
            { RowOffset: 1, ColumnOffset: 0 } => new Position(0, previousHeadPosition.Column),
            { RowOffset: 0, ColumnOffset: -1 } => new Position(previousHeadPosition.Row, Columns - 1),
            _ => previousHeadPosition
        };

        if (newHeadPos == FoodPosition)
        {
            Score++;
            AddFood();
        }

        RemoveTail();
        AddHead(newHeadPos);
    }
}