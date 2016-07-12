using System;
using System.Collections.Generic;
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
    /// Interaction logic for Cell.xaml
    /// </summary>
    /// 
    using Point = Tuple<double, double>;
   
    public partial class Cell : UserControl,IComparable
    {
        public int RowIndex;
        public int ColIndex;
        public bool IsSet;
        public Node CellNode;
        public static NodeType CurrentSelectedType;
        private Brush CurrentBrushProperty;
        public Brush CurrentBrush
        {
            get
            {
                return CurrentBrushProperty;
            }
            set
            {
                CurrentBrushProperty = value;
               CellImage.Fill = value ;
            }
        }
        public static SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.White);
        public static SolidColorBrush VisitedBrush = new SolidColorBrush(Colors.Yellow);
        public static ImageBrush UpBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Up.png", UriKind.Relative)));
        public static ImageBrush DownBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Down.png", UriKind.Relative)));
        public static ImageBrush LeftBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Left.png", UriKind.Relative)));
        public static ImageBrush RightBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Right.png", UriKind.Relative)));
        public static ImageBrush StartBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Start.jpg", UriKind.Relative)));
        public static ImageBrush GoalBrush = new ImageBrush(new BitmapImage(new Uri("Resources/Exit.jpg", UriKind.Relative)));
        public static SolidColorBrush ObstacleBrush = new SolidColorBrush(Colors.Gray);

        public static Dictionary<Direction, Brush> DirectionBrushes = new Dictionary<Direction, Brush>();
        private NodeType TypeProperty;
        public static bool IsClicked;
        public NodeType Type { get { return TypeProperty; } set
            {
                TypeProperty = value;
                CellNode.Type = value;
                switch(value)
                {
                    case NodeType.Normal: 
                        {
                            CurrentBrush = DefaultBrush;
                            break;
                        }
                    case NodeType.Obstacle:
                        {
                            CurrentBrush = ObstacleBrush;
                            break;
                        }
                 
                    case NodeType.Start:
                        {
                            CurrentBrush = StartBrush;
                            break;
                        }
                    case NodeType.Goal:
                        {
                            CurrentBrush = GoalBrush;
                            break;
                        }
                }
            } }
        public static Direction GetDirectionBetween(Cell A,Cell B)
        {
            if (A.RowIndex == B.RowIndex)
            {
                if (A.ColIndex > B.ColIndex)
                    return Direction.LEFT;
                else
                    return Direction.RIGHT;

            }
            else if (A.ColIndex == B.ColIndex)
            {
                if (A.RowIndex > B.RowIndex)
                    return Direction.UP;
                else
                    return Direction.DOWN;
            }
            else
                return Direction.LEFT;
        }

        public static bool IsSourceSet;
        public static Cell SourceCell;
        public static bool IsDestinationSet;
        public static Cell DestinationCell;
        private ContextMenu ResetMenu;
        public Cell()
            :this(0,0)
        {

        }
        public Cell(int r,int c)
        {
            RowIndex = r;
            ColIndex = c;
            
            InitializeComponent();

            CellNode = new Node(GetLocationFromIndex(r, c));
            CellNode.AssignCell(this);
            Type = NodeType.Normal;

            if (DirectionBrushes.Count == 0)
            {
                DirectionBrushes.Add(Direction.UP, UpBrush);
                DirectionBrushes.Add(Direction.DOWN, DownBrush);
                DirectionBrushes.Add(Direction.LEFT, LeftBrush);
                DirectionBrushes.Add(Direction.RIGHT, RightBrush);
            }
            MainWindow.instance.StartMenu.Click += StartBrushSelected;
            MainWindow.instance.ObstacleMenu.Click += ObstacleBrushSelected;
            MainWindow.instance.DestinationMenu.Click += GoalBrushSelected;

            ResetMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "Reset Tile";
            item.Click += (object o, RoutedEventArgs e) => { Reset(); };
            ResetMenu.Items.Add(item);
            
        }
        public static Point GetLocationFromIndex(int r,int c)
        {
            double windowWidth = MainWindow.instance.Width;
            double windowHeight = MainWindow.instance.Height;

            double resultX = (r) * (r + 1) * MainWindow.TILE_HEIGHT / 2;
            double resultY =  (c * (c + 1) * MainWindow.TILE_WIDTH / 2);

            //get x 
            return new Point(resultX, resultY);
        }
        public void Reset()
        {
            Type = NodeType.Normal;
           // IsSourceSet = false;
            IsSet = false;
            IsClicked = false;
          //  IsDestinationSet = false;
            CellNode.Reset();
        }
        public void MarkVisited()
        {
            if(Type == NodeType.Normal)
            CurrentBrush = VisitedBrush;
            CellNode.MarkVisited();
        }
        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            Cell cell = e.Source as Cell;
            if(cell != null)
            {
                IsClicked = true;

                if (!IsSourceSet)
                {
                    Type = CurrentSelectedType;
                    if (Type == NodeType.Start)
                    {
                        IsSourceSet = true;
                        SourceCell = this;
                    }
                }
                else if(CurrentSelectedType == NodeType.Start)
                {
                    if (SourceCell != null)
                        SourceCell.Reset();
                    SourceCell = this;
                    IsSourceSet = false;
                }
                else if(!IsDestinationSet)
                {
                    Type = CurrentSelectedType;
                    if(Type == NodeType.Goal)
                    {
                        IsDestinationSet = true;
                        DestinationCell = this;
                    }
                }
                else if(CurrentSelectedType == NodeType.Goal)
                {
                    if (DestinationCell != null)
                        DestinationCell.Reset();
                    DestinationCell = this;
                    IsDestinationSet = false;
                }
                else
                {
                    Type = CurrentSelectedType;
                }
                e.Handled = true;
            }
        }
 
        public void StartBrushSelected(object sender, RoutedEventArgs e)
        {
            CurrentSelectedType = NodeType.Start;
        }
        public void GoalBrushSelected(object sender, RoutedEventArgs e)
        {
            CurrentSelectedType = NodeType.Goal;
        }
        public void ObstacleBrushSelected(object sender, RoutedEventArgs e)
        {
            CurrentSelectedType = NodeType.Obstacle;
        }

        private void MouseEnterEvent(object sender, MouseEventArgs e)
        {
            Cell cell = e.Source as Cell;
            if (cell != null)
            {
                if (IsClicked && (CurrentSelectedType != NodeType.Goal) && (CurrentSelectedType != NodeType.Start))
                {
                    Type = CurrentSelectedType;
                }
                e.Handled = true;
            }
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cell = e.Source as Cell;
            if(cell != null)
            {
                IsClicked = false;
                e.Handled = true;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Cell otherCell = obj as Cell;
            if(otherCell != null)
            {
                if (this.RowIndex == otherCell.RowIndex)
                    if (this.ColIndex == otherCell.ColIndex)
                        return 0;
                return this.RowIndex.CompareTo(otherCell.RowIndex);
            }
            else
            {
                throw new Exception("Other object is NOT a cell");
            }
        }

        private void ResetTile(object sender, MouseButtonEventArgs e)
        {
            Cell cell = e.Source as Cell;
            if(cell != null)
            {
                ResetMenu.IsOpen = true;
            }
        }
    }
}
