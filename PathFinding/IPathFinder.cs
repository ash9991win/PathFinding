using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public interface IPathFinder
    {
         List<Node> FindPath(Graph graph,Cell start,Cell dest);
    }
    public class Heuristics
    {
        public static double EuclidianDistance(Node A, Node B)
        {
            double XSquared = (B.Location.Item1 - A.Location.Item1) * (B.Location.Item1 - A.Location.Item1);
            double YSquared = (B.Location.Item2 - A.Location.Item2) * (B.Location.Item2 - A.Location.Item2);

            return Math.Sqrt(XSquared + YSquared);
        }
        public static double ManhattanDistance(Node A, Node B)
        {
            return (Math.Abs(A.Location.Item1 - B.Location.Item1) + Math.Abs(A.Location.Item2 - B.Location.Item2));
        }
    }
    public class BFS : IPathFinder
    {
        public List<Node> FindPath(Graph graph, Cell start, Cell dest)
        {
            List<Node> pathToReturn = new List<Node>();
            foreach(var cell in graph.CellMap)
            {
                cell.CellNode.Reset();
            }
            start.MarkVisited();
            start.CellNode.Parent = null;
            Queue<Node> Q = new Queue<Node>();
            Q.Enqueue(start.CellNode);
            bool isPathFound = false;
            while(Q.Count != 0)
            {
                var current = Q.Dequeue();
                if (current == dest.CellNode)
                {
                    isPathFound = true;
                    break;
                }
                foreach(var neighbor in current.Neighbors)
                {
                    if (!neighbor.IsVisited() && neighbor.Type != NodeType.Obstacle)
                    {
                        Q.Enqueue(neighbor);
                        neighbor.MarkVisited();
                        neighbor.GetCell().MarkVisited();
                        neighbor.Parent = current;
                    }

                }
            }
            if(isPathFound)
            {
                var currentNode = dest.CellNode;
                while(currentNode.Parent != null)
                {
                    pathToReturn.Add(currentNode);
                    currentNode = currentNode.Parent;
                }
            }
            return pathToReturn;
        }
    }
    public class GreedyBest : IPathFinder
    {
        public List<Node> FindPath(Graph graph, Cell start, Cell dest)
        {
            List<Node> pathToReturn = new List<Node>();
            List<Node> ClosedSet = new List<Node>();
            List<Node> OpenSet = new List<Node>();
            var CurrentNode = start;
            ClosedSet.Add(CurrentNode.CellNode);
            do
            {
                foreach (var neighbor in CurrentNode.CellNode.Neighbors)
                {
                    if (neighbor.Type != NodeType.Obstacle)
                    {
                        if (ClosedSet.Contains(neighbor))
                            continue;
                        else
                        {
                            neighbor.Parent = CurrentNode.CellNode;

                            if (!OpenSet.Contains(neighbor))
                            {
                                neighbor.GetHeuristic(CurrentNode.CellNode);
                                OpenSet.Add(neighbor);
                            }
                        }
                    }
                }
                if (OpenSet.Count == 0)
                    break;
                var result = OpenSet.OrderBy(arg => arg.mHeuristic);
                CurrentNode = result.First().GetCell();
                OpenSet.Remove(CurrentNode.CellNode);
                ClosedSet.Add(CurrentNode.CellNode);
                CurrentNode.MarkVisited();
            } while (CurrentNode != dest);

            var currentNode = dest.CellNode;
            while (currentNode.Parent != null)
            {
                pathToReturn.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            return pathToReturn;
        }
    }
    public class Dijkstra : IPathFinder
    {
        public List<Node> FindPath(Graph graph, Cell start, Cell dest)
        {
            Node.shouldCompareByCost = true;
            Node.HeuristicFunction += Heuristics.EuclidianDistance;
            List<Node> pathToReturn = new List<Node>();
            List<Node> frontier = new List<Node>();
            List<Node> closedSet = new List<Node>();
            frontier.Add(start.CellNode);
            while(frontier.Count > 0)
            {
                var result = frontier.OrderBy(arg =>  arg.PathCost );
                var currentNode = result.First();
                frontier.Remove(currentNode);
                if (currentNode == dest.CellNode)
                    break;
                foreach(var neighbor in currentNode.Neighbors)
                {
                    if (neighbor.Type != NodeType.Obstacle)
                    {
                        var pathCost = currentNode.PathCost + Graph.Cost(currentNode, neighbor) ;
                        if (!closedSet.Contains(neighbor) || pathCost < neighbor.PathCost)
                        {
                            neighbor.PathCost = pathCost;
                            closedSet.Add(neighbor);
                            neighbor.GetCell().MarkVisited();
                            neighbor.Parent = currentNode;
                            frontier.Add(neighbor);
                        }
                    }
                }

            }
            var node = dest.CellNode;
            while (node != start.CellNode && node.Parent != null)
            {
                pathToReturn.Add(node);
                node = node.Parent;
            }
            return pathToReturn;
        }
    }
    public class AStar : IPathFinder
    {
        public List<Node> FindPath(Graph graph, Cell start, Cell dest)
        {
            SortedSet<Node> OpenSet = new SortedSet<Node>();
            List<Node> ClosedSet = new List<Node>();
            List<Node> pathToReturn = new List<Node>();
            var CurrentNode = start.CellNode;
            ClosedSet.Add(CurrentNode);
            do
            {
                foreach (var neighbor in CurrentNode.Neighbors)
                {
                    if (neighbor.Type == NodeType.Obstacle)
                        continue;
                    if (ClosedSet.Contains(neighbor))
                        continue;
                    var pathCost = CurrentNode.PathCost + Graph.Cost(CurrentNode, neighbor);
                    if (OpenSet.Contains(neighbor))
                    {
                        if (pathCost < neighbor.PathCost)
                        {
                            neighbor.Parent = CurrentNode;
                            neighbor.PathCost = pathCost;
                        }
                    }
                    else
                    {
                        neighbor.GetHeuristic(CurrentNode);
                        neighbor.Parent = CurrentNode;
                        neighbor.PathCost = pathCost;
                        OpenSet.Add(neighbor);
                    }
                }
                if (OpenSet.Count == 0) break;
                var result = OpenSet.OrderBy(arg => arg.TotalCost);
                CurrentNode = result.First();
                OpenSet.Remove(CurrentNode);
                ClosedSet.Add(CurrentNode);
                CurrentNode.GetCell().MarkVisited();
            } while (CurrentNode != dest.CellNode);

            var node = dest.CellNode;
            while (node != start.CellNode && node.Parent != null)
            {
                pathToReturn.Add(node);
                node = node.Parent;
            }
            return pathToReturn;
        }
    }
}
