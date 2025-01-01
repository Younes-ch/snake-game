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
    private readonly int _rows = 15, _cols = 15;
    private readonly Image[,] gridImages;
    
    public MainWindow()
    {
        InitializeComponent();
        gridImages = SetupGrid();
    }

    private Image[,] SetupGrid()
    {
        var images = new Image[_rows, _cols];
        GameGrid.Rows = _rows;
        GameGrid.Columns = _cols;

        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _cols; c++)
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
}