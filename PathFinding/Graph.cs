using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    using Point = Tuple<double, double>;
    public enum SearchType
    {
        BFS, Djikistra, AStar, GreedyBFS
    }
    public sealed class Graph 
    {
        public static Dictionary<SearchType, IPathFinder> SearchAlgorithm = new Dictionary<SearchType, IPathFinder>();
        public Cell[,] CellMap;
        private Cell StartNode;
        private Cell DestinationNode;
        public Graph(Cell[,] cell)
        {
            CellMap = cell;
            AssignNeighbors();
            SearchAlgorithm.Add(SearchType.BFS, new BFS());
            SearchAlgorithm.Add(SearchType.GreedyBFS, new GreedyBest());
            SearchAlgorithm.Add(SearchType.Djikistra, new Dijkstra());
            SearchAlgorithm.Add(SearchType.AStar, new AStar());

        }
        public void Clear()
        {
            Node.HeuristicFunction = null;
            foreach(var cell in CellMap)
            {
                if(cell.Type == NodeType.Normal)
                {
                    cell.CurrentBrush = Cell.DefaultBrush;
                    cell.CellNode.Reset();
                }
            }
        }
        public void AssignNeighbors()
        {
            foreach(var cell in CellMap)
            {
                int r = cell.RowIndex;
                int c = cell.ColIndex;

                int firstNeighborRow = r - 1;
                if(firstNeighborRow >=0 && firstNeighborRow < MainWindow.NUMBER_OF_ROWS)
                {
                    var neighborNode = CellMap[firstNeighborRow, c].CellNode;
                    cell.CellNode.AddNeighbor(neighborNode);
                }
                int secondNeighborRow = r + 1;
                if (secondNeighborRow >= 0 && secondNeighborRow < MainWindow.NUMBER_OF_ROWS)
                {
                    var neighborNode = CellMap[secondNeighborRow, c].CellNode;
                    cell.CellNode.AddNeighbor(neighborNode);
                }
                int firstNeighborCol = c - 1;
                if (firstNeighborCol >= 0 && firstNeighborCol < MainWindow.NUMBER_OF_COLS)
                {
                    var neighborNode = CellMap[r, firstNeighborCol].CellNode;
                    cell.CellNode.AddNeighbor(neighborNode);
                }
                int secondNeighborCol = c + 1;
                if (secondNeighborCol >= 0 && secondNeighborCol < MainWindow.NUMBER_OF_COLS)
                {
                    var neighborNode = CellMap[r, secondNeighborCol].CellNode;
                    cell.CellNode.AddNeighbor(neighborNode);
                }


            }
        }
        public static double Cost(Node source, Node dest)
        {
                return 1.0f;
        }
        public void Reset() {
            foreach(var cell in CellMap)
            {
                cell.Reset();
            }
        }
        public List<Node> FindPath(SearchType type)
        {
            var path = SearchAlgorithm[type].FindPath(this, Cell.SourceCell, Cell.DestinationCell);
            if(path != null)
            {
                path.Reverse();
                return path;
            }
            return null;
        }
    }
}
