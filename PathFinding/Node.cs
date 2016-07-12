using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    using Point = Tuple<double, double>;

    public enum NodeType
    {
        Normal,Obstacle,Start,Goal,Path
    }
    public enum Direction
    {
        UP,DOWN,LEFT,RIGHT
    }
    public sealed class Node : IComparable
    {
        public  delegate double HeuristicFunctionDelegate(Node A, Node B);

        public static HeuristicFunctionDelegate HeuristicFunction;
        public NodeType Type { get { return mType; }  set { mType = value; } }
        public Point Location { get { return mLocation; } private set { mLocation = value; } }
        public double PathCost { get { return mPathCost; }  set { mPathCost = value; } }
        public double TotalCost { get { return mPathCost + mHeuristic; }  set { mTotalCost = value; } }
        public List<Node> Neighbors {  get { return mNeighbors; }  private set { mNeighbors = value; } }
        public void AddNeighbor(Node neighbor)
        {
            if (Neighbors == null)
                Neighbors = new List<Node>();
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
            }
        }
        
        public Node Parent { get { return mParent; } set { mParent = value; } }
     
        
        public Node(Point point,NodeType type = NodeType.Normal)
        {
            Location = point;
            Type = type;
            Reset();
        }
        public void MarkVisited()
        {
            mVisited = true;
        }
        public void AssignCell(Cell cell)
        {
            CellInWindow = cell;
        }
        public Cell GetCell()
        {
            return CellInWindow;
        }
        public bool IsVisited()
        {
            return mVisited;
        }
        public void Reset() { mHeuristic = mPathCost = 0.0f; mVisited = false; shouldCompareByCost = false; }
        public double GetHeuristic(Node node)
        {
            mHeuristic =  HeuristicFunction.Invoke(this, node);
            return mHeuristic;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Node otherNode = obj as Node;
            if (otherNode != null)
            {
                return this.GetCell().CompareTo(otherNode.GetCell());
            }
            else
                throw new Exception("Object is NOT a node");
        }
        public static bool shouldCompareByCost = false;
        Guid mID;
        NodeType mType;
        Point mLocation;
        double mPathCost;
        double mTotalCost;
        public double mHeuristic;
        bool mVisited;
        Cell CellInWindow = null;
        List<Node> mNeighbors;
        Node mParent;
    }
}
