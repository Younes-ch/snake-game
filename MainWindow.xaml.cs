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

    private const int Rows = 30;
    private const int Cols = 30;
    private readonly Image[,] gridImages;
    private GameState _gameState;
    
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
                    Source = Images.Empty
                };

                images[r, c] = image;
                GameGrid.Children.Add(image);
            }
        }

        return images;
    }
    
    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        Draw();
        await GameLoop();
    }
    
    private void Draw()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var gridValue = _gameState.Grid[r, c];
                gridImages[r, c].Source = gridValToImage[gridValue];
            }
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

    private async Task GameLoop()
    {
        while (!_gameState.GameOver)
        {
            await Task.Delay(100);
            _gameState.Move();
            Draw();
        }
    }
}