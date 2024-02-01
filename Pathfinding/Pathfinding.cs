using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    public void FindPath(Vector3 startPos, Vector3 targetPos, int speed)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode, speed);
                return;
            }

            foreach (var neighbour in grid.GetNeighbourNodes4(currentNode))
            {
                if (neighbour != null)
                {
                    if (!neighbour.GetWalkable() || closedSet.Contains(neighbour) || neighbour.GetHasEntity() || Mathf.RoundToInt(Mathf.Abs(currentNode.GetHeight() - neighbour.GetHeight())) > grid.GetNodeSize())
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance4(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance4(neighbour, targetNode);
                        neighbour.SetParent(currentNode);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode, int budget)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetParent();
        }
        path.Reverse();

        List<Node> pathWithCost = new List<Node>();

        for (int i = 0; i < path.Count; i++)
        {
            if (budget != 0)
            {
                pathWithCost.Add(path[i]);
                budget--;
            }
        }

        if (pathWithCost.Contains(endNode))
        {
            grid.SetPath(pathWithCost);
            return;
        }
        grid.SetPath(new List<Node>());
    }

    private int GetDistance4(Node nodeA, Node nodeB)
    {
        int xA, yA, xB,yB;
        nodeA.GetCoordinates(out xA, out yA);
        nodeB.GetCoordinates(out xB, out yB);
        int dstX = Mathf.Abs(xA - xB);
        int dstY = Mathf.Abs(yA - yB);

        if (dstX > dstY)
        {
            return 10 * dstY + 10 * (dstX - dstY);
        }
        return 10 * dstX + 10 * (dstY - dstX);
    }

    private int GetDistance8(Node nodeA, Node nodeB)
    {
        int xA, yA, xB, yB;
        nodeA.GetCoordinates(out xA, out yA);
        nodeB.GetCoordinates(out xB, out yB);
        int dstX = Mathf.Abs(xA - xB);
        int dstY = Mathf.Abs(yA - yB);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
