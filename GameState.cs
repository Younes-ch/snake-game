﻿namespace SnakeGame;

public class GameState
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public GridValue[,] Grid { get; set; }
    public Direction SnakeDirection { get; private set; }
    public int Score { get; private set; }
    public bool GameOver { get; private set; }

    private readonly LinkedList<Position> _snakePositions = [];
    private readonly Random _random = new();

    public GameState(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Grid = new GridValue[Rows, Columns];
        SnakeDirection = Direction.Right;
        
        AddSnake();
        AddFood();
    }

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
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (Grid[r, c] == GridValue.Empty)
                {
                    yield return new Position(r, c);
                }
            }
        }
    }
    private void AddFood()
    {
        var empty = new List<Position>(EmptyPositions());

        if (empty.Count == 0)
        {
            return;
        }

        var pos = empty[_random.Next(empty.Count)];
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
        SnakeDirection = dir;
    }

    private bool OutsideGrid(Position pos)
    {
        return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
    }

    private GridValue WillHit(Position newHeadPosition)
    {
        if (OutsideGrid(newHeadPosition))
        {
            return GridValue.Outside;
        }

        if (newHeadPosition == TailPosition())
        {
            return GridValue.Empty;
        }
        
        return Grid[newHeadPosition.Row, newHeadPosition.Column];
    }

    public void Move()
    {
        var newHeadPos = HeadPosition().Translate(SnakeDirection);
        var hit = WillHit(newHeadPos);
        
        if (hit is GridValue.Snake or GridValue.Outside)
        {
            GameOver = true;
        } 
        else if (hit == GridValue.Empty)
        {
            RemoveTail();
            AddHead(newHeadPos);
        }
        else if (hit == GridValue.Food)
        {
            AddHead(newHeadPos);
            Score++;
            AddFood();
        }
    }
}