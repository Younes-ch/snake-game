using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
    {
        { GridValue.Empty, Images.Empty },
        { GridValue.Snake, Images.Body },
        { GridValue.Food, Images.Food },
    };

    private readonly Dictionary<Direction, int> dirToRotation = new()
    {
        { Direction.Up, 0 },
        { Direction.Right, 90 },
        { Direction.Down, 180 },
        { Direction.Left, 270 },
    };

    private const int Rows = 15;
    private const int Cols = 15;
    private readonly Image[,] gridImages;
    private GameState _gameState;
    private bool gameRunning;

    public MainWindow()
    {
        InitializeComponent();
        gridImages = SetupGrid();
        _gameState = new GameState(Rows, Cols);
    }

    private Image[,] SetupGrid()
    {
        var images = new Image[Rows, Cols];
        GameGrid.Rows = Rows;
        GameGrid.Columns = Cols;

        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var image = new Image
                {
                    Source = Images.Empty,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                images[r, c] = image;
                GameGrid.Children.Add(image);
            }
        }

        return images;
    }

    private async Task RunGame()
    {
        Draw();
        await ShowCountDown();
        Overlay.Visibility = Visibility.Hidden;
        await GameLoop();
        await ShowGameOver();
        _gameState = new GameState(Rows, Cols);
    }

    private async void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Overlay.IsVisible) e.Handled = true;

        if (!gameRunning)
        {
            gameRunning = true;
            await RunGame();
            gameRunning = false;
        }
    }

    private void Draw()
    {
        DrawGrid();
        DrawSnakeHead();
        ScoreText.Text = $"SCORE: {_gameState.Score}";
    }

    private void DrawGrid()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var gridValue = _gameState.Grid[r, c];
                gridImages[r, c].Source = gridValToImage[gridValue];
                gridImages[r, c].RenderTransform = Transform.Identity;
            }
        }
    }

    private void DrawSnakeHead()
    {
        var headPosition = _gameState.HeadPosition();
        var image = gridImages[headPosition.Row, headPosition.Column];
        image.Source = Images.Head;

        var rotation = dirToRotation[_gameState.SnakeDirection];
        image.RenderTransform = new RotateTransform(rotation);
    }

    private async Task GameLoop()
    {
        while (!_gameState.GameOver)
        {
            await Task.Delay(100);
            _gameState.Move();
            Draw();
        }
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (_gameState.GameOver) return;

        switch (e.Key)
        {
            case Key.Left:
                _gameState.ChangeDirection(Direction.Left);
                break;
            case Key.Right:
                _gameState.ChangeDirection(Direction.Right);
                break;
            case Key.Up:
                _gameState.ChangeDirection(Direction.Up);
                break;
            case Key.Down:
                _gameState.ChangeDirection(Direction.Down);
                break;
        }
    }

    private async Task ShowCountDown()
    {
        for (var i = 3; i >= 1; i--)
        {
            OverylayText.Text = i.ToString();
            await Task.Delay(500);
        }
    }

    private async Task ShowGameOver()
    {
        await Task.Delay(1000);
        OverylayText.Text = "PRESS ANY KEY TO START";
        Overlay.Visibility = Visibility.Visible;
    }
}