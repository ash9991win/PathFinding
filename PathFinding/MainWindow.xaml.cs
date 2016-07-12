using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public static int NUMBER_OF_ROWS = 20;
        public static int NUMBER_OF_COLS = 20;
        public static double TILE_WIDTH;
        public static double TILE_HEIGHT;
        public static Cell[,] Cells;
        public static Grid TileGrid;
        public static Graph Graph;
        public static Stopwatch PathTimer = new Stopwatch();
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            TILE_WIDTH = Width / NUMBER_OF_COLS;
            TILE_HEIGHT = Height / NUMBER_OF_ROWS;
            Width += TILE_WIDTH;
            Height += TILE_HEIGHT;
            TileGrid = new Grid();
            GridWrap.Children.Add(TileGrid);
            PopulateCells();
            Graph = new Graph(Cells);
        }
        public static void PopulateCells()
        {
            TileGrid.RowDefinitions.Clear();
            TileGrid.ColumnDefinitions.Clear();
            TileGrid.Children.Clear();
            Cells = new Cell[NUMBER_OF_ROWS, NUMBER_OF_COLS];
            for(int i =0; i < NUMBER_OF_ROWS; i++)
            {
                for(int j = 0; j < NUMBER_OF_COLS;j++)
                {

                    Cells[i, j] = new Cell(i, j);
                    var CurrentTile = Cells[i, j];
                    CurrentTile.Width = TILE_WIDTH;
                    CurrentTile.Height = TILE_HEIGHT;
                    CurrentTile.SetValue(Grid.ColumnProperty, j);
                    CurrentTile.SetValue(Grid.RowProperty, i);
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(TILE_WIDTH);
                    TileGrid.ColumnDefinitions.Add(col);
                    TileGrid.Children.Add(CurrentTile);
                }
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(TILE_HEIGHT);
                TileGrid.RowDefinitions.Add(row);
            }
        }

        private void ResetTiles(object sender, RoutedEventArgs e)
        {
            foreach(var cell in Cells)
            {
                cell.Reset();
            }
        }

        private void CheckIfPathCanBeFound()
        {
            if (!Cell.IsSourceSet )
                MessageBox.Show("The start node hasnt been selected!");
            if (!Cell.IsDestinationSet)
                MessageBox.Show("The Destination node hasnt been selected!");
        }
        private void FindPathWithBFS(object sender, RoutedEventArgs e)
        {
            Graph.Clear();
            CheckIfPathCanBeFound();
            PathTimer.Restart();
            var path = Graph.FindPath(SearchType.BFS);
            PathTimer.Stop();
            var totalTime = PathTimer.ElapsedMilliseconds;
            if(path != null)
            {
               foreach(var node in path)
                {
                    if (node.Parent != null  && node.Parent.Type == NodeType.Normal)
                    {
                        node.Parent.GetCell().CurrentBrush = Cell.DirectionBrushes[Cell.GetDirectionBetween(node.Parent.GetCell(), node.GetCell())];
                    }
                }
            }
            MessageBox.Show("Total Time : " + totalTime + "ms");
            e.Handled = true;
        }
        private void FindPathWithGreedy(object sender, RoutedEventArgs e)
        {
            Graph.Clear();
            var menuItem = e.Source as MenuItem;
            if(menuItem == Euclidian)
            {
                Node.HeuristicFunction += Heuristics.EuclidianDistance;
            }
            else if(menuItem == Manhattan)
            {
                Node.HeuristicFunction += Heuristics.ManhattanDistance;
            }
            CheckIfPathCanBeFound();
            PathTimer.Restart();
            var path = Graph.FindPath(SearchType.GreedyBFS);
            var totalTime = PathTimer.ElapsedMilliseconds;
            if (path != null)
            {
                foreach (var node in path)
                {
                    if (node.Parent != null && node.Parent.Type == NodeType.Normal)
                    {
                        node.Parent.GetCell().CurrentBrush = Cell.DirectionBrushes[Cell.GetDirectionBetween(node.Parent.GetCell(), node.GetCell())];
                    }
                }
            }
            MessageBox.Show("Total Time: " + totalTime + "ms");
            e.Handled = true;
        }

        private void FindPathWithDjik(object sender, RoutedEventArgs e)
        {
            Graph.Clear();
            CheckIfPathCanBeFound();
            PathTimer.Restart();
            var path = Graph.FindPath(SearchType.Djikistra);
            PathTimer.Stop();
            if (path != null)
            {
                foreach (var node in path)
                {
                    if (node.Parent != null && node.Parent.Type == NodeType.Normal)
                    {
                        node.Parent.GetCell().CurrentBrush = Cell.DirectionBrushes[Cell.GetDirectionBetween(node.Parent.GetCell(), node.GetCell())];
                    }
                }
            }
            MessageBox.Show("Total Time: " + PathTimer.ElapsedMilliseconds + "ms");
            e.Handled = true;
        }

        private void FindPathWithAStar(object sender, RoutedEventArgs e)
        {
            Graph.Clear();
            var menuItem = e.Source as MenuItem;
            if (menuItem == EuclidianAStar)
            {
                Node.HeuristicFunction += Heuristics.EuclidianDistance;
            }
            else if (menuItem == ManhattanAStar)
            {
                Node.HeuristicFunction += Heuristics.ManhattanDistance;
            }
            CheckIfPathCanBeFound();
            PathTimer.Restart();
            var path = Graph.FindPath(SearchType.AStar);
            PathTimer.Stop();
            if (path != null)
            {
                foreach (var node in path)
                {
                    if (node.Parent != null && node.Parent.Type == NodeType.Normal)
                    {
                        node.Parent.GetCell().CurrentBrush = Cell.DirectionBrushes[Cell.GetDirectionBetween(node.Parent.GetCell(), node.GetCell())];
                    }
                }
            }
            MessageBox.Show("Total Time: " + PathTimer.ElapsedMilliseconds + "ms");
            e.Handled = true;
        }
    }
}
